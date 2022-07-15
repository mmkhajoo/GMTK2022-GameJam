using System;
using Managers;
using Managers.Audio_Manager;
using Objects;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace DefaultNamespace
{
    public class Player : MonoBehaviour, IPlayer
    {
        public Transform Transform => transform;

        [Header("Transition Configs")] [SerializeField]
        private float _searchObjectRadius = 1f;

        [SerializeField] private float _transitionTime = 0.5f;

        #region Fields

        private ObjectController _objectTransitionedTo;

        private PlayerStateType _currentPlayerStateType = PlayerStateType.None;

        #endregion

        #region Controllers

        private PlayerMovement _playerMovement;
        private CharacterController2D _characterController2D;
        private GravityController _gravityController;
        private DashController _dashController;
        private ConstantForce2D _constantForce2D;
        private BoxCollider2D _boxCollider2D;
        private CircleCollider2D _circleCollider2D;
        private Rigidbody2D _rigidbody2D;

        #endregion

        #region Events

        [Header("Events")] [SerializeField] private PlayerStateEvent _onPlayerStateChanged;
        [SerializeField] private UnityEvent OnPlayerLand;
        [SerializeField] private UnityEvent OnPlayerJumped;
        [SerializeField] private UnityEvent OnTransitioned;
        [SerializeField] private UnityEvent OnPlayerGotOut;


        [Header("Audio Source")] [SerializeField]
        private AudioSource _audioSource;

        [SerializeField] private AudioSource _transitionAudioSource;
        [SerializeField] private AudioSource _walkAudioSource;
        #endregion

        #region Private Properties

        private bool isPlayerMoving => _playerMovement.VerticalMove != 0f || _playerMovement.HorizontalMove != 0f;

        #endregion

        private bool _isTransitioning;

        private bool _collisionTriggered;

        private void Awake()
        {
            _playerMovement = GetComponent<PlayerMovement>();
            _characterController2D = GetComponent<CharacterController2D>();
            _gravityController = GetComponent<GravityController>();
            _dashController = GetComponent<DashController>();
            _constantForce2D = GetComponent<ConstantForce2D>();
            _boxCollider2D = GetComponent<BoxCollider2D>();
            _circleCollider2D = GetComponent<CircleCollider2D>();
            _rigidbody2D = GetComponent<Rigidbody2D>();

            _playerMovement.OnJump += () =>
            {
                OnPlayerJumped?.Invoke();
                AudioManager.instance.PlaySoundEffect(_audioSource, AudioTypes.Jump);
            };
            _playerMovement.OnLand += () =>
            {
                OnPlayerLand?.Invoke();
                AudioManager.instance.PlaySoundEffect(_audioSource, AudioTypes.Land);
            };
            _onPlayerStateChanged.AddListener(state =>
            {
                if (state == PlayerStateType.Walking)
                {
                    AudioManager.instance.PlaySoundEffect(_walkAudioSource, AudioTypes.Walk);
                }
                else
                {
                    AudioManager.instance.StopSoundEffect(_walkAudioSource);
                }
            });
        }

        private void Update()
        {
            if (isPlayerMoving)
            {
                SetPlayerState(PlayerStateType.Walking);
            }

            if (!isPlayerMoving && _playerMovement.IsGrounded && _currentPlayerStateType != PlayerStateType.Idle)
            {
                SetPlayerState(PlayerStateType.Idle);
            }

            if (Input.GetKeyDown(KeyCode.E) && !_isTransitioning)
            {
                if (_objectTransitionedTo == null)
                {
                    CheckTransitionObjects();
                }
                else
                {
                    GetOutFromItem();
                }
            }
        }

        private void SetPlayerState(PlayerStateType playerStateType)
        {
            if (playerStateType == _currentPlayerStateType)
                return;

            _currentPlayerStateType = playerStateType;
            _onPlayerStateChanged?.Invoke(_currentPlayerStateType);
        }

        public void Enable()
        {
            _playerMovement.enabled = true;
            _characterController2D.enabled = true;
            _boxCollider2D.isTrigger = false;
            _circleCollider2D.isTrigger = false;
            _constantForce2D.enabled = true;
            _dashController.enabled = true;
        }

        public void Disable()
        {
            _rigidbody2D.velocity = Vector2.zero;

            _playerMovement.enabled = false;
            _characterController2D.enabled = false;
            _boxCollider2D.isTrigger = true;
            _circleCollider2D.isTrigger = true;
            _constantForce2D.enabled = false;
            _dashController.enabled = false;
        }

        private void CheckTransitionObjects()
        {
            float minDistance = Single.MaxValue;

            ObjectController selectedObjectController = null;

            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _searchObjectRadius);
            for (int i = 0; i < colliders.Length; i++)
            {
                var currentGameObject = colliders[i].gameObject;

                if (currentGameObject != gameObject)
                {
                    if (currentGameObject.TryGetComponent(out ObjectController objectController))
                    {
                        var distance = Vector3.Distance(transform.position, objectController.transform.position);

                        if (distance < minDistance && objectController.CompareTag("Transitionable"))
                        {
                            selectedObjectController = objectController;
                            minDistance = distance;
                        }
                    }
                }
            }

            if (selectedObjectController != null)
            {
                Transition(selectedObjectController);
            }
        }

        public void Transition(ObjectController transitableObject)
        {
            _objectTransitionedTo = transitableObject;

            _isTransitioning = true;

            AudioManager.instance.PlaySoundEffect(_transitionAudioSource, AudioTypes.PossesIn);

            _circleCollider2D.enabled = false;
            _boxCollider2D.enabled = false;

            LeanTween.move(gameObject, transitableObject.transform, _transitionTime);
            LeanTween.scale(gameObject, Vector3.zero, _transitionTime).setOnComplete(OnTransitionCompleted);
        }

        private void OnTransitionCompleted()
        {
            Disable();

            _isTransitioning = false;

            _objectTransitionedTo.SetPlayer(this);
            
            OnTransitioned?.Invoke();
        }

        public void GetOutFromItem()
        {
            transform.position = _objectTransitionedTo.Transform.position;
            
            var targetPosition = _objectTransitionedTo.Transform.position + Vector3.down * 0.1f;

            _isTransitioning = true;
            
            LeanTween.move(gameObject, targetPosition, _transitionTime);

            LeanTween.scale(gameObject, Vector3.one, _transitionTime).setOnComplete(() =>
            {
                _isTransitioning = false;
                
                _circleCollider2D.enabled = true;
                _boxCollider2D.enabled = true;

                Enable();

                _objectTransitionedTo.PlayerGotOut();
                _objectTransitionedTo = null;

                _gravityController.SetGravity();
                
                OnPlayerGotOut?.Invoke();
            });
            
            AudioManager.instance.PlaySoundEffect(_transitionAudioSource, AudioTypes.PossesOut);
        }

        public void Die()
        {
            Disable();
            
            SetPlayerState(PlayerStateType.Die);
            
            AudioManager.instance.PlaySoundEffect(_audioSource,AudioTypes.Die);
            //TODO : Play Die Animation;

            GameManager.instance.LoseGame();
        }
        
        void OnCollisionEnter2D(Collision2D collision)
        {
            Debug.Log("Collision Called.");

            if (_collisionTriggered)
                return;
            
            if (collision.collider.CompareTag("Deadly"))
            {
                Die();
                _collisionTriggered = true;
                return;
            }

            if (collision.collider.CompareTag("Win"))
            {
                Disable();
                
                LeanTween.move(gameObject, collision.collider.transform, _transitionTime);
                LeanTween.scale(gameObject, Vector3.zero, _transitionTime).setOnComplete(GameManager.instance.WinGame);
                
                _collisionTriggered = true;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Win"))
            {
                Disable();
                
                LeanTween.move(gameObject, collision.transform, _transitionTime);
                LeanTween.scale(gameObject, Vector3.zero, _transitionTime).setOnComplete(GameManager.instance.WinGame);
                
                _collisionTriggered = true;
            }
        }
    }

    [Serializable]
    public class PlayerStateEvent : UnityEvent<PlayerStateType>
    {
    }
}