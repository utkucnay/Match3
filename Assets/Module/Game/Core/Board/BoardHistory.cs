using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public struct Move
{
    public Cell cellFrom;
    public Cell cellTo;
    public Item itemFrom;
    public Item itemTo;
}

public struct BoardHistory
{
    public Move moveFirst;
    public int[] blastedTileIndexes;
    public bool isReturn;
    public FixedString128Bytes blastConditionDebugName;
}
