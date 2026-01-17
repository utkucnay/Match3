using UnityEngine;

public class CoreSystemRegistry : SystemRegistry
{
    [SerializeField] private BoardView boardView;

    private Level _level;
    private EventBus _eventBus;

    protected override void Awake()
    {
        base.Awake();

        _level = new Level(this, boardView);
        _eventBus = new EventBus();

        RegisterSystem(_level);
        RegisterSystem(_eventBus);
    }
}