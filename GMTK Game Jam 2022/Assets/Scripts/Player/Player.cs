using System;
using System.Collections.Generic;
using Managers;
using Managers.Audio_Manager;
using Objects;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using WeaponSystem;

namespace DefaultNamespace
{
    public class Player : MonoBehaviour, IPlayer, IDamageable
    {
        public Transform Transform => transform;

        [Header("Transition Configs")] [SerializeField]
        private float _searchObjectRadius = 1f;

        [SerializeField] private float _transitionTime = 0.5f;

        #region Fields

        [SerializeField]
        private int _health;

        private PlayerStateType _currentPlayerStateType = PlayerStateType.None;

        #endregion

        #region Controllers

        private PlayerMovement _playerMovement;
        private CharacterController2D _characterController2D;
        private DashController _dashController;
        private ConstantForce2D _constantForce2D;
        private BoxCollider2D _boxCollider2D;
        private CircleCollider2D _circleCollider2D;
        private Rigidbody2D _rigidbody2D;
        private WeaponData _weaponData;

        [SerializeField]
        private float immuneTime;
        private bool _isImmune;
        private float tempTime;

        #endregion

        #region Events

        [Header("Events")] [SerializeField] private PlayerStateEvent _onPlayerStateChanged;
        [SerializeField] private UnityEvent OnPlayerLand;
        [SerializeField] private UnityEvent OnPlayerJumped;
        [SerializeField] private UnityEvent OnTakeDamage;
        [SerializeField] private UnityEvent OnEndImmunity;
        [SerializeField] private UnityEvent OnDiceTimerDone;


        [Header("Audio Source")] [SerializeField]
        private AudioSource _audioSource;

        [SerializeField] private AudioSource _walkAudioSource;
        #endregion

        #region Private Properties

        public int Health => _health;

        private bool isPlayerMoving => _playerMovement.VerticalMove != 0f || _playerMovement.HorizontalMove != 0f;

        public CharacterType CharacterType => CharacterType.Player;

        public bool IsImmune
        {
            get
            {
                return _isImmune;
            }
            set
            {
                _isImmune = value;
            }
        }

        #endregion


        private bool _collisionTriggered;

        private void Awake()
        {
            var weaponManager = Resources.Load("WeaponManager") as WeaponManager;
            _weaponData = weaponManager.Get(CharacterType.Player);

            _playerMovement = GetComponent<PlayerMovement>();
            _characterController2D = GetComponent<CharacterController2D>();
            _dashController = GetComponent<DashController>();
            _constantForce2D = GetComponent<ConstantForce2D>();
            _boxCollider2D = GetComponent<BoxCollider2D>();
            _circleCollider2D = GetComponent<CircleCollider2D>();
            _rigidbody2D = GetComponent<Rigidbody2D>();

            var randomWeapon = UnityEngine.Random.Range(0, 4);
            _characterController2D.SetWeapon(_weaponData.weapons[randomWeapon]);

            _playerMovement.OnJump += () =>
            {
                OnPlayerJumped?.Invoke();
               // AudioManager.instance.PlaySoundEffect(_audioSource, AudioTypes.Jump);
            };
            _playerMovement.OnLand += () =>
            {
                OnPlayerLand?.Invoke();
               // AudioManager.instance.PlaySoundEffect(_audioSource, AudioTypes.Land);
            };

            _onPlayerStateChanged.AddListener(state =>
            {
                if (state == PlayerStateType.Walking)
                {
                   // AudioManager.instance.PlaySoundEffect(_walkAudioSource, AudioTypes.Walk);
                }
                else
                {
                   // AudioManager.instance.StopSoundEffect(_walkAudioSource);
                }
            });
        }

        private void Update()
        {
            if (Input.GetMouseButton(1))
            {
                TakeDamage(50);
            }
            if (isPlayerMoving)
            {
                SetPlayerState(PlayerStateType.Walking);
            }

            if (!isPlayerMoving && _playerMovement.IsGrounded && _currentPlayerStateType != PlayerStateType.Idle)
            {
                SetPlayerState(PlayerStateType.Idle);
            }

            if (IsImmune)
            {
                tempTime += Time.deltaTime;
                if(tempTime >= immuneTime)
                {
                    tempTime = 0;
                    _isImmune = false;
                    OnEndImmunity?.Invoke();
                }
            }

        }

        private void SetPlayerState(PlayerStateType playerStateType)
        {
            if (playerStateType == _currentPlayerStateType)
                return;

            _currentPlayerStateType = playerStateType;
           // _onPlayerStateChanged?.Invoke(_currentPlayerStateType);
        }

        public void RollOftheDice(int weaponNumber)
        {
            OnDiceTimerDone?.Invoke();
            _characterController2D.SetWeapon(_weaponData.weapons[weaponNumber]);
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

        public void TakeDamage(int hitDamage)
        {
            if (IsImmune)
                return;

            var tempHealth = _health - hitDamage;
            if (tempHealth <= 0) 
            {
                Die();
                return;
            }

            _health -= hitDamage;
            _isImmune = true;
            OnTakeDamage?.Invoke();
        }
    }
    

    [Serializable]
    public class PlayerStateEvent : UnityEvent<PlayerStateType>
    {
    }
}