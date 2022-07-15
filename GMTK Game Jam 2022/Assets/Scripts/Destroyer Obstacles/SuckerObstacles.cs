using DefaultNamespace;
using Newtonsoft.Json.Serialization;
using Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SuckerObstacles : MonoBehaviour, IDestroyerObstacles
{
    [SerializeField]
    private DirectionType _directionType;

    [Header("Detection Variables")]
    [SerializeField]
    private float _distanceDetection;
    [SerializeField]
    private float _detectionsSizeMulti;
    [SerializeField]
    private Transform _targetTransform;
    [SerializeField]
    private Transform _transitionTransform;

    [Header("Sucker Stats")]
    [SerializeField]
    private float _suckingSpeed;
    [SerializeField]
    private float _transitionTime;
    [SerializeField]
    private UnityEvent suckerDisabled;
        
    private bool _isActive;
    private BoxCollider2D _collider;

    public bool IsActive => _isActive;
    public DirectionType DirectionType => _directionType;

    public void Initialize()
    {
        _collider = GetComponent<BoxCollider2D>();
        Active();
    }

    public void Active()
    {
        _isActive = true;
    }

    public void Deactive()
    {
        _isActive = false;
        suckerDisabled.Invoke();
    }

    public void Execute()
    {
        RaycastHit2D[] hits = Detection(_detectionsSizeMulti, Color.green);

        foreach (var hit in hits)
        {
            if(hit.transform.CompareTag("Transitionable"))
            {
                hit.rigidbody.velocity = Vector2.zero;

                if(Vector2.Distance(hit.transform.position, _targetTransform.position) == 0)
                {
                    hit.rigidbody.isKinematic = true;
                    Deactive();
                    return;
                }

                hit.transform.position = Vector3.MoveTowards(hit.transform.position, _targetTransform.position, Time.deltaTime * _suckingSpeed);
            }

            else if(hit.transform.CompareTag("Player"))
            {
                var player = hit.transform.GetComponent<Player>();
                player.Disable();

                if (Vector2.Distance(hit.transform.position, _targetTransform.position) < 1)
                {
                    Debug.Log("PALYER");
                    LeanTween.move(hit.transform.gameObject, _transitionTransform.transform, _transitionTime);
                    LeanTween.scale(hit.transform.gameObject, Vector3.zero, _transitionTime).setOnComplete(player.Die);
                    Deactive();
                    return;
                }

                hit.transform.position = Vector3.MoveTowards(hit.transform.position, _targetTransform.position, Time.deltaTime * _suckingSpeed);

            }
        }
    }

    private RaycastHit2D[] Detection(float sizeMulti, Color color)
    {
       var _directionDetect = Vector2.zero;

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

    private void Start()
    {
        Initialize();
    }

    public void Update()
    {
        if (_isActive)
            Execute();
    }
}
