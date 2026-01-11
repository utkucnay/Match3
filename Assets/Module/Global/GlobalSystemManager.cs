using UnityEngine;

public class GlobalSystemManager : SystemManager
{
    public static GlobalSystemManager Instance { get; private set; }
    
    [SerializeField] private int _logLevel;

    Logger logger;

    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;

            logger = new Logger(_logLevel);
            RegisterSystem(logger);

            return;
        }

        DestroyImmediate(this);
    }
}
