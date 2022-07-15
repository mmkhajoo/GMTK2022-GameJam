using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using DG.Tweening;
using Managers.Audio_Manager;
using UnityEngine;
using UnityEngine.Events;

public class DashController : MonoBehaviour
{
    private PlayerMovement _playerMovement;
    private GravityController _gravityController;
    private ConstantForce2D _constantForce2D;


    private BoxCollider2D _boxCollider2D;
    private CircleCollider2D _circleCollider2D;
    private Rigidbody2D _rigidbody2D;

    [SerializeField] private float _dashDistance;
    [SerializeField] private float _dashTime;

    [SerializeField] private float _dashMinimumOffset = 0.5f;

    private Vector3 _startedPosition;
    private Vector3 _targetPosition;
    private IEnumerable<DirectionType> _directionTypes;


    private float _counter;
    private bool _dashClicked;

    [Header("Event")] [SerializeField] private UnityEvent _onDash;

    [SerializeField] private LayerMask _dashableMask; 
    
    [Header("Audio Source")] [SerializeField]
    private AudioSource _audioSource;


    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _gravityController = GetComponent<GravityController>();
        _constantForce2D = GetComponent<ConstantForce2D>();


        _boxCollider2D = GetComponent<BoxCollider2D>();
        _circleCollider2D = GetComponent<CircleCollider2D>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        
        _directionTypes = Enum.GetValues(typeof(DirectionType)).Cast<DirectionType>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Debug Multiplier") && !_dashClicked)
        {
            if (CanDash())
            {
                _counter = 0;
                Dash();
            }
        }

        if (_dashClicked)
        {
            _counter += Time.deltaTime;
        }

        if (_counter > _dashTime && _dashClicked)
        {
            _dashClicked = false;

            _boxCollider2D.isTrigger = false;
            _circleCollider2D.isTrigger = false;

            _constantForce2D.enabled = true;
        }

        Debug.DrawLine(_startedPosition, _targetPosition, Color.red);
    }

    private void Dash()
    {
        _constantForce2D.enabled = false;

        _boxCollider2D.isTrigger = true;
        _circleCollider2D.isTrigger = true;

        _rigidbody2D.DOMove(_targetPosition, _dashTime);

        _dashClicked = true;

        AudioManager.instance.PlaySoundEffect(_audioSource, AudioTypes.Dash);

        _onDash?.Invoke();
    }

    private bool CanDash()
    {
        var targetPosition = transform.position + _playerMovement.Direction * _dashDistance;

        RaycastHit2D dashableRaycastHit2D =
            Physics2D.Raycast(transform.position, _playerMovement.Direction, _dashDistance,_dashableMask);

        if (dashableRaycastHit2D.collider != null && !dashableRaycastHit2D.collider.CompareTag("Dashable") )
        {

            targetPosition = dashableRaycastHit2D.point - (Vector2)(_playerMovement.Direction * _boxCollider2D.size.x / 2f);
            
            
            // if (!dashableRaycastHit2D.collider.CompareTag("Dashable") && !dashableRaycastHit2D.collider.CompareTag("Win"))
            // {
            //     return false;
            // }
        }

        if (Vector2.Distance(transform.position, targetPosition) < _dashMinimumOffset)
            return false;
        
        // RaycastHit2D rightHit =
        //     Physics2D.Raycast(targetPosition + _playerMovement.Direction * _boxCollider2D.size.x / 2, Vector2.zero);
        // RaycastHit2D leftHit = Physics2D.Raycast(targetPosition - _playerMovement.Direction * _boxCollider2D.size.x / 2,
        //     Vector2.zero);
        //
        // if (rightHit.collider != null || leftHit.collider != null)
        // {
        //     return false;
        // }


        // foreach (var directionType in _directionTypes)
        // {
        //     if (!CheckDirectionWall(directionType, targetPosition))
        //         return false;
        // }

        _startedPosition = transform.position;
        _targetPosition = targetPosition;

        return true;
    }

    private bool CheckDirectionWall(DirectionType directionType, Vector3 targetPosition)
    {
        switch (directionType)
        {
            case DirectionType.Up:
                if (targetPosition.y > _gravityController.gorundDictionary[directionType].position.y)
                    return false;
                break;
            case DirectionType.Down:
                if (targetPosition.y < _gravityController.gorundDictionary[directionType].position.y)
                    return false;
                break;
            case DirectionType.Left:
                if (targetPosition.x < _gravityController.gorundDictionary[directionType].position.x)
                    return false;
                break;
            case DirectionType.Right:
                if (targetPosition.x > _gravityController.gorundDictionary[directionType].position.x)
                    return false;
                break;
        }

        return true;
    }
}