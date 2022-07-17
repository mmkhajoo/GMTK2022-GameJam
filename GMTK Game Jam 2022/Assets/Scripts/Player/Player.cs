using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using Managers.Audio_Manager;
using UnityEngine;
using UnityEngine.Events;
using WeaponSystem;

namespace DefaultNamespace
{
    public class Player : MonoBehaviour, IPlayer, IDamageable
    {
        public Transform Transform => transform;

        [SerializeField] private Animator _animator;

        #region Fields

        [SerializeField] private int _health;

        [SerializeField] private GameObject _rollDiceGO;
        [SerializeField] private GameObject _mainBody;
        [SerializeField] private float _rollDiceTimer = 0.5f;
        private float tempTimerDice;
        private bool _isDiceRoll;

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
        private float tempTimer;

        private int _lastWeapon;

        #endregion

        #region Events

        [Header("Events")] [SerializeField] private PlayerStateEvent _onPlayerStateChanged;
        [SerializeField] private UnityEvent OnPlayerLand;
        [SerializeField] private UnityEvent OnPlayerJumped;
        [SerializeField] private UnityEvent OnTakeDamage;
        [SerializeField] private UnityEvent OnEndImmunity;
        [SerializeField] private UnityEvent OnDiceBegin;
        [SerializeField] private UnityEvent OnDiceRolledDone;


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
        [SerializeField] private GameObject jumpParticleSystem;


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

            _lastWeapon = UnityEngine.Random.Range(0, 4);
            _characterController2D.SetWeapon(_weaponData.weapons[2]);

            _playerMovement.OnJump += () =>
            {
                OnPlayerJumped?.Invoke();
                AudioManager.instance.PlaySoundEffect(_audioSource, AudioTypes.Jump);
                StartCoroutine(ExecuteParticleSystem(jumpParticleSystem));
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
                   //AudioManager.instance.PlaySoundEffect(_walkAudioSource, AudioTypes.Walk);
                }
                else
                {
                   //.instance.StopSoundEffect(_walkAudioSource);
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

            if (IsImmune)
            {
                tempTimer += Time.deltaTime;
                if(tempTimer >= immuneTime)
                {
                    tempTimer = 0;
                    _isImmune = false;
                    _animator.SetLayerWeight(1, 0);
                    OnEndImmunity?.Invoke();
                }
            }

            if (_isDiceRoll)
            {
                tempTimerDice += Time.deltaTime;
                if (tempTimerDice >= _rollDiceTimer)
                {
                    tempTimerDice = 0;
                    _isDiceRoll = false;

                    _rollDiceGO.SetActive(false);
                    _mainBody.SetActive(true);

                    int weaponNumber = _lastWeapon;
                    while (_lastWeapon == weaponNumber)
                    {
                        weaponNumber = UnityEngine.Random.Range(0, 4); 
                    }
                    _lastWeapon = weaponNumber;
                    _characterController2D.SetWeapon(_weaponData.weapons[weaponNumber]);

                    OnDiceRolledDone?.Invoke();
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

        public void RollOftheDice()
        {
            _rollDiceGO.SetActive(true);
            _mainBody.SetActive(false);
            _isDiceRoll = true;
            OnDiceBegin?.Invoke();
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
                
                
                _collisionTriggered = true;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Win"))
            {
                Disable();
                

                
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
            _animator.SetLayerWeight(1, 1);
            OnTakeDamage?.Invoke();
        }

        public void PlaySound(string str)
        {
            AudioManager.instance.PlaySoundEffect(_audioSource,Enum.Parse<AudioTypes>(str));
        }
        public void StopSound()
        {
            AudioManager.instance.StopSoundEffect(_audioSource);
        }
        
        
        public static IEnumerator ExecuteParticleSystem(GameObject obj)
        {
            obj.SetActive(true);
            yield return new WaitForSeconds(0.3f);
            obj.SetActive(false);
        }
    }


    [Serializable]
    public class PlayerStateEvent : UnityEvent<PlayerStateType>
    {
    }
}