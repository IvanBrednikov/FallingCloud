using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//порядок генерации карты:
//1)создание чанков
//2)создание еды
//3)создание нижних препятсвий
//4)создание разделительных облаков
//5)коррекция большой разницы высот облаков
//6)создание верхних препятствий с коррекцией высоты по соседним облакам

//рекомендация: держать разницу между дальностью создания верхних и нижних препятсвий не меньше чем в 2 раза,
//чтобы избежать неправильного размещения верхних препятствий до этапа корректировки облаков

public class MapGenerator : MonoBehaviour
{
    [SerializeField] Cloud player;
    [SerializeField] PlayerProgress progress;
    [SerializeField] GameProgress gameProgress;

    [SerializeField] GameObject chunkPrefab;
    [SerializeField] GameObject cloud1Prefab;
    [SerializeField] GameObject bigCloudPrefab;

    //generator
    bool generateMap = true;
    [SerializeField] bool useRandomSeed = true;
    [SerializeField] int seed;
    [SerializeField] bool spawnObstacles = true;
    [SerializeField] bool spawnFood = true;

    //chunks
    [SerializeField] float chunkInterval = 15;
    [SerializeField] float chunkSpawnFarness = 30;
    Queue<GameObject> chunks;
    float lastChunkPosition = 0;
    GameObject firstChunkInQueue;

    //portal
    [SerializeField] GameObject portalPrefab;
    [SerializeField] float portalExtentByRightBoard = 20;

    //food
    [SerializeField] int firstChunkWithFoodPosX;
    [SerializeField] int maxFoodOnChunk;
    [SerializeField] int minFoodOnChunk;
    [SerializeField] Vector2 chunkXBoard;
    [SerializeField] Vector2 chunkYBoard;
    [SerializeField] FoodVariant[] foodVariants;

    //obstacle
    [SerializeField] float obstacleSpawnFarness = 30;
    float nextObstaclePosition = 0;
    [SerializeField] float obstacleStartPosition = 20;

    [SerializeField] float doNotSpawnBigCloudChance = 5; //%
    [SerializeField] ObstalceVariant[] obstacleVariants;
    [SerializeField] UpperObstacleVariant[] upperObstacleVariants;
    [SerializeField] UpperObstacleVariant[] specialUpperVariants;

    //upper obstacle params
    float lastUpperObstaclePosition = 0;
    [SerializeField] float upperObstacleSpawnFarness = 20;
    const float spaceBoardY = 37f;
    [SerializeField] int specialObstacleSpawnChance = 20;//from 100%
    [SerializeField] float defaultActualUpperPathWidth = 15; //значение ширины прохода при отсутсвии облаков
    [SerializeField] float bigCloudHalfHeight = 4f; //усреднённый отступ от центра разделительного облака до его края
    [SerializeField] float upperObstacleOffsetTolerance; //допустимое смещение препятсвия на верхнем пути, чтобы оно поместилось в расчётную ширину пути
    [SerializeField] float defaultUpperInterval = 15; //стандартный интервал между препятствиями на случай пропуска точки спавна
    [SerializeField] float maxDistanceBigCloudSearch = 11; //максимальная дистанция до центра облака,
                                                           //относительно которого будет выполнена коррекция верхнего препятствия,
                                                           //рекомендованное значение - длинна разделительного облака + отступ

    //коррекция высот
    GameObject lastSpawnedBigCloud;
    [SerializeField] float maxYDifferenceBtwBigClouds = 7;
    [SerializeField] float bigCloudOffsetByCorrectPercent = 0.5f;
    [SerializeField] float maxIntervalForCorrect = 15;

    //модификаторы сложности и размера игрока
    [SerializeField] SizeModifier[] sizeModifiers;
    [SerializeField] DifficultyModifier[] difficultyModifiers;

    //debug
    [SerializeField] bool showDebugLines;

    public event SimpleEvent OnPortalSpawn;

    private void Start()
    {
        SetSeed();
        nextObstaclePosition = obstacleStartPosition;
        lastUpperObstaclePosition = obstacleStartPosition;
        chunks = new Queue<GameObject>();
        CreateChunks();
        firstChunkInQueue = chunks.Dequeue();
    }

