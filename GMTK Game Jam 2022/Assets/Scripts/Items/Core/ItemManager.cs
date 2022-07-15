using Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ItemManager : MonoBehaviour
{
    [SerializeField]
    private List<Item> _items;
    private Item _currentItem;
    private bool _canActiveItem;

    public UnityEvent OnSwitchItem;

    public void Initialize()
    {
        _canActiveItem = true;

        foreach (var item in _items)
        {
            item.Initialize();
            item.Disable();
        }
        _items[0].Enable();
        _currentItem = _items[0];
    }

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            _canActiveItem = false;

        if (Input.GetMouseButtonUp(0))
            _canActiveItem = true;

        if (!_canActiveItem)
            return;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!_currentItem.IsActive)
            {
                EnableNextItem();
                OnSwitchItem?.Invoke();
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            ActiveItem();
        }


    }

    private void ActiveItem()
    {
        if (!_currentItem.IsActive)
            _currentItem.Active();

        else
            _currentItem.DeActive();
    }

    private void EnableNextItem()
    {
        bool nextActive = false;
        Vector3 _nextItemPosition = Vector3.zero;
        ISlot _nextSlotSet = null;

        for (int i = 0; i < _items.Count; i++)
        {
            if (nextActive)
            {
                _items[i].Enable();
                _nextSlotSet.OnPlaceDragItem(_items[i], _nextItemPosition);
                _currentItem = _items[i];
                return;
            }

            if (_items[i].IsEnable)
            {
                _nextItemPosition = _items[i].transform.position;
                _nextSlotSet = _items[i].CurrentSlot;

                _items[i].Disable();
                nextActive = true;
            }
        }

        _items[0].Enable();
        _nextSlotSet.OnPlaceDragItem(_items[0], _nextItemPosition);
        _currentItem = _items[0];

    }
}
