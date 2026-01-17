using System.Collections;
using UnityEngine;
using PrimeTween;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Unity.Mathematics;

public class BoardView : MonoBehaviour
{
    [SerializeField] private AssetReferenceGameObject _CellViewPrefabRef;
    [SerializeField] private AssetReferenceGameObject _itemViewPrefabRef;

    [SerializeField] private Transform _cellViewContainer;
    [SerializeField] private Transform _itemViewContainer;

    [SerializeField] private float2 itemViewSize = new float2(20, 20);
    
    private AddressableLoader _addressableLoader;

    private CellView[] _cellViews;
    private ItemView[] _itemViews;
    
    private Board _board;

    private EventBus _eventBus;

    private Coroutine _currentAnimationCoroutine;

    private bool isInitialized = false;

    private IEnumerator Start()
    {
        _addressableLoader = GlobalSystemRegistry.Instance.GetSystem<AddressableLoader>();

        GameObject cellViewPrefab = null;
        yield return _addressableLoader.LoadAddressableCoroutine<GameObject>(_CellViewPrefabRef, prefab =>
        {
            cellViewPrefab = prefab;
        });

        GameObject itemViewPrefab = null;
        yield return _addressableLoader.LoadAddressableCoroutine<GameObject>(_itemViewPrefabRef, prefab =>
        {
            itemViewPrefab = prefab;
        });

        yield return new WaitUntil(() => isInitialized);

        _cellViews = new CellView[_board.Cells.Length];
        SetCellViews(cellViewPrefab);
        
        _itemViews = new ItemView[_board.Items.Length];
        SetItemViews(itemViewPrefab);
    }

    private void OnEnable()
    {
        _eventBus?.Subscribe<BoardUpdateEvent>(OnBoardUpdateEvent);  
    }

    private void OnDisable()
    {
        _eventBus.Unsubscribe<BoardUpdateEvent>(OnBoardUpdateEvent);
    }

    private void OnDestroy()
    {
        _addressableLoader.UnloadAddressable<GameObject>(_CellViewPrefabRef);
        _addressableLoader.UnloadAddressable<GameObject>(_itemViewPrefabRef);
    }

    public void Initialize(SystemRegistry systemRegistry, Board board)
    {
        _board = board;
        _eventBus = systemRegistry.GetSystem<EventBus>();
        _eventBus.Subscribe<BoardUpdateEvent>(OnBoardUpdateEvent);        
        isInitialized = true;
    }

    public void OnBoardUpdateEvent(BoardUpdateEvent boardUpdateEvent)
    {
        BoardViewUpdate(in boardUpdateEvent.boardUpdateResult);
    }

    public void SetCellViews(GameObject cellViewPrefab)
    {
        float2 beginPos = new float2(-(_board.BoardData.width / 2f) * itemViewSize.x + itemViewSize.x / 2f,
            -(_board.BoardData.height / 2f) * itemViewSize.y + itemViewSize.y / 2f);

        for (int i = 0; i < _board.Cells.Length; i++)
        {
            var cellViewObj = Instantiate(cellViewPrefab, _cellViewContainer);
            _cellViews[i] = cellViewObj.GetComponent<CellView>();
            _cellViews[i].Initialize(_board.Cells[i], beginPos + new float2(
                (i % _board.BoardData.width) * itemViewSize.x,
                (i / _board.BoardData.width) * itemViewSize.y));
            _board.Cells[i].cellViewIndex = i;
        }
    }

    public void SetItemViews(GameObject itemViewPrefab)
    {
        for (int i = 0; i < _board.Items.Length; i++)
        {
            var itemViewObj = Instantiate(itemViewPrefab, _itemViewContainer);
            _itemViews[i] = itemViewObj.GetComponent<ItemView>();
            _itemViews[i].Initialize(in _board.Items[i]);
            _board.Items[i].itemViewIndex = i;
        }

        for (int i = 0; i < _board.Cells.Length; i++)
        {
            var itemViewIndex = _board.Items[_board.Cells[i].itemIndex].itemViewIndex;
            _itemViews[itemViewIndex].gameObject.SetActive(true);
            _itemViews[itemViewIndex].transform.position = _cellViews[i].transform.position;
        }
    }

    public void BoardViewUpdate(in BoardUpdateResult boardUpdateResult)
    {
        if (_currentAnimationCoroutine != null)
        {
            StopCoroutine(_currentAnimationCoroutine);
        }
        _currentAnimationCoroutine = StartCoroutine(PlayBoardAnimations(boardUpdateResult));
    }

    private IEnumerator PlayBoardAnimations(BoardUpdateResult boardUpdateResult)
    {
        yield return PlaySwapAnimation(boardUpdateResult.moveFirst);

        if (boardUpdateResult.blastedTileIndexes != null && boardUpdateResult.blastedTileIndexes.Length > 0)
        {
            //yield return PlayBlastAnimation(boardUpdateResult.blastedTileIndexes);
        }

        if (boardUpdateResult.isReturn)
        {
            yield return PlaySwapAnimation(boardUpdateResult.moveFirst);
        }
        else
        {
            //yield return PlayFallAnimation();
        }

        _currentAnimationCoroutine = null;
    }

    public IEnumerator PlayBlastAnimation(int[] blastedTileIndexes)
    {
        yield return null;
    }

    public IEnumerator PlayFallAnimation()
    {
        yield return null;
    }

    public IEnumerator PlaySpawnAnimation()
    {
        yield return null;
    }

    public IEnumerator PlaySwapAnimation(SwapMove swapMove)
    {
        var seq = Sequence.Create()
            .Group(Tween.Position(_itemViews[swapMove.itemFrom.itemViewIndex].transform, _cellViews[swapMove.cellTo.cellViewIndex].transform.position, 0.3f))
            .Group(Tween.Position(_itemViews[swapMove.itemTo.itemViewIndex].transform, _cellViews[swapMove.cellFrom.cellViewIndex].transform.position, 0.3f));

        yield return seq.ToYieldInstruction();
    }
}