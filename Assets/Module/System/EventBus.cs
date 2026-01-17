using System;
using System.Collections.Generic;

public sealed class EventBus : ISubsystem
{
    private readonly Dictionary<Type, Delegate> _table = new();


    public void Subscribe<T>(Action<T> listener)
    {
        if (listener == null) throw new ArgumentNullException(nameof(listener));

        var type = typeof(T);
        if (_table.TryGetValue(type, out var existing))
            _table[type] = (Action<T>)existing + listener;
        else
            _table[type] = listener;
    }

    public void Unsubscribe<T>(Action<T> listener)
    {
        if (listener == null) return;

        var type = typeof(T);
        if (!_table.TryGetValue(type, out var existing)) return;

        var updated = (Action<T>)existing - listener;
        if (updated == null) _table.Remove(type);
        else _table[type] = updated;
    }

    public void Publish<T>(T evt)
    {
        var type = typeof(T);
        if (_table.TryGetValue(type, out var existing))
            ((Action<T>)existing)?.Invoke(evt);
    }

    public void Clear()
    {
        _table.Clear();
    }
}