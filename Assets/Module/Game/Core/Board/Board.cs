using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

public struct BoardConfig
{
    public int width;
    public int height;
    List<int2> excludeList;
}

public class Board 
{
    private const int ExcludedItemIndex = -1;

    private BoardCell[] _cells;
    private Item[] _items;
    private BlastRule[] _blastConditions;

    private readonly BoardConfig boardData;

    public  Board(BoardConfig boardData)
    {
        this.boardData = boardData;
        int size = boardData.width * boardData.height;
        _cells = new BoardCell[size];
        _items = new Item[size * 2];

        var blastConditions = new List<BlastRule>();

        BlastRule special1Condition = new BlastRule()
        {
            name = "Special 1 Blast Rule",
            order = 9,
            direction = Direction.Right | Direction.Up,
            maxPossibleSelectTile = boardData.height * boardData.width,
            itemType = ItemType.Special_1,
            blastSizes = new BlastRequirement[]
            {
                new BlastRequirement() { min = 3, direction = Direction.Right | Direction.Left },
                new BlastRequirement() { min = 3, direction = Direction.Up | Direction.Down },
            }
        };

        BlastRule verticalCondition = new BlastRule()
        {
            name = "Vertical Blast Rule",
            order = 10,
            direction = Direction.Up | Direction.Down,
            maxPossibleSelectTile = boardData.height,
            itemType = ItemType.None,
            blastSizes = new BlastRequirement[]
            {
                new BlastRequirement() { min = 3, direction = Direction.Up | Direction.Down },
            }
        };

        BlastRule horizontalCondition = new BlastRule()
        {
            name = "Horizontal Blast Rule",
            order = 10,
            direction = Direction.Right | Direction.Left,
            maxPossibleSelectTile = boardData.width,
            itemType = ItemType.None,
            blastSizes = new BlastRequirement[]
            {
                new BlastRequirement() { min = 3, direction = Direction.Right | Direction.Left },
            }
        };

        blastConditions.Add(special1Condition);
        blastConditions.Add(verticalCondition);
        blastConditions.Add(horizontalCondition);

        _blastConditions = blastConditions.ToArray();
    }

    public void Initialize(LevelData data, ref Random random)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        int width = data.Width;
        int height = data.Height;
        int size = width * height;

        if (_cells == null || _cells.Length != size)
        {
            _cells = new BoardCell[size];
        }

        if (_items == null || _items.Length < size)
        {
            _items = new Item[size];
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = (y * width) + x;
                bool isExcluded = data.ExcludedCells[index];

                BoardCell cell = new BoardCell();

                if (isExcluded)
                {
                    cell.itemIndex = ExcludedItemIndex;
                    cell.upCellIndex = -1;
                    cell.downCellIndex = -1;
                    cell.rightCellIndex = -1;
                    cell.leftCellIndex = -1;
                    _cells[index] = cell;
                    continue;
                }

                cell.itemIndex = index;

                ItemType itemType = data.Items[index];
                if (itemType == ItemType.None)
                {
                    itemType = ItemTypeExtensions.GetRandomBlastType(ref random);
                }

                _items[index] = new Item
                {
                    itemType = itemType,
                    health = 0
                };

                cell.upCellIndex = GetAdjacentIndex(x, y + 1, width, height, data.ExcludedCells);
                cell.downCellIndex = GetAdjacentIndex(x, y - 1, width, height, data.ExcludedCells);
                cell.rightCellIndex = GetAdjacentIndex(x + 1, y, width, height, data.ExcludedCells);
                cell.leftCellIndex = GetAdjacentIndex(x - 1, y, width, height, data.ExcludedCells);

                _cells[index] = cell;
            }
        }
    }

    private static int GetAdjacentIndex(int x, int y, int width, int height, bool[] excludedCells)
    {
        if (x < 0 || x >= width || y < 0 || y >= height)
        {
            return -1;
        }

        int index = (y * width) + x;
        return excludedCells[index] ? -1 : index;
    }

    public BoardUpdateResult OnBoardUpdate(int cellIndex, Direction direction, ref Random random)
    {
        BoardUpdateResult boardHistory = new BoardUpdateResult();

        //Swap Items
        BoardCell cell = _cells[cellIndex];

        int targetCellIndex = cell.GetDirectionIndex(direction);
        if (targetCellIndex == -1)
        {
            throw new Exception();
        }
        BoardCell targetCell = _cells[targetCellIndex];

        SwapCellItem(cellIndex, targetCellIndex);

        Item item = _items[cell.itemIndex];
        Item targetItem = _items[targetCell.itemIndex];

        boardHistory.moveFirst = new SwapMove()
        {
            cellFrom = cell,
            cellTo = targetCell,
            itemFrom = item,
            itemTo = targetItem,
        };

        bool isBlast = false;

        if (item.itemType.HasFlag(ItemType.Blast))
        {
            for (int i = 0; i < _blastConditions.Length; i++)
            {
                BlastRule blastCondition = _blastConditions[i];

                Span<int> selectedTiles = stackalloc int[blastCondition.maxPossibleSelectTile];
                selectedTiles.Fill(-1);

                Span<int> countedTiles = stackalloc int[(int)Direction.Count];
                countedTiles.Fill(0);

                int count = CheckBlastTile(cellIndex, selectedTiles, countedTiles, blastCondition.direction);
                isBlast = blastCondition.IsBlastSizeValid(countedTiles);

                if (isBlast)
                {
                    boardHistory.blastedTileIndexes = new int[count];
                    selectedTiles[..count].CopyTo(boardHistory.blastedTileIndexes);
                    boardHistory.blastConditionDebugName = blastCondition.name;
                    break;
                }
            }
        }

        if (isBlast)
        {
            for (int i = 0; i < boardHistory.blastedTileIndexes.Length; i++)
            {
                int blastedIndex = boardHistory.blastedTileIndexes[i];
                int itemIndex = _cells[blastedIndex].itemIndex;
                _items[itemIndex].itemType = ItemTypeExtensions.GetRandomBlastType(ref random);
            }
        }

        boardHistory.isReturn = !isBlast;

        if (!isBlast)
        {
            SwapCellItem(cellIndex, targetCellIndex);
        }

        return boardHistory;
    }

    private int CheckBlastTile(int beginCellIndex, Span<int> selectedCells, Span<int> countedTiles, Direction direction)
    {
        BoardCell cell = _cells[beginCellIndex];
        int count = 0;

        Queue<int> cellIndexQueue = new();
        cellIndexQueue.Enqueue(beginCellIndex);
        Item targetItem = _items[cell.itemIndex];

        while (cellIndexQueue.Count != 0)
        {
            int cellIndex = cellIndexQueue.Dequeue(); 
            BoardCell cellQueue = _cells[cellIndex];

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
                    countedTiles[((Direction)(1 << i)).GetDirectionIndex()]++;
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
