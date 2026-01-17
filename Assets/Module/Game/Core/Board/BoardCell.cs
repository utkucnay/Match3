
public struct BoardCell
{
    public int itemIndex;

    public int cellViewIndex;

    public int upCellIndex;
    public int downCellIndex;
    public int rightCellIndex;
    public int leftCellIndex;

    public int GetDirectionIndex(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return upCellIndex;
            case Direction.Down:
                return downCellIndex;
            case Direction.Right:
                return rightCellIndex;
            case Direction.Left:
                return leftCellIndex;
        }

        return -1;
    }
}

