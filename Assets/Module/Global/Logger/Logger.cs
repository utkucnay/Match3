using UnityEngine;

public class Logger : ISystem
{
    private readonly int logLevel;

    public Logger(int logLevel)
    {
        this.logLevel = logLevel;
    }

    public void Log(string log, int logLevel = 5)
    {
        if (this.logLevel <= logLevel)
        {
            Debug.Log(log);
        }
    }

    public void LogWarning(string log, int logLevel = 5)
    {
        if (this.logLevel <= logLevel)
        {
            Debug.LogWarning(log);
        }
    }

    public void LogError(string log)
    {
        Debug.LogError(log);
    }

    public void LogAssertion(object log)
    {
        Debug.LogAssertion(log);
    }
}
