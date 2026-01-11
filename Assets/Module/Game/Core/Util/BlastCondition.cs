using Unity.Collections;
using UnityEngine;

public struct BlastCondition
{
    public FixedString128Bytes name;
    public int order;
    public Direction direction;
    public int maxPossibleSelectTile;
    public int minCount;
    public int itemType;
}
