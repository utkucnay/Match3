using System;
using Unity.Mathematics;

public class LevelData : ISystem
{
    public event Action OnLevelStartEvent;
    public event Action OnLevelUpdateEvent;
    public event Action OnLevelLateUpdateEvent;
    public event Action<bool> OnLevelEndEvent;

    Board board;
    //Goal goal;
    int moveCount;

    bool isBoardInteractable;

    LogService logger;

    public LevelData()
    {
        logger = GlobalSystemRegistry.Instance.GetSystem<LogService>();
        isBoardInteractable = false;
    }

    ~LevelData()
    {
        logger.Log("LevelData Instance Destroyed");
    }

    public void Initialize()
    {
        
    }

    public void OnLevelStart()
    {
        isBoardInteractable = true;

        OnLevelStartEvent?.Invoke();
    }

    public void OnLevelUpdate(int2 dragStart, Direction direction)
    {
        logger.Log($"LevelData Update From {dragStart}, With Direction {direction}", 1);

        if (!isBoardInteractable)
        {
            return;
        }

        //var boardHistory = board.OnBoardUpdate(dragStart, direction);
        //goal.Update(boardHistory);

        moveCount--;

        OnLevelUpdateEvent?.Invoke();
        OnLevelLateUpdate();
    }

    public void OnLevelLateUpdate()
    {
        //if (goal.IsReach())
        {
            //OnLevelEnd(true);
            return;
        }

        if (moveCount <= 0)
        {
            OnLevelEnd(false);
            return;
        }


        OnLevelLateUpdateEvent?.Invoke();
    }

    public void OnLevelEnd(bool isWin)
    {
        if (isWin)
        {
            
        }
        else
        {
            
        }

        OnLevelEndEvent?.Invoke(isWin);
    }
}

