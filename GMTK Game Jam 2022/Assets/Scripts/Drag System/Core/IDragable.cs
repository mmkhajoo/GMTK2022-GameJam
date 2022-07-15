using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDragable
{
    bool IsScrollDrag { get; }

    bool CanDrag { get; }

    ISlot CurrentSlot { get; }

    DirectionType DirectionType { get; }

    void OnValidPlacement(DirectionType slotTypes, ISlot slot);

    void OnInvalidePlacement();

    Vector3 GetScrollPosition(Vector3 mousePosition);
}