    void SetSeed()
    {
        if (useRandomSeed)
        {
            seed = Random.Range(int.MinValue, int.MaxValue);
            Debug.Log("Map generation seed: " + seed);
        }
        Random.InitState(seed);
    }

    GameObject CreateChunks()
    {
        GameObject result = Instantiate(chunkPrefab);
        result.transform.position = new Vector3(lastChunkPosition, 0, 0);
        lastChunkPosition += chunkInterval;
        chunks.Enqueue(result);
        SpawnFood(lastChunkPosition, result.transform);
        return result;
    }

    void SpawnFood(float chunkXPosition, Transform foodParent)
    {
        if(spawnFood && chunkXPosition > firstChunkWithFoodPosX)
        {
            int foodAmount = Random.Range(minFoodOnChunk, maxFoodOnChunk);
            int[] priorities = RandomSelect.Adaptee(foodVariants);

            while (foodAmount > 0)
            {
                int index = RandomSelect.SelectByChance(priorities);
                GameObject foodObj = Instantiate(foodVariants[index].prefab);
                Food foodComponent = foodObj.GetComponent<Food>();
                foodAmount -= (int)foodComponent.FoodCount;
                float x = Random.Range(chunkXBoard.x, chunkXBoard.y) + chunkXPosition;
                float y = Random.Range(chunkYBoard.x, chunkYBoard.y);
                foodObj.transform.position = new Vector3(x, y, 0);
                foodObj.transform.parent = foodParent;
            }
        }
    }

