using System;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;


public enum ItemType : int
{
    None = 0,
    Blast = 16,
    Blast_Red = 17,
    Blast_Blue = 18,
    Blast_Green = 20,
    Blast_Yellow = 21,
    Special = 32,
    Special_1 = 33, 
    Special_2 = 34,
    Special_3 = 35,
    Obstacle = 64,
    Obstacle_1 = 65
}

public static class ItemTypeExtensions
{
    public static ItemType GetRandomBlastType(ref Random random)
    {
        Span<int> blastTypes = stackalloc int[]
        {
            (int)ItemType.Blast_Red,
            (int)ItemType.Blast_Blue,
            (int)ItemType.Blast_Green,
            (int)ItemType.Blast_Yellow
        };

        int randomIndex = random.NextInt(0, blastTypes.Length);
        return (ItemType)blastTypes[randomIndex];
    }

    public static ItemType GetRandomSpecialType(ref Random random)
    {
        Span<int> specialTypes = stackalloc int[]
        {
            (int)ItemType.Special_1,
            (int)ItemType.Special_2,
            (int)ItemType.Special_3
        };

        int randomIndex = random.NextInt(0, specialTypes.Length);
        return (ItemType)specialTypes[randomIndex];
    }
}
