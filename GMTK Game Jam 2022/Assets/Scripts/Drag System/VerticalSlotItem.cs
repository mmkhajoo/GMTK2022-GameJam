using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class VerticalSlotItem : SlotItem
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

    #endregion

    #region Public Methods

    public override Vector3 GetCurrentPositionInSlot(Vector3 inputPosition)
    {
        Vector2 setPosition = Vector2.zero;

        if (inputPosition.y > MaxPos.position.y)
            setPosition = new Vector2(transform.position.x, MaxPos.position.y);

        else if (inputPosition.y < MinPos.position.y)
            setPosition = new Vector2(transform.position.x, MinPos.position.y);

        else
            setPosition = new Vector2(transform.position.x, inputPosition.y);

        return setPosition;
    }


    public override void OnPlaceDragItem(DragItem dragItem, Vector3 dragPosition)
    {

        Vector2 setPosition = Vector2.zero;

        if (dragPosition.y > MaxPos.position.y)
            setPosition = new Vector2(transform.position.x, MaxPos.position.y);

        else if (dragPosition.y < MinPos.position.y)
            setPosition = new Vector2(transform.position.x, MinPos.position.y);

        else
            setPosition = new Vector2(transform.position.x, dragPosition.y);

        dragItem.transform.position = Vector2.Lerp(dragItem.transform.position, setPosition, 1f);

        dragItem.OnValidPlacement(DirectionType, this);
    }

    #endregion
}
