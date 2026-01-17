using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.XR;

public class AddressableLoader : GenericSubsystem
{
    public Dictionary<string, UnityEngine.Object> _loadedAssets = new Dictionary<string, UnityEngine.Object>();

    public Dictionary<string, Queue<Action<UnityEngine.Object>>> _loadingQueues = new Dictionary<string, Queue<Action<UnityEngine.Object>>>();

    public AddressableLoader(SystemRegistry systemRegistry) : base(systemRegistry)
    {
    }

    public IEnumerator Initialize()
    {
        yield return Addressables.InitializeAsync();
    }

    public void LoadAddressable<T>(string address, Action<T> onLoaded) where T : UnityEngine.Object
    {
        if (_loadedAssets.TryGetValue(address, out var loadedAsset))
        {
            if (loadedAsset != null)
            {
                onLoaded?.Invoke(loadedAsset as T);
                return;
            }
        }

        GlobalSystemRegistry.Instance.StartCoroutine(LoadAddressableCoroutine<T>(address, onLoaded));
    }

    public void LoadAddressable<T>(AssetReference assetReference, Action<T> onLoaded) where T : UnityEngine.Object
    {
        if (_loadedAssets.TryGetValue(assetReference.RuntimeKey.ToString(), out var loadedAsset))
        {
            if (loadedAsset != null)
            {
                onLoaded?.Invoke(loadedAsset as T);
                return;
            }
        }

        GlobalSystemRegistry.Instance.StartCoroutine(LoadAddressableCoroutine<T>(assetReference, onLoaded));
    }

    public IEnumerator LoadAddressableCoroutine<T>(string address, Action<T> onLoaded) where T : UnityEngine.Object
    {
        if (_loadedAssets.TryGetValue(address, out var loadedAsset))
        {
            if (loadedAsset == null)
            {
                _loadingQueues[address].Enqueue(obj => onLoaded?.Invoke(obj as T));
                yield break;
            }
            else
            {
                onLoaded?.Invoke(loadedAsset as T);
                yield break;
            }
        }

        _loadedAssets.Add(address, null);
        _loadingQueues.Add(address, new Queue<Action<UnityEngine.Object>>());

        var handle = Addressables.LoadAssetAsync<T>(address);
        yield return handle;
        _loadedAssets[address] = handle.Result;
        onLoaded?.Invoke(handle.Result);

        var queue = _loadingQueues[address];
        while (queue.Count > 0)
        {
            var callback = queue.Dequeue();
            callback?.Invoke(handle.Result);
        }
    }

    public IEnumerator LoadAddressableCoroutine<T>(AssetReference assetReference, Action<T> onLoaded) where T : UnityEngine.Object
    {
        if (_loadedAssets.TryGetValue(assetReference.RuntimeKey.ToString(), out var loadedAsset))
        {
            if (loadedAsset == null)
            {
                _loadingQueues[assetReference.RuntimeKey.ToString()].Enqueue(obj => onLoaded?.Invoke(obj as T));
                yield break;
            }
            else
            {
                onLoaded?.Invoke(loadedAsset as T);
                yield break;
            }
        }

        _loadedAssets.Add(assetReference.RuntimeKey.ToString(), null);
        _loadingQueues.Add(assetReference.RuntimeKey.ToString(), new Queue<Action<UnityEngine.Object>>());

        var handle = assetReference.LoadAssetAsync<T>();
        yield return handle;
        _loadedAssets[assetReference.RuntimeKey.ToString()] = handle.Result;
        onLoaded?.Invoke(handle.Result);

        var queue = _loadingQueues[assetReference.RuntimeKey.ToString()];
        while (queue.Count > 0)
        {
            var callback = queue.Dequeue();
            callback?.Invoke(handle.Result);
        }
    }

    public void UnloadAddressable<T>(string address) where T : UnityEngine.Object
    {
        if (_loadedAssets.TryGetValue(address, out var handle))
        {
            if (_loadedAssets.Remove(address))
            {
                Addressables.Release(handle);
            }
        }
    }

    public void UnloadAddressable<T>(AssetReference assetReference) where T : UnityEngine.Object
    {
        var address = assetReference.RuntimeKey.ToString();
        if (_loadedAssets.TryGetValue(address, out var handle))
        {
            if (_loadedAssets.Remove(address))
            {
                Addressables.Release(handle);
            }
        }
    }
}
