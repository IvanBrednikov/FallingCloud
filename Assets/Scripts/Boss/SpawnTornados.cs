using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTornados : BossAttack
{
    [SerializeField] GameObject tornadoPrefab;
    [SerializeField] Vector2 zeroPoint;
    [SerializeField] float xExtent;
    [SerializeField] float yExtent;
    [SerializeField] int tornadosCount;

    [SerializeField] float tornadoLifeTime;
    [SerializeField] float tornadoActivateTime;
    [SerializeField] float attackPrepareTime;

    Coroutine spawnCoroutine;

    [SerializeField] float spawnDelay;
    int tornadosPerSpawn;

    public override void Activate()
    {
        base.Activate();
        pointToMove = player.transform.position;
        tornadosPerSpawn = Mathf.FloorToInt(tornadosCount * (spawnDelay/AttackTime));

        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);
        spawnCoroutine = StartCoroutine(SpawnTornadoes());
    }

    public override void Deactivate()
    {
        base.Deactivate();
    }

    IEnumerator SpawnTornadoes()
    {
        yield return new WaitForSeconds(attackPrepareTime);
        

        while (attackIsActive)
        {
            for (int i = 0; i < tornadosPerSpawn; i++)
            {
                float x = Random.Range(-xExtent + zeroPoint.x, xExtent + zeroPoint.x);
                float y = Random.Range(-yExtent + zeroPoint.y, yExtent + zeroPoint.y);
                Vector2 position = new Vector2(x, y);

                GameObject tornado = Instantiate(tornadoPrefab);
                tornado.transform.position = position;
                StartCoroutine(ActivateTornados(tornado));
            }
            InvokeSoundEffect();
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    IEnumerator ActivateTornados(GameObject tornado)
    {
        yield return new WaitForSeconds(tornadoActivateTime);
        if (tornado != null)
        {
            tornado.GetComponent<Collider2D>().enabled = true;
            StartCoroutine(DestroyTornado(tornado));
        }
    }

    IEnumerator DestroyTornado(GameObject tornado)
    {
        yield return new WaitForSeconds(tornadoLifeTime);

        if (tornado != null)
            Destroy(tornado);
    }
}
