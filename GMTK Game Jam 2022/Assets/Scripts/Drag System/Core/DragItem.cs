using System.Collections;
using System.Collections.Generic;
using Items;
using UnityEngine;

public class DragItem : MonoBehaviour, IDragable
{
    private const float _moveSpeed = 50f;

    private ISlot _currentSlot;
    protected bool _isScrollDrag;
    protected bool _canDrag;

    private bool _isReturnPosition;
    private Vector3 _returnPosition;
    private DirectionType _directionType;

    protected ItemSound _itemSound;
    
    public bool IsScrollDrag => _isScrollDrag;
    public bool CanDrag => _canDrag;
    public DirectionType DirectionType => _directionType;
    public ISlot CurrentSlot => _currentSlot;

    public virtual void Initialize()
    {
        _canDrag = true;
    }

    private void Start()
    {
        _returnPosition = transform.position;
        _itemSound = GetComponent<ItemSound>();
    }

    protected virtual void Update()
    {
        if (Vector3.Distance(_returnPosition, transform.position) == 0)
            _isReturnPosition = false;
        

        if (_isReturnPosition)
            transform.position = Vector3.MoveTowards(transform.position, _returnPosition, Time.deltaTime * _moveSpeed);
    }

    public void OnValidPlacement(DirectionType directionType, ISlot slot)
    {
        _returnPosition = transform.position;
        _currentSlot = slot;

        if(_directionType != directionType)
        {
            switch (directionType)
            {
                case DirectionType.Up:
                    transform.rotation = Quaternion.Euler(0,0,180);
                    _directionType = directionType;
                    break;
                case DirectionType.Down:
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                    _directionType = directionType;
                    break;
                case DirectionType.Left:
                    transform.rotation = Quaternion.Euler(0, 0, -90);
                    _directionType = directionType;
                    break;
                case DirectionType.Right:
                    transform.rotation = Quaternion.Euler(0, 0, 90);
                    _directionType = directionType;
                    break;
                default:
                    break;
            }
        }

    }

    public void OnInvalidePlacement()
    {
        _isReturnPosition = true;
    }

    public Vector3 GetScrollPosition(Vector3 mousePosition)
    {
        Vector3 scrollPosition = Vector3.zero;

        scrollPosition = CurrentSlot.GetCurrentPositionInSlot(mousePosition);

        return scrollPosition;

    }
}
