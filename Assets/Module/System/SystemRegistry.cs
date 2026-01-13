using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class SystemRegistry : MonoBehaviour
{
    protected Dictionary<Type, ISystem> _systems;

    protected virtual void Awake()
    {
        _systems = new Dictionary<Type, ISystem>();
    }

    public void RegisterSystem<T>(T system) where T : class, ISystem
    {
        _systems.Add(system.GetType(), system);
    }

    public void UnregisterSystem<T>(T system) where T : class, ISystem
    {
        _systems.Remove(system.GetType());
    }

    public T GetSystem<T>() where T : class, ISystem
    {
        if (_systems.TryGetValue(typeof(T), out ISystem system))
        {
            return system as T;
        }

        return null;
    }
}

