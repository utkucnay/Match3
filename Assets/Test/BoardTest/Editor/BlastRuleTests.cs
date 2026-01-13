using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BlastRuleTests
{
    [Test]
    public void OnBoardUpdate_WhenBlastColumnExists_ReturnsBlastedTiles()
    {
        var data = new BoardConfig { width = 5, height = 5 };
        var board = new Board(data);

        var cells = BuildCells(data.width, data.height);
        var items = BuildItems(data.width, data.height, ItemType.Special_1);

        int[] blastIndexes =
        {
            IndexOf(2, 1, data.width),
            IndexOf(2, 2, data.width),
            IndexOf(2, 3, data.width),
            IndexOf(2, 4, data.width)
        };

        for (int i = 0; i < blastIndexes.Length; i++)
        {
            items[blastIndexes[i]].itemType = ItemType.Blast_Red;
        }

        SetPrivateField(board, "_cells", cells);
        SetPrivateField(board, "_items", items);

        var history = board.OnBoardUpdate(IndexOf(2, 2, data.width), Direction.Up);

        Assert.IsFalse(history.isReturn, "Expected blast to mark move as non-return.");
        Assert.IsNotNull(history.blastedTileIndexes, "Expected blastedTileIndexes to be populated.");
        CollectionAssert.AreEquivalent(blastIndexes, history.blastedTileIndexes.ToList().Where(x => x != -1).OrderBy(x => x));
    }

    [Test]
    public void OnBoardUpdate_WhenVerticalBlast_SetsVerticalDebugName()
    {
        var data = new BoardConfig { width = 5, height = 5 };
        var board = new Board(data);

        var cells = BuildCells(data.width, data.height);
        var items = BuildItems(data.width, data.height, ItemType.Special_1);

        int[] blastIndexes =
        {
            IndexOf(2, 1, data.width),
            IndexOf(2, 2, data.width),
            IndexOf(2, 3, data.width)
        };

        for (int i = 0; i < blastIndexes.Length; i++)
        {
            items[blastIndexes[i]].itemType = ItemType.Blast_Red;
        }

        SetPrivateField(board, "_cells", cells);
        SetPrivateField(board, "_items", items);

        var history = board.OnBoardUpdate(IndexOf(2, 2, data.width), Direction.Up);

        Assert.IsFalse(history.isReturn, "Expected blast to mark move as non-return.");
        Assert.AreEqual("Vertical Blast Rule", history.blastConditionDebugName.ToString());
    }

    [Test]
    public void OnBoardUpdate_WhenHorizontalBlast_SetsHorizontalDebugName()
    {
        var data = new BoardConfig { width = 5, height = 5 };
        var board = new Board(data);

        var cells = BuildCells(data.width, data.height);
        var items = BuildItems(data.width, data.height, ItemType.Special_1);

        int[] blastIndexes =
        {
            IndexOf(1, 2, data.width),
            IndexOf(2, 2, data.width),
            IndexOf(3, 2, data.width)
        };

        for (int i = 0; i < blastIndexes.Length; i++)
        {
            items[blastIndexes[i]].itemType = ItemType.Blast_Red;
        }

        SetPrivateField(board, "_cells", cells);
        SetPrivateField(board, "_items", items);

        var history = board.OnBoardUpdate(IndexOf(2, 2, data.width), Direction.Right);

        Assert.IsFalse(history.isReturn, "Expected blast to mark move as non-return.");
        Assert.AreEqual("Horizontal Blast Rule", history.blastConditionDebugName.ToString());
    }

    [Test]
    public void OnBoardUpdate_WhenNoBlast_ReturnsIsReturnTrueAndNoBlastedTiles()
    {
        var data = new BoardConfig { width = 3, height = 3 };
        var board = new Board(data);

        var cells = BuildCells(data.width, data.height);
        var items = BuildItems(data.width, data.height, ItemType.Special_1);

        SetPrivateField(board, "_cells", cells);
        SetPrivateField(board, "_items", items);

        var history = board.OnBoardUpdate(IndexOf(1, 1, data.width), Direction.Up);

        Assert.IsTrue(history.isReturn, "Expected move to be marked as return when no blast occurs.");
        Assert.IsNull(history.blastedTileIndexes, "Expected no blasted tiles when no blast occurs.");
    }

    [Test]
    public void OnBoardUpdate_WhenNoBlast_ReturnsSwappedTiles()
    {
        var data = new BoardConfig { width = 2, height = 1 };
        var board = new Board(data);

        var cells = BuildCells(data.width, data.height);
        var items = BuildItems(data.width, data.height, ItemType.Special_1);

        cells[0].itemIndex = 0;
        cells[1].itemIndex = 1;
        items[0].itemType = ItemType.Special_1;
        items[1].itemType = ItemType.Special_2;

        SetPrivateField(board, "_cells", cells);
        SetPrivateField(board, "_items", items);

        var history = board.OnBoardUpdate(0, Direction.Right);

        var updatedCells = GetPrivateField<BoardCell[]>(board, "_cells");

        Assert.IsTrue(history.isReturn, "Expected move to be marked as return when no blast occurs.");
        Assert.AreEqual(0, updatedCells[0].itemIndex, "Expected cell 0 to return to item 0 after swap back.");
        Assert.AreEqual(1, updatedCells[1].itemIndex, "Expected cell 1 to return to item 1 after swap back.");
    }

    private static int IndexOf(int x, int y, int width)
    {
        return (y * width) + x;
    }

    private static BoardCell[] BuildCells(int width, int height)
    {
        int size = width * height;
        var cells = new BoardCell[size];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = IndexOf(x, y, width);

                cells[index] = new BoardCell
                {
                    itemIndex = index,
                    upCellIndex = y + 1 < height ? IndexOf(x, y + 1, width) : -1,
                    downCellIndex = y - 1 >= 0 ? IndexOf(x, y - 1, width) : -1,
                    rightCellIndex = x + 1 < width ? IndexOf(x + 1, y, width) : -1,
                    leftCellIndex = x - 1 >= 0 ? IndexOf(x - 1, y, width) : -1
                };
            }
        }

        return cells;
    }

    private static Item[] BuildItems(int width, int height, ItemType defaultType)
    {
        int size = width * height;
        var items = new Item[size * 2];

        for (int i = 0; i < size; i++)
        {
            items[i] = new Item { itemType = defaultType, health = 1 };
        }

        return items;
    }

    private static void SetPrivateField<T>(object target, string fieldName, T value)
    {
        var field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        if (field == null)
        {
            throw new InvalidOperationException($"Field '{fieldName}' not found on {target.GetType().Name}.");
        }

        field.SetValue(target, value);
    }

    private static T GetPrivateField<T>(object target, string fieldName)
    {
        var field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        if (field == null)
        {
            throw new InvalidOperationException($"Field '{fieldName}' not found on {target.GetType().Name}.");
        }

        return (T)field.GetValue(target);
    }
}


