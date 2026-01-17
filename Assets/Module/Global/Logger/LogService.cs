using UnityEngine;

public class LogService : GenericSubsystem
{
    private readonly int logLevel;

    public LogService(SystemRegistry systemRegistry, int logLevel) : base(systemRegistry)
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

