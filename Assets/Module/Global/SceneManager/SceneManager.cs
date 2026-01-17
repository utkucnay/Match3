using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneManager : GenericSubsystem, IDisposable
{
    private const string InitialSceneAddress = "2-MainScene";
    private const string GameplaySceneAddress = "3-GameScene";

    private AsyncOperationHandle<SceneInstance> loadHandle;

    public SceneManager(SystemRegistry systemRegistry) : base(systemRegistry)
    {
        
    }

    public void LoadScene()
    {
        _systemRegistry.StartCoroutine(LoadSceneCoroutine());
    }
    
    public IEnumerator LoadSceneCoroutine()
    {
        loadHandle = Addressables.LoadSceneAsync(GameplaySceneAddress, LoadSceneMode.Single, false);
        yield return loadHandle;
        yield return loadHandle.Result.ActivateAsync();
    }

    public void Dispose()
    {
        Addressables.UnloadSceneAsync(loadHandle);
    }
}