    void SpawnObstacles()
    {
        float maxObstaclePosition = player.transform.position.x + obstacleSpawnFarness;
        int[] priorities = RandomSelect.Adaptee(obstacleVariants);

        while (maxObstaclePosition > nextObstaclePosition)
        {
            //поиск ближайшего чанка для автоудаления препятсвий за пределами игровой области
            GameObject[] allChunks = GameObject.FindGameObjectsWithTag("chunk");
            Transform nearestChunk = null;

            if (allChunks.Length > 0)
            {
                float minDistance = Vector2.Distance(allChunks[0].transform.position, new Vector2(nextObstaclePosition, 0));
                nearestChunk = allChunks[0].transform;

                for (int i = 0; i < allChunks.Length; i++)
                {
                    float distance = Vector2.Distance(allChunks[i].transform.position, new Vector2(nextObstaclePosition, 0));
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearestChunk = allChunks[i].transform;
                    }
                }
            }

            //выбор варианта
            int variantNumber = RandomSelect.SelectByChance(priorities);
            ObstalceVariant variant = obstacleVariants[variantNumber];
            Vector3 OPosition = new Vector3(nextObstaclePosition + variant.leftInterval, variant.pivotY, -1);

            //использование модификаторов для смещения
            SizeModifier sizeModifier = GetSizeModifier;
            DifficultyModifier difficultyModifier = GetDifficultyModifier;
            OPosition.x += sizeModifier.xMargin + difficultyModifier.xMargin;

            //создание препятствия на змеле
            if (variant.prefab != null)
            {
                GameObject obstacle = Instantiate(variant.prefab);
                obstacle.transform.position = OPosition;
                obstacle.transform.parent = nearestChunk;
            }

            //создание разделительной линнии большими облаками
            if (variant.spawnBigCloud && Random.Range(0, 100) >= doNotSpawnBigCloudChance)
            {
                GameObject cloudObstacle = Instantiate(bigCloudPrefab);
                cloudObstacle.transform.position = new Vector3(nextObstaclePosition + variant.leftInterval, variant.cloudOfRoofHeight + sizeModifier.yMargin + difficultyModifier.yMargin, -2);
                cloudObstacle.transform.parent = nearestChunk;

                //исправление резких перепадов высот
                if (lastSpawnedBigCloud != null)
                {
                    float heightDifference = Mathf.Abs(lastSpawnedBigCloud.transform.position.y - cloudObstacle.transform.position.y);
                    float interval = cloudObstacle.transform.position.x - lastSpawnedBigCloud.transform.position.x;
                    if (heightDifference > maxYDifferenceBtwBigClouds && interval <= maxIntervalForCorrect)
                    {
                        bool lastCloudHigher = lastSpawnedBigCloud.transform.position.y > cloudObstacle.transform.position.y;
                        float newHeight = heightDifference * bigCloudOffsetByCorrectPercent;
                        if (lastCloudHigher)
                        {
                            newHeight = cloudObstacle.transform.position.y + newHeight;
                            cloudObstacle.transform.position = new Vector2(cloudObstacle.transform.position.x, newHeight);
                        }
                        else
                        {
                            newHeight = lastSpawnedBigCloud.transform.position.y + newHeight;
                            lastSpawnedBigCloud.transform.position = new Vector2(lastSpawnedBigCloud.transform.position.x, newHeight);
                        }
                    }
                }

                lastSpawnedBigCloud = cloudObstacle;
            }

            //make interval randomized
            nextObstaclePosition += variant.rightInterval + variant.leftInterval + sizeModifier.xMargin + difficultyModifier.xMargin;
        }
    }

    void SpawnSecondLevelObstacles()
    {
        
        float maxObstaclePosition = player.transform.position.x + upperObstacleSpawnFarness;

        while (maxObstaclePosition > lastUpperObstaclePosition)
        {
            float cacheX = lastUpperObstaclePosition;
            //поиск ближайшего чанка для автоудаления препятсвий за пределами игровой области
            GameObject[] allChunks = GameObject.FindGameObjectsWithTag("chunk");
            Transform nearestChunk = null;

            if (allChunks.Length > 0)
            {
                float minDistance = Vector2.Distance(allChunks[0].transform.position, new Vector2(lastUpperObstaclePosition, 0));
                nearestChunk = allChunks[0].transform;

                for (int i = 0; i < allChunks.Length; i++)
                {
                    float distance = Vector2.Distance(allChunks[i].transform.position, new Vector2(lastUpperObstaclePosition, 0));
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearestChunk = allChunks[i].transform;
                    }
                }
            }

            //проверка возможности создания препятсвия
            float actualPathWidth;
            float obstacleX = lastUpperObstaclePosition;
            bool canSpawnObstacle = CheckWithOfPath(out actualPathWidth, ref obstacleX);

            //изначальный список препятсвий
            List<UpperObstacleVariant> variantsList = new List<UpperObstacleVariant>();
            foreach (UpperObstacleVariant variant in upperObstacleVariants)
                variantsList.Add(variant);

            //добавление особых препятсвий
            if (Random.Range(0, 100) <= specialObstacleSpawnChance)
            {
                foreach (UpperObstacleVariant variant in specialUpperVariants)
                    variantsList.Add(variant);
            }

            //фильтр невмещающихся вариантов препятсвий
            List<UpperObstacleVariant> filteredVariants = new List<UpperObstacleVariant>();
            foreach (UpperObstacleVariant variant in variantsList)
                if (actualPathWidth - (variant.obstacleFullHeight - upperObstacleOffsetTolerance) >= GetMinimalUpperPathWidth)
                    filteredVariants.Add(variant);

            if (canSpawnObstacle && filteredVariants.Count > 0)
            {
                //выбор наибольшего препятсвия
                UpperObstacleVariant selectedVariant = filteredVariants[0];
                float maxHeight = filteredVariants[0].obstacleFullHeight;
                foreach (UpperObstacleVariant variant in filteredVariants)
                    if (variant.obstacleFullHeight > maxHeight)
                    {
                        selectedVariant = variant;
                        maxHeight = variant.obstacleFullHeight;
                    }

                //определение положения препятствия
                bool obstacleOnUp = Random.Range(0, 2) == 1;
                float y;
                if (obstacleOnUp)
                    y = spaceBoardY - actualPathWidth + GetMinimalUpperPathWidth + (selectedVariant.obstacleFullHeight / 2);
                else
                    y = spaceBoardY - GetMinimalUpperPathWidth - (selectedVariant.obstacleFullHeight / 2);
                
                //создание препятсвия
                GameObject obstacle = Instantiate(selectedVariant.prefab);
                obstacle.transform.parent = nearestChunk;
                obstacle.transform.position = new Vector3(obstacleX, y, -2);
                if (obstacleOnUp)
                    obstacle.transform.rotation = Quaternion.Euler(0, 0, 180); //ОТЛАДКА

                lastUpperObstaclePosition = obstacle.transform.position.x + selectedVariant.rightInterval + GetDifficultyModifier.xMarginUpper;
            }
            else
            {
                //пропуск точки для спавна
                lastUpperObstaclePosition += defaultUpperInterval;
            }
        }
    }

    /// <summary>
    /// Проверяет ширину прохода верхнего пути между резделительными облаками и верхней границей
    /// </summary>
    /// <param name="actualWidth">Фактическая ширина проёма</param>
    /// <param name="x">Позиция препятсвия по X, может быть скорректирована</param>
    /// <returns>Возвращает true если ширина соответсвует минимально возможной ширине, и false если проход слишком узкий для препятствий.</returns>
    bool CheckWithOfPath(out float actualWidth, ref float x)
    {
        bool result = false;

        //поиск максимальной высоты ближайших неповторяющихся 3 разделительных облаков
        GameObject[] allClouds = GameObject.FindGameObjectsWithTag("storm cloud");
        float maxY = 0; //Y самого высокого облака

        //фильтр по максимальному отдалению
        List<GameObject> filteredClouds = new List<GameObject>();
        for (int i = 0; i < allClouds.Length; i++)
        {
            float distance = Mathf.Abs(allClouds[i].transform.position.x - x);
            if (distance <= maxDistanceBigCloudSearch)
                filteredClouds.Add(allClouds[i]);
        }
        allClouds = filteredClouds.ToArray();

        if (allClouds.Length > 0)
        {
            //поиск соседних облаков
            Transform cloud1 = allClouds[0].transform;
            Transform cloud2 = null;
            Transform cloud3 = null;

            float minDistance = Mathf.Abs(allClouds[0].transform.position.x - x);
            int cloudsCount = 1;

            //поиск ближейшего облака - cloud1
            for (int i = 0; i < allClouds.Length; i++)
            {
                float distance = Mathf.Abs(allClouds[i].transform.position.x - x);
                if (distance < minDistance)
                {
                    cloud1 = allClouds[i].transform;
                    minDistance = distance;
                }
            }

            //поиск cloud2
            if (allClouds.Length > 1)
            {
                cloudsCount++;
                minDistance = float.NaN;

                for (int i = 0; i < allClouds.Length; i++)
                    if (allClouds[i].transform != cloud1)
                    {
                        float distance = Mathf.Abs(allClouds[i].transform.position.x - x);
                        if (float.IsNaN(minDistance) || distance < minDistance)
                        {
                            cloud2 = allClouds[i].transform;
                            minDistance = distance;
                        }
                    }
            }

            //поиск cloud3
            if (allClouds.Length > 2)
            {
                cloudsCount++;
                minDistance = float.NaN;

                for (int i = 0; i < allClouds.Length; i++)
                    if (allClouds[i].transform != cloud1 && allClouds[i].transform != cloud2)
                    {
                        float distance = Mathf.Abs(allClouds[i].transform.position.x - x);
                        if (float.IsNaN(minDistance) || distance < minDistance)
                        {
                            cloud3 = allClouds[i].transform;
                            minDistance = distance;
                        }
                    }
            }

            //определение самого высокого облака и его X,Y
            maxY = cloud1.position.y;

            if (cloud2 != null && cloud2.position.y > maxY)
            {
                maxY = cloud2.position.y;
                if (cloud2.position.x >= x)
                    x = cloud2.position.x;
            }

            if (cloud3 != null && cloud3.position.y > maxY)
            {
                maxY = cloud3.position.y;
                if(cloud3.position.x >= x)
                    x = cloud3.position.x;
            }

            //определение ширины прохода в точке X
            actualWidth = spaceBoardY - (maxY + bigCloudHalfHeight);
            if (actualWidth >= GetMinimalUpperPathWidth)
                result = true;
            else
                result = false;
        }
        else
        {
            //если не найдены облака в пределах maxDistance
            result = true;
            actualWidth = defaultActualUpperPathWidth;
        }

        return result;
    }

    float GetPlayerHeight
    {
        get
        {
            return GetSizeModifier.ownDimensions.y;
        }
    }

    float GetMinimalUpperPathWidth
    {
        get
        {
            return GetPlayerHeight + GetDifficultyModifier.yMarginUpper;
        }
    }

    public void GenerateNewMap()
    {
        DestroyAllChunks();
        chunks = new Queue<GameObject>();
        lastChunkPosition = 0;
        nextObstaclePosition = obstacleStartPosition;
        lastUpperObstaclePosition = obstacleStartPosition;
        SetSeed();
        CreateChunks();
        firstChunkInQueue = chunks.Dequeue();
        generateMap = true;
    }

    public void SpawnPortal()
    {
        if (generateMap)
        {
            generateMap = false;
            CreateChunks();
            CreateChunks();
            CreateChunks();
            GameObject portal = Instantiate(portalPrefab);
            portal.transform.position = new Vector3(lastChunkPosition - portalExtentByRightBoard, 20, 0);
            portal.transform.parent = chunks.ToArray()[chunks.Count-1].transform;
            OnPortalSpawn?.Invoke();
        }
    }

    public void DestroyAllChunks()
    {
        while (chunks.Count > 0)
        {
            GameObject chunk = chunks.Dequeue();
            Destroy(chunk);
        }
        if (firstChunkInQueue != null)
            Destroy(firstChunkInQueue);
    }

    private void Update()
    {
        if (generateMap)
        {
            if (player.transform.position.x >= lastChunkPosition - chunkSpawnFarness)
            {
                CreateChunks();
            }

            if (player.transform.position.x > nextObstaclePosition - obstacleSpawnFarness && spawnObstacles)
            {
                SpawnObstacles();
            }

            if (player.transform.position.x > lastUpperObstaclePosition - upperObstacleSpawnFarness && spawnObstacles)
            {
                SpawnSecondLevelObstacles();
            }

            if (player.transform.position.x > firstChunkInQueue.transform.position.x + chunkSpawnFarness)
            {
                Destroy(firstChunkInQueue);
                firstChunkInQueue = chunks.Dequeue();
            }
        }
    }

    [System.Serializable]
    public class ObstalceVariant : IRandomSelectable
    {
        public string name;
        public GameObject prefab;
        public float upperBoard; //Верхняя граница припятствия
        public float estimatedY; //центр расчитываемого пути ???
        public float leftInterval; //левый отступ от точки нового препятсвия
        public float rightInterval;
        public float cloudOfRoofHeight; //константная высота разделительного облака
        public float pivotY; //смещение для спавна препятсвия
        public bool spawnBigCloud = true;
        public int spawnPriority;

        int IRandomSelectable.spawnPriority => spawnPriority;

    }
    [System.Serializable]
    public class UpperObstacleVariant
    {
        public string name;
        public GameObject prefab;
        public float rightInterval;
        public float obstacleFullHeight;
    }
    [System.Serializable]
    public class FoodVariant : IRandomSelectable
    {
        public string name;
        public GameObject prefab;
        public int spawnPriority;

        int IRandomSelectable.spawnPriority => spawnPriority;
    }

    public SizeModifier GetSizeModifier
    {
        get
        {
            SizeModifier result = null;
            for (int i = 0; i < sizeModifiers.Length; i++)
                if (sizeModifiers[i].playerSize == player.GetSize)
                {
                    result = sizeModifiers[i];
                    break;
                }
            if (result != null)
                return result;
            else
                throw new System.Exception("Modifier for size \"" + player.GetSize + "\" not found.");
        }
    }

    [System.Serializable]
    public class SizeModifier
    {
        public string elementName;
        public CloudSize.ESize playerSize;
        public float yMargin;
        public float xMargin;
        public Vector2 ownDimensions;
    }
    public DifficultyModifier GetDifficultyModifier
    {
        get
        {
            DifficultyModifier result = null;
            for (int i = 0; i < difficultyModifiers.Length; i++)
                if (difficultyModifiers[i].difficulty == gameProgress.GetDifficulty)
                {
                    result = difficultyModifiers[i];
                    break;
                }
            if (result != null)
                return result;
            else
                throw new System.Exception("Modifier for difficulty \"" + gameProgress.GetDifficulty + "\" not found.");
        }
    }

    public int Seed { get => seed;}

    [System.Serializable]
    public class DifficultyModifier
    {
        public string elementName;
        public GameProgress.Difficulty difficulty;
        public float yMargin;
        public float xMargin;
        public float yMarginUpper;
        public float xMarginUpper;
    }
}