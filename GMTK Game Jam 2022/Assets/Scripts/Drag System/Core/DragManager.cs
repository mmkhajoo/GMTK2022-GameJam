using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class DragManager : MonoBehaviour, IDragManager
{
    #region Fields

    [SerializeField]
    private List<SlotItem> _slots;
    [SerializeField]
    private List<DragItem> _dragItems;

    [SerializeField] private UnityEvent onDragEvent;
    [SerializeField] private UnityEvent onDragRelease;
    
    private Action<IDragable, ISlot,bool> _onRelease;
    private Action<bool> _onDrag;
    private DragItem _selectedDragble;
    private Rigidbody2D _selectedObjectRb;
    private Vector3 offset;
    private Vector3 mousePosition;

    #endregion

    #region Properties

    public Vector3 DragPosition => GetDragPosition();
    public Action<IDragable, ISlot,bool> OnRelease => _onRelease;
    public Action<bool> OnDrag => _onDrag;
    public DragItem SelectedObject => _selectedDragble;

    #endregion

    #region Public Methods

    public void Initialize()
    {
        foreach (var slot in _slots)
        {
            if(slot.DirectionType == DirectionType.Up)
            {
                foreach (var drag in _dragItems)
                {
                    slot.OnPlaceDragItem(drag, drag.transform.position);
                }
            }
        }

        RegisterCallBack(((isScrollDrag) =>
        {
            if(!isScrollDrag)
                onDragEvent.Invoke();
        }), (dragable, slot,isScrollDrag) =>
        {
            if(!isScrollDrag)
                onDragRelease.Invoke();
        });
    }

    public void RegisterCallBack(Action<bool> onDrag, Action<IDragable, ISlot,bool> onRelease)
    {
        _onDrag += onDrag;
        _onRelease += onRelease;
    }

    #endregion

    #region Private Methods

    private Vector3 GetDragPosition()
    {
        if (_selectedDragble.IsScrollDrag)
        {
            return _selectedDragble.GetScrollPosition(mousePosition);
        }

        return mousePosition + offset;
    }

    private void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    private void Update()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            Collider2D[] targetObject = Physics2D.OverlapPointAll(mousePosition);
            foreach (var item in targetObject)
            {
                if (item.TryGetComponent<IDragable>(out IDragable drag))
                {

                    if (!drag.CanDrag)
                        return;

                    _selectedDragble = (DragItem)drag;

                    OnDrag?.Invoke(_selectedDragble.IsScrollDrag);

                    _selectedObjectRb = item.transform.gameObject.GetComponent<Rigidbody2D>();
                    offset = _selectedObjectRb.transform.position - mousePosition;
                }
            }
        }

        if (Input.GetMouseButtonUp(0) && _selectedObjectRb)
        {
            if(_selectedDragble.IsScrollDrag)
            {
                _selectedDragble.CurrentSlot.OnPlaceDragItem((DragItem)SelectedObject, DragPosition);
                _selectedObjectRb = null;
                _selectedDragble = null;
                OnRelease?.Invoke(SelectedObject, null,true);
                return;
            }

            Collider2D[] targetObject = Physics2D.OverlapPointAll(mousePosition);

            foreach (var item in targetObject)
            {
                if (item.TryGetComponent<ISlot>(out ISlot slot))
                {
                    slot.OnPlaceDragItem((DragItem)SelectedObject, DragPosition);

                    OnRelease?.Invoke(SelectedObject, slot,false);

                    _selectedObjectRb = null;
                    _selectedDragble = null;

                    return;
                }
            }

            _selectedDragble.OnInvalidePlacement();
            
            OnRelease?.Invoke(SelectedObject, null,false);
            
            _selectedObjectRb = null;
            _selectedDragble = null;

        }

    }

    private void FixedUpdate()
    {
        if (_selectedObjectRb)
        {
            _selectedObjectRb.MovePosition(DragPosition);
        }
    }

    #endregion
}
