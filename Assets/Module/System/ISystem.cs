using UnityEngine;

public interface ISubsystem
{
}

public abstract class GenericSubsystem : ISubsystem
{
    protected SystemRegistry _systemRegistry;
    protected EventBus _eventBus;

    public GenericSubsystem(SystemRegistry systemRegistry)
    {
        _systemRegistry = systemRegistry;
        _eventBus = systemRegistry.GetSystem<EventBus>();
    }
}

public abstract class SubsystemMono : MonoBehaviour, ISubsystem
{
    protected SystemRegistry _systemRegistry;
    protected EventBus _eventBus;

    protected virtual void Initialize(SystemRegistry systemRegistry)
    {
        _systemRegistry = systemRegistry;
        _eventBus = systemRegistry.GetSystem<EventBus>();
    }
}
