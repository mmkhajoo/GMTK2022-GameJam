using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDragManager 
{
    DragItem SelectedObject { get; }

    Action<IDragable,ISlot, bool> OnRelease { get; }

    Action<bool> OnDrag { get; }

    void RegisterCallBack(Action<bool> onDrag, Action<IDragable, ISlot, bool> onRelease);
}
