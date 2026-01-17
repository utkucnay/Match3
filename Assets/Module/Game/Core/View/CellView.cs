using Unity.Mathematics;
using UnityEngine;

public class CellView : MonoBehaviour
{
    public void Initialize(BoardCell cell, float2 position)
    {
        transform.localPosition = new Vector3(position.x, position.y, 0);
    }
}
