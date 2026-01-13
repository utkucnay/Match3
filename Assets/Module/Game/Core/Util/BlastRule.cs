using System;
using Unity.Collections;

public struct BlastRequirement
{
    public int min;
    public Direction direction;
}

public unsafe struct BlastRule
{
    public FixedString128Bytes name;
    public int order;
    public Direction direction;
    public int maxPossibleSelectTile;
    public ItemType itemType;
    public BlastRequirement[] blastSizes;
    public fixed int minCount[(int)Direction.Count];

    public Span<int> MinCount
    {
        get 
        {
            fixed (int* ptr = minCount)
            {
                return new Span<int>(ptr, (int)Direction.Count);
            }
        }
        
    }

    public int MinCountFind(Direction dir)
    {
        int count = 0;
        if (direction.HasFlag(dir))
        {
            count += minCount[dir.GetDirectionIndex()];
        }
        return count;
    }

    public bool IsBlastSizeValid(Span<int> countedTiles)
    {
        for( int i = 0; i < blastSizes.Length; i++)
        {
            BlastRequirement blastSize = blastSizes[i];
            int totalCount = 0;
            for (int j = 0; j < (int)Direction.Count; j++)
            {
                Direction dir = (Direction)(1 << j);
                if (blastSize.direction.HasFlag(dir))
                {
                    totalCount += countedTiles[j];
                }
            }

            if (totalCount + 1 < blastSize.min)
            {
                return false;
            }
        }
        return true;
    }
}

