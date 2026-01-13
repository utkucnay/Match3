using UnityEngine;

public class GlobalSystemRegistry : SystemRegistry
{
    public static GlobalSystemRegistry Instance { get; private set; }
    
    [SerializeField] private int _logLevel;

    private LogService logger;

    protected override void Awake()
    {
        base.Awake();

        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;

            logger = new LogService(_logLevel);
            RegisterSystem(logger);

            return;
        }

        DestroyImmediate(this);
    }
}

