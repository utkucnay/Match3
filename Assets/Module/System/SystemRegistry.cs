using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class SystemRegistry : MonoBehaviour
{
    protected Dictionary<Type, SystemRegistry> _registries;
    protected Dictionary<Type, ISubsystem> _systems;
    protected List<IInitializable> _initializables;
    protected List<IDisposable> _disposables;

    protected virtual void Awake()
    {
        _registries = new Dictionary<Type, SystemRegistry>();
        _systems = new Dictionary<Type, ISubsystem>();
        _initializables = new List<IInitializable>();
        _disposables = new List<IDisposable>();
    }

    public void RegisterSystem<T>(T system) where T : ISubsystem
    {
        _systems.Add(system.GetType(), system);
        if (system is IDisposable disposable)
        {
            _disposables.Add(disposable);
        }
        if (system is IInitializable initializable)
        {
            _initializables.Add(initializable);
        }
    }

    public void UnregisterSystem<T>(T system) where T : ISubsystem
    {
        _systems.Remove(system.GetType());
        if (system is IDisposable disposable)
        {
            _disposables.Remove(disposable);
        }
        if (system is IInitializable initializable)
        {
            _initializables.Remove(initializable);
        }
    }

    public T GetSystem<T>() where T : class, ISubsystem
    {
        if (_systems.TryGetValue(typeof(T), out ISubsystem system))
        {
            return system as T;
        }

        foreach (var registry in _registries.Values)
        {
            var foundSystem = registry.GetSystem<T>();
            if (foundSystem != null)
            {
                return foundSystem;
            }
        }

        return null;
    }

    protected virtual void Start()
    {
        foreach (var initializable in _initializables)
        {
            initializable.Initialize();
        }
    }

    protected virtual void OnDisable()
    {
        foreach (var disposable in _disposables)
        {
            disposable?.Dispose();
        }
    }
}
