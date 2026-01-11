using System;
using System.Collections.Generic;
using Unity.Mathematics;

public struct BoardData
{
    public int width;
    public int height;
    List<int2> excludeList;
}

public class Board 
{
    private Cell[] _cells;
    private Item[] _items;
    private BlastCondition[] _blastConditions;

    private readonly BoardData boardData;

    public Board(BoardData boardData)
    {
        this.boardData = boardData;
        int size = boardData.width * boardData.height;
        _cells = new Cell[size];
        _items = new Item[size * 2];

        var blastConditions = new List<BlastCondition>();

        BlastCondition special1Condition = new BlastCondition()
        {
            name = "Special 1 Blast Condition",
            order = 9,
            direction = Direction.Up | Direction.Down,
            maxPossibleSelectTile = boardData.height,
            minCount = 3,
        };

        BlastCondition verticalCondition = new BlastCondition()
        {
            name = "Vertical Blast Condition",
            order = 10,
            direction = Direction.Up | Direction.Down,
            maxPossibleSelectTile = boardData.height,
            minCount = 3
        };

        BlastCondition horizontalCondition = new BlastCondition()
        {
            name = "Horizontal Blast Condition",
            order = 10,
            direction = Direction.Right | Direction.Left,
            maxPossibleSelectTile = boardData.width,
            minCount = 3,
        };

        blastConditions.Add(verticalCondition);
        blastConditions.Add(horizontalCondition);

        _blastConditions = blastConditions.ToArray();
    }

    public BoardHistory OnBoardUpdate(int cellIndex, Direction direction)
    {
        BoardHistory boardHistory = new BoardHistory();

        //Swap Items
        Cell cell = _cells[cellIndex];

        int targetCellIndex = cell.GetDirectionIndex(direction);
        if (targetCellIndex == -1)
        {
            throw new Exception();
        }
        Cell targetCell = _cells[targetCellIndex];

        SwapCellItem(cellIndex, targetCellIndex);

        Item item = _items[cell.itemIndex];
        Item targetItem = _items[targetCell.itemIndex];

        boardHistory.moveFirst = new Move()
        {
            cellFrom = cell,
            cellTo = targetCell,
            itemFrom = item,
            itemTo = targetItem,
        };

        //Check For Blast Tile
        bool isBlast = false;

        if (item.itemType.HasFlag(ItemType.Blast))
        {
            for (int i = 0; i < _blastConditions.Length; i++)
            {
                BlastCondition blastCondition = _blastConditions[i];

                Span<int> selectedTiles = stackalloc int[blastCondition.maxPossibleSelectTile];
                selectedTiles.Fill(-1);

                int count = CheckBlastTile(cellIndex, selectedTiles, blastCondition.direction);
                isBlast = count >= blastCondition.minCount;

                if (isBlast)
                {
                    boardHistory.blastedTileIndexes = new int[count];
                    selectedTiles[..count].CopyTo(boardHistory.blastedTileIndexes);
                    boardHistory.blastConditionDebugName = blastCondition.name;
                    break;
                }
            }
        }

        boardHistory.isReturn = !isBlast;

        if (!isBlast)
        {
            SwapCellItem(cellIndex, targetCellIndex);
        }

        return boardHistory;
    }

    private int CheckBlastTile(int beginCellIndex, Span<int> selectedCells, Direction direction)
    {
        Cell cell = _cells[beginCellIndex];
        int count = 0;

        Queue<int> cellIndexQueue = new();
        cellIndexQueue.Enqueue(beginCellIndex);

        Item targetItem = _items[cell.itemIndex];

        while (cellIndexQueue.Count != 0)
        {
            int cellIndex = cellIndexQueue.Dequeue(); 
            Cell cellQueue = _cells[cellIndex];

            if (selectedCells.IndexOf(cellIndex) == -1)
            {
                selectedCells[count] = cellIndex;
                count++;
            }

            Span<int> cellIndexes = stackalloc int[(int)Direction.Count]
            {
                cellQueue.upCellIndex,
                cellQueue.downCellIndex,
                cellQueue.rightCellIndex,
                cellQueue.leftCellIndex,
                cellQueue.upCellIndex != -1 ? _cells[cellQueue.upCellIndex].rightCellIndex : -1,
                cellQueue.upCellIndex != -1 ? _cells[cellQueue.upCellIndex].leftCellIndex : -1,
                cellQueue.downCellIndex != -1 ?_cells[cellQueue.downCellIndex].rightCellIndex : -1,
                cellQueue.downCellIndex != -1 ? _cells[cellQueue.downCellIndex].leftCellIndex : -1,
            };

            for (int i = 0; i < cellIndexes.Length; i++)
            {
                if (!direction.HasFlag((Direction)(1 << i)))
                {
                    continue;
                }

                if (cellIndexes[i] == -1)
                {
                    continue;
                }

                if (selectedCells.IndexOf(cellIndexes[i]) != -1)
                {
                    continue;
                }

                if (targetItem.itemType == _items[_cells[cellIndexes[i]].itemIndex].itemType)
                {
                    selectedCells[count] = cellIndexes[i];
                    cellIndexQueue.Enqueue(cellIndexes[i]);
                    count++;
                }    
            }
        }

        return count;
    }

    private void SwapCellItem(int cellFromIndex, int cellToIndex)
    {
        int swapItem1 = _cells[cellFromIndex].itemIndex;
        int swapItem2 = _cells[cellToIndex].itemIndex;
        
        _cells[cellFromIndex].itemIndex = swapItem2;
        _cells[cellToIndex].itemIndex = swapItem1;
    }
}
