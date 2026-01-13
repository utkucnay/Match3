using Unity.Mathematics;

public struct ProgressCounter
{
    public int id;
    public int value;
    public int maxValue;

    public ProgressCounter(int id, int maxValue)
    {
        value = 0;
        this.id = id;
        this.maxValue = maxValue;
    }

    public void IncreaseProgress(int progressAmount)
    {
        value = math.clamp(value + progressAmount, 0, maxValue);
    }

    public bool IsMax()
    {
        return maxValue == value;
    }
}
