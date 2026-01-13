using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public struct SwapMove
{
    public BoardCell cellFrom;
    public BoardCell cellTo;
    public Item itemFrom;
    public Item itemTo;
}

public struct BoardUpdateResult
{
    public SwapMove moveFirst;
    public int[] blastedTileIndexes;
    public bool isReturn;
    public FixedString128Bytes blastConditionDebugName;
}

