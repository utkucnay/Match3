using System;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

public class Level : ISystem
{
    public event Action OnLevelStartEvent;
    public event Action OnLevelUpdateEvent;
    public event Action OnLevelLateUpdateEvent;
    public event Action<bool> OnLevelEndEvent;

    private Board board;
    private readonly ILevelDataProvider levelDataProvider;
    //private Goal goal;
    private int moveCount;
    private bool isBoardInteractable;

    private LogService logger;
    private Random random;

    public Board Board => board;

    public Level(ILevelDataProvider levelDataProvider = null)
    {
        logger = GlobalSystemRegistry.Instance.GetSystem<LogService>();
        this.levelDataProvider = levelDataProvider ?? new TextFileLevelDataProvider();
        isBoardInteractable = false;
        random = new Random(1);
    }

    ~Level()
    {
        logger.Log("LevelData Instance Destroyed");
    }

    public void Initialize()
    {
        LoadLevel("level_01");
    }

    public void LoadLevel(string levelName)
    {
        LevelData levelData = levelDataProvider.LoadLevel(levelName);

        BoardConfig boardConfig = new BoardConfig
        {
            width = levelData.Width,
            height = levelData.Height
        };

        board = new Board(boardConfig);
        board.Initialize(levelData, ref random);
        moveCount = levelData.Moves;
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

        //var boardHistory = board.OnBoardUpdate(dragStart, direction, ref random);
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
