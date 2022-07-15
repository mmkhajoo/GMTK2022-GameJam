using Items;
using Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetItem : Item
{
    [Header("Detect Variables")]
    [SerializeField]
    private float _distanceDetection;
    [SerializeField]
    private float _detectionSizeMulti;
    [SerializeField]
    private float _disableSizeMulti;

    [Header("Magnet Stats")]
    [SerializeField]
    private float _powerMagnet;

    private Rigidbody2D objectRigidBoody;
    private BoxCollider2D _collider;
    private Vector2 _directionDetect;
    private ObjectController _objectPosses;


    public override void Initialize()
    {
        base.Initialize();

        _collider = GetComponent<BoxCollider2D>();
    }

    public override void Execute()
    {
        RaycastHit2D[] hits = Detection(_detectionSizeMulti, Color.green);

        foreach (var hit in hits)
        {
            if (hit.transform.TryGetComponent<ObjectController>(out ObjectController objectPosses))
            {
                if (objectPosses.ObjectType == ObjectType.Metal)
                {
                    if (objectPosses.IsEnable)
                    {
                        _objectPosses = objectPosses;

                        _objectPosses.Rigidbody2D.gravityScale = 0f;
                    }
                    else
                    {
                        DisableObject();
                    }
                }
            }
        }
    }

    public override void Disable()
    {
        _isScrollDrag = false;
        CurrentSlot?.SetScrollSlot(false);
        DisableObject();
        base.Disable();
    }

    public override void Active()
    {
        base.Active();
        _isScrollDrag = true;
        CurrentSlot.SetScrollSlot(true);

    }
    public override void DeActive()
    {
        base.DeActive();
        _isScrollDrag = false;
        CurrentSlot.SetScrollSlot(false);
        DisableObject();

    }

    private RaycastHit2D[] Detection(float sizeMulti, Color color)
    {
        _directionDetect = Vector2.zero;

        switch (DirectionType)
        {
            case DirectionType.None:
                break;
            case DirectionType.Up:
                _directionDetect = Vector2.down;
                break;
            case DirectionType.Down:
                _directionDetect = Vector2.up;
                break;
            case DirectionType.Left:
                _directionDetect = Vector2.right;
                break;
            case DirectionType.Right:
                _directionDetect = Vector2.left;
                break;
            default:
                break;
        }
        if (DirectionType == DirectionType.Down || DirectionType == DirectionType.Up)
        {
            Debug.DrawRay(_collider.bounds.center + new Vector3(_collider.bounds.extents.x * sizeMulti, 0), _directionDetect * (_collider.bounds.extents.y + _distanceDetection), color);
            Debug.DrawRay(_collider.bounds.center - new Vector3(_collider.bounds.extents.x * sizeMulti, 0), _directionDetect * (_collider.bounds.extents.y + _distanceDetection), color);
        }
        else
        {
            Debug.DrawRay(_collider.bounds.center + new Vector3(0, _collider.bounds.extents.y * sizeMulti), _directionDetect * (_collider.bounds.extents.x + _distanceDetection), color);
            Debug.DrawRay(_collider.bounds.center - new Vector3(0, _collider.bounds.extents.y * sizeMulti), _directionDetect * (_collider.bounds.extents.x + _distanceDetection), color);
        }
        return Physics2D.BoxCastAll(_collider.bounds.center, _collider.bounds.size * sizeMulti, 0f, _directionDetect, _distanceDetection);
    }

    protected override void Update()
    {
        base.Update();

        if (IsActive)
            Execute();

        if (_objectPosses != null)
        {
            RaycastHit2D[] hits = Detection(_disableSizeMulti, Color.red);
            bool isInRange = false;

            foreach (var hit in hits)
            {
                if(hit.transform.gameObject == _objectPosses.gameObject)
                {
                    isInRange = true;
                    break;
                }
            }

            if (!isInRange)
            {
                DisableObject();
                return;
            }

            Vector3 directionPull = transform.position - _objectPosses.Transform.position;

            _objectPosses.Rigidbody2D.AddForce(directionPull * Time.deltaTime * _powerMagnet);
        }
    }

    private void DisableObject()
    {
        if(_objectPosses!= null)
        {
            _objectPosses.Rigidbody2D.gravityScale = 1f;
            _objectPosses = null;
        }
    }

}
