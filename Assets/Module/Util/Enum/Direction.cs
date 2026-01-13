using UnityEngine;

public enum Direction
{
    Up = 1,
    Down = 2,
    Right = 4,
    Left = 8,
    Up_Right = 16,
    Up_Left = 32,
    Down_Right = 64,
    Down_Left = 128,
    Count = 8
}

public static class DirectionExtensions
{
    public static int GetDirectionIndex(this Direction dir)
    {
        switch (dir)
        {
            case Direction.Up:
                return 0;
            case Direction.Down:
                return 1;
            case Direction.Right:
                return 2;
            case Direction.Left:
                return 3;
            case Direction.Up_Right:
                return 4;
            case Direction.Up_Left:
                return 5;
            case Direction.Down_Right:
                return 6;
            case Direction.Down_Left:
                return 7;
            default:
                Debug.LogError("Invalid Direction");
                return -1;
        }
    }
}
