using System;

public class LevelData
{
    public int Width { get; }
    public int Height { get; }
    public int Moves { get; }
    public ItemType[] Items { get; }
    public bool[] ExcludedCells { get; }

    public LevelData(int width, int height, int moves, ItemType[] items, bool[] excludedCells)
    {
        if (width <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(width), "Width must be positive.");
        }

        if (height <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(height), "Height must be positive.");
        }

        if (moves < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(moves), "Moves cannot be negative.");
        }

        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        if (excludedCells == null)
        {
            throw new ArgumentNullException(nameof(excludedCells));
        }

        int expectedLength = width * height;
        if (items.Length != expectedLength)
        {
            throw new ArgumentException("Items array length must match width * height.", nameof(items));
        }

        if (excludedCells.Length != expectedLength)
        {
            throw new ArgumentException("Excluded cells array length must match width * height.", nameof(excludedCells));
        }

        Width = width;
        Height = height;
        Moves = moves;
        Items = items;
        ExcludedCells = excludedCells;
    }

    public ItemType GetItem(int x, int y)
    {
        return Items[GetIndex(x, y)];
    }

    public bool IsExcluded(int x, int y)
    {
        return ExcludedCells[GetIndex(x, y)];
    }

    private int GetIndex(int x, int y)
    {
        return (y * Width) + x;
    }
}
