using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SlotItem : MonoBehaviour, ISlot
{
    [SerializeField]
    protected Color _activeSlotColor;

    private Color _firstColor;
    private SpriteRenderer _spriteRenderer;

    public abstract Transform MaxPos { get; }
    public abstract Transform MinPos { get; }

    public abstract DirectionType DirectionType { get; }
    public abstract void OnPlaceDragItem(DragItem dragItem, Vector3 dragPosition);
    public abstract Vector3 GetCurrentPositionInSlot(Vector3 inputPosition);

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _firstColor = _spriteRenderer.color;
    }

    public void SetScrollSlot(bool isScroll)
    {
        if (isScroll)
            _spriteRenderer.color = _activeSlotColor;
        else
            _spriteRenderer.color = _firstColor;
    }
}
