using UnityEngine;

struct Range
{
    public int min; //min inclusive; 
    public int max; //max exclusive
}

public static class RandomSelect
{
    public static int SelectByChance(int[] chances)
    {
        if (chances != null && chances.Length > 0)
        {
            int summ = 0;
            foreach (int i in chances)
            {
                summ += i;
            }

            int randResult = Random.Range(0, summ);

            Range[] ranges = new Range[chances.Length];

            for (int i = 0; i < chances.Length; i++)
            {
                int startRange = 0;
                if (i != 0)
                    startRange = ranges[i - 1].max;

                ranges[i].min = startRange;
                ranges[i].max = startRange + chances[i];
            }
            
            int resultIndex = -1;

            for (int i = 0; i < ranges.Length; i++)
            {
                if (randResult >= ranges[i].min && randResult < ranges[i].max)
                {
                    resultIndex = i;
                    break;
                }
            }

            return resultIndex;
        }
        else
            throw new System.Exception("Chances length must be greater that zero and not null");
    }

    public static int[] Adaptee(IRandomSelectable[] list)
    {
        int[] result = new int[list.Length];
        for(int i = 0; i < list.Length; i++)
        {
            result[i] = list[i].spawnPriority;
        }
        return result;
    }
}

public interface IRandomSelectable
{
    public int spawnPriority { get; }
}
