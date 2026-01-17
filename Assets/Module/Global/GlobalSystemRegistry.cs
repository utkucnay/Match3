using System.Collections;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class GlobalSystemRegistry : SystemRegistry
{
    public static GlobalSystemRegistry Instance { get; private set; }
    
    [SerializeField] private int _logLevel;

    private LogService _logger;
    private SceneManager _sceneManager;
    private AddressableLoader _addressableLoader;

    protected override void Awake()
    {
        base.Awake();

        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;

            _logger = new LogService(this, _logLevel);
            RegisterSystem(_logger);

            _sceneManager = new SceneManager(this);
            RegisterSystem(_sceneManager);

            _addressableLoader = new AddressableLoader(this);   
            RegisterSystem(_addressableLoader);

            return;
        }

        DestroyImmediate(this);
    }

    protected IEnumerator Start()
    {
        yield return _addressableLoader.Initialize();
        yield return _sceneManager.LoadSceneCoroutine();
    }
}

