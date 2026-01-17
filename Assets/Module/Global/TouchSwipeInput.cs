using System;
using System.Collections.Generic;
using UnityEngine;

public enum SwipeDirection
{
    Up,
    Down,
    Left,
    Right
}

public struct SwipeEvent
{
    public int TargetId;
    public RectTransform TargetRect;
    public Vector2 StartPosition;
    public Vector2 EndPosition;
    public Vector2 Delta;
    public SwipeDirection Direction;
}

public class TouchSwipeInput : SubsystemMono
{
    [SerializeField] private Camera _uiCamera;
    [SerializeField] private float _swipeThreshold = 20f;
    [SerializeField] private bool _useMouseInEditor = true;

    private struct TargetRegistration
    {
        public int id;
        public RectTransform rect;
        public Action<SwipeEvent> onSwipe;
    }

    private readonly List<TargetRegistration> _targets = new List<TargetRegistration>();
    private bool _pointerDown;
    private Vector2 _pointerStart;
    private TargetRegistration? _activeTarget;

    private void Awake()
    {
        if (_uiCamera == null)
        {
            _uiCamera = Camera.main;
        }
    }

    private void Update()
    {
        if (TryGetPointerDown(out var downPos))
        {
            _pointerDown = true;
            _pointerStart = downPos;
            _activeTarget = FindTargetAt(downPos);
        }

        if (_pointerDown && TryGetPointerUp(out var upPos))
        {
            if (_activeTarget.HasValue)
            {
                TryFireSwipe(upPos, _activeTarget.Value);
            }

            _pointerDown = false;
            _activeTarget = null;
        }
    }

    public void RegisterTarget(int id, RectTransform rect, Action<SwipeEvent> onSwipe)
    {
        if (rect == null)
        {
            return;
        }

        for (int i = 0; i < _targets.Count; i++)
        {
            if (_targets[i].id == id)
            {
                _targets[i] = new TargetRegistration
                {
                    id = id,
                    rect = rect,
                    onSwipe = onSwipe
                };
                return;
            }
        }

        _targets.Add(new TargetRegistration
        {
            id = id,
            rect = rect,
            onSwipe = onSwipe
        });
    }

    public void UnregisterTarget(int id)
    {
        for (int i = _targets.Count - 1; i >= 0; i--)
        {
            if (_targets[i].id == id)
            {
                _targets.RemoveAt(i);
                return;
            }
        }
    }

    public void ClearTargets()
    {
        _targets.Clear();
    }

    private TargetRegistration? FindTargetAt(Vector2 screenPos)
    {
        for (int i = _targets.Count - 1; i >= 0; i--)
        {
            var target = _targets[i];
            if (target.rect == null || !target.rect.gameObject.activeInHierarchy)
            {
                continue;
            }

            if (RectTransformUtility.RectangleContainsScreenPoint(target.rect, screenPos, _uiCamera))
            {
                return target;
            }
        }

        return null;
    }

    private void TryFireSwipe(Vector2 endPos, TargetRegistration target)
    {
        Vector2 delta = endPos - _pointerStart;
        if (_swipeThreshold > 0f && delta.magnitude < _swipeThreshold)
        {
            return;
        }

        var direction = GetSwipeDirection(delta);
        target.onSwipe?.Invoke(new SwipeEvent
        {
            TargetId = target.id,
            TargetRect = target.rect,
            StartPosition = _pointerStart,
            EndPosition = endPos,
            Delta = delta,
            Direction = direction
        });
    }

    private static SwipeDirection GetSwipeDirection(Vector2 delta)
    {
        if (Mathf.Abs(delta.x) >= Mathf.Abs(delta.y))
        {
            return delta.x >= 0f ? SwipeDirection.Right : SwipeDirection.Left;
        }

        return delta.y >= 0f ? SwipeDirection.Up : SwipeDirection.Down;
    }

    private bool TryGetPointerDown(out Vector2 screenPos)
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                screenPos = touch.position;
                return true;
            }
        }

        if (_useMouseInEditor && Input.GetMouseButtonDown(0))
        {
            screenPos = Input.mousePosition;
            return true;
        }

        screenPos = default;
        return false;
    }

    private bool TryGetPointerUp(out Vector2 screenPos)
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                screenPos = touch.position;
                return true;
            }
        }

        if (_useMouseInEditor && Input.GetMouseButtonUp(0))
        {
            screenPos = Input.mousePosition;
            return true;
        }

        screenPos = default;
        return false;
    }
}
