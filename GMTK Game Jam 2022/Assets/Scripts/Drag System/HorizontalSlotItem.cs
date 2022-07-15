using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalSlotItem : SlotItem
{
    #region Fields

    [SerializeField]
    private DirectionType _directionType;

    [Header("Position Offset")]
    [SerializeField]
    private Transform _maxPos;
    [SerializeField]
    private Transform _minPos;

    #endregion

    #region Properties

    public override DirectionType DirectionType => _directionType;

    public override Transform MaxPos => _maxPos;

    public override Transform MinPos => _minPos;

    public override Vector3 GetCurrentPositionInSlot(Vector3 inputPosition)
    {
        Vector2 setPosition = Vector2.zero;

        if (inputPosition.x > MaxPos.position.x)
            setPosition = new Vector2(MaxPos.position.x, transform.position.y);

        else if (inputPosition.x < MinPos.position.x)
            setPosition = new Vector2(MinPos.position.x, transform.position.y);

        else
            setPosition = new Vector2(inputPosition.x, transform.position.y);

        return setPosition;
    }

    #endregion

    #region Public Methods

    public override void OnPlaceDragItem(DragItem dragItem, Vector3 dragPosition)
    {
        Vector2 setPosition = Vector2.zero;

        if (dragPosition.x > MaxPos.position.x)
            setPosition = new Vector2(MaxPos.position.x, transform.position.y);

        else if (dragPosition.x < MinPos.position.x)
            setPosition = new Vector2(MinPos.position.x, transform.position.y);

        else
            setPosition = new Vector2(dragPosition.x, transform.position.y);

        dragItem.transform.position = Vector2.Lerp(dragItem.transform.position, setPosition, 1f);

        dragItem.OnValidPlacement(DirectionType, this);
    }

    #endregion
}
