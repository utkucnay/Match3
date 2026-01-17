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
    Blast_Purple = 21,
    Special = 32,
    Special_1 = 33, 
    Special_2 = 34,
    Special_3 = 35,
    Obstacle = 64,
    Obstacle_1 = 65
}

public static class ItemTypeExtensions
{
    public static int ItemTypeToIndex(this ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Blast_Red:
                return 0;
            case ItemType.Blast_Blue:
                return 1;
            case ItemType.Blast_Green:
                return 2;
            case ItemType.Blast_Purple:
                return 3;
            case ItemType.Obstacle_1:
                return 4;
            case ItemType.Special_1:
                return 5;
            case ItemType.Special_2:
                return 6;
            case ItemType.Special_3:
                return 7;
            default:
                return -1;            
        }
    }

    public static ItemType GetRandomBlastType(ref Random random)
    {
        Span<int> blastTypes = stackalloc int[]
        {
            (int)ItemType.Blast_Red,
            (int)ItemType.Blast_Blue,
            (int)ItemType.Blast_Green,
            (int)ItemType.Blast_Purple
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
