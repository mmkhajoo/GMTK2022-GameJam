using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISlot 
{
    Transform MaxPos { get; }

    Transform MinPos { get; }

    DirectionType DirectionType { get; }

    void SetScrollSlot(bool isScroll);

    void OnPlaceDragItem(DragItem dragItem, Vector3 dragPosition);

    Vector3 GetCurrentPositionInSlot(Vector3 inputPosition);
}
