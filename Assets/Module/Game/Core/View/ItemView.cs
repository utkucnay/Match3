using UnityEngine;

public class ItemView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;

    AddressableLoader _addressableLoader;

    void Awake()
    {
        _addressableLoader = GlobalSystemRegistry.Instance.GetSystem<AddressableLoader>();
    }

    public void Initialize(in Item item)
    {
        gameObject.SetActive(false);
        
        if (item.itemType == ItemType.None)
        {
            return;
        }

        _addressableLoader.LoadAddressable<Sprite>(item.itemType.ToString(), sprite =>
        {
            _spriteRenderer.sprite = sprite;
        });
    }
}
