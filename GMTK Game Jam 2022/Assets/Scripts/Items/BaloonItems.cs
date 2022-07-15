using Items;
using Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaloonItems : Item
{
    [Header("Detect Variables")]
    [SerializeField]
    private float _distanceDetection;
    [SerializeField]
    private float _detectionSizeMulti;

    [Header("Baloon Stats")]
    [SerializeField]
    private float _powerBaloon;
    [SerializeField]
    private float _activeTime;

    private BoxCollider2D _collider;
    private Color _rayColor;
    private Vector2 _directionDetect;
    private float tmp_time;

    public override void Initialize()
    {
        base.Initialize();

        _collider = GetComponent<BoxCollider2D>();
        tmp_time = 0f;
    }

    public override void Execute()
    {
        RaycastHit2D[] hits = Detection(_detectionSizeMulti, Color.green);

        foreach (var hit in hits)
        {
            if (hit.transform.TryGetComponent<ObjectController>(out ObjectController objectPosses))
            {
                if (objectPosses.ObjectType == ObjectType.Forceable  && objectPosses.IsEnable )
                {
                    Vector2 _directionPush = objectPosses.transform.position - transform.position;

                    objectPosses.Rigidbody2D.AddForce(_directionPush * Time.deltaTime * _powerBaloon);
                }
            }
        }
    }
    public override void Active()
    {
        _canDrag = false;
        base.Active();
    }

    public override void DeActive()
    {
        _canDrag = true;
        base.DeActive();
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
        {
            tmp_time += Time.deltaTime;
            if(tmp_time > _activeTime)
            {
                tmp_time = 0f;
                DeActive();
                return;
            }

            Execute();
        }
    }

}
