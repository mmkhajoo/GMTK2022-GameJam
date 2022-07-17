using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Managers.Audio_Manager;
using UnityEngine;
using UnityEngine.Events;
using WeaponSystem;

public class CharacterController2D : MonoBehaviour
{
    public bool IsGrounded => m_Grounded;
    public bool IsAttackDone => !_attack && !_attackTimeActive;

    public event Action OnJumpAvailable;
    public event Action<SubWeaponType> OnAttack;
    public event Action<SubWeaponType> OnMeleeAttackDone;

    [SerializeField] private Animator _animator;

    [SerializeField] private float m_JumpForce = 400f; // Amount of force added when the player jumps.

    [SerializeField] private float jump_maxTime = 1f;

    private float _jumpTimeInterval = 0f;


    [Range(0, 1)]
    [SerializeField]
    private float m_CrouchSpeed = .36f; // Amount of maxSpeed applied to crouching movement. 1 = 100%

    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f; // How much to smooth out the movement
    [SerializeField] private bool m_AirControl = false; // Whether or not a player can steer while jumping;
    [SerializeField] private LayerMask m_WhatIsGround; // A mask determining what is ground to the character
    [SerializeField] private Transform m_GroundCheck; // A position marking where to check if the player is grounded.
    [SerializeField] private Transform m_CeilingCheck; // A position marking where to check for ceilings
    [SerializeField] private Collider2D m_CrouchDisableCollider; // A collider that will be disabled when crouching

    [Header("Attacking")]
    [SerializeField] private Transform _porojectilePos;
    [SerializeField] private Transform _meleePosDetection;

    const float k_GroundedRadius = .5f; // Radius of the overlap circle to determine if grounded
    private bool m_Grounded; // Whether or not the player is grounded.
    const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
    private Rigidbody2D m_Rigidbody2D;
    private ConstantForce2D _constantForce2D;
    private bool m_FacingRight = true; // For determining which way the player is currently facing.
    private Vector3 m_Velocity = Vector3.zero;

    [SerializeField] private List<GameObject> _weaponSprites;
    [SerializeField] private List<GameObject> _weaponIcons;
    private Weapon _weapon;
    private bool _attack;
    private bool _attackTimeActive;
    private float tempFireRate;

    [Header("Events")]
    [Space]
    public UnityEvent OnLandEvent;
    public UnityEvent OnAttackEvent;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool>
    {
    }

    public BoolEvent OnCrouchEvent;
    private bool m_wasCrouching = false;

    private Vector3 _direction;

    public Vector3 Direction => _direction;

    private bool _isJumpCalled;

    [SerializeField] private GameObject meleeWeaponParticleSystem;
    
    [SerializeField] private AudioSource weaponAudioSource;
    [SerializeField] private AudioSource playerAudioSource;
    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        _constantForce2D = GetComponent<ConstantForce2D>();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();

        if (OnCrouchEvent == null)
            OnCrouchEvent = new BoolEvent();

        OnAttack += OnPlayerAttack;
    }

    private void Update()
    {
        bool wasGrounded = m_Grounded;
        m_Grounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                m_Grounded = true;

                if (!wasGrounded)
                {
                    OnLandEvent?.Invoke();

                    OnJumpAvailable?.Invoke();
                    _isJumpCalled = false;

                    _jumpTimeInterval = 0;
                }
            }
        }

        if (_attackTimeActive)
        {
            tempFireRate += Time.deltaTime;
            if (tempFireRate >= _weapon.fireRate)
            {
                tempFireRate = 0;
                _attack = false;
                _attackTimeActive = false;
            }
        }

    }

    public void CheckRotation(float horizontalMove)
    {
        // If the input is moving the player right and the player is facing left...
        if (horizontalMove != 0f)
        {
            if (horizontalMove > 0)
            {
                if (m_FacingRight)
                {
                    _direction = transform.right.normalized;

                }
                else
                {
                    _direction = -transform.right.normalized;
                }
            }
            else
            {
                if (!m_FacingRight)
                {
                    _direction = transform.right.normalized;
                }
                else
                {
                    _direction = -transform.right.normalized;
                }
            }
        }

        horizontalMove = Mathf.Abs(horizontalMove);
        _animator.SetFloat("foreward", horizontalMove);

        CheckFlip();
    }

    public void SetAttackState()
    {
        _attackTimeActive = true;

        switch (_weapon.weaponType)
        {
            case WeaponType.Melee:

                _animator.SetTrigger("melee");

                break;
            case WeaponType.Range:

                _animator.SetTrigger("range");

                break;
            default:
                break;
        }
    }

    public void Move(float verticalMove, float horizontalMove, bool crouch, bool jump)
    {
        // If crouching, check to see if the character can stand up
        // if (!crouch)
        // {
        //     // If the character has a ceiling preventing them from standing up, keep them crouching
        //     if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
        //     {
        //         crouch = true;
        //     }
        // }

        //only control the player if grounded or airControl is turned on

        if (m_Grounded || m_AirControl)
        {
            // If crouching
            if (crouch)
            {
                if (!m_wasCrouching)
                {
                    m_wasCrouching = true;
                    OnCrouchEvent?.Invoke(true);
                }

                // Reduce the speed by the crouchSpeed multiplier
                horizontalMove *= m_CrouchSpeed;

                // Disable one of the colliders when crouching
                if (m_CrouchDisableCollider != null)
                    m_CrouchDisableCollider.enabled = false;
            }
            else
            {
                // Enable the collider when not crouching
                if (m_CrouchDisableCollider != null)
                    m_CrouchDisableCollider.enabled = true;

                if (m_wasCrouching)
                {
                    m_wasCrouching = false;
                    OnCrouchEvent.Invoke(false);
                }
            }

            Vector3 targetVelocity = Vector3.zero;

            // Move the character Base On The Gravity We Set.
            if (_constantForce2D.force.y != 0f)
            {
                targetVelocity = new Vector2(horizontalMove * 10f, m_Rigidbody2D.velocity.y);
            }
            else if (_constantForce2D.force.x != 0f)
            {
                targetVelocity = new Vector2(m_Rigidbody2D.velocity.x, verticalMove * 10f);
            }

            // And then smoothing it out and applying it to the character
            m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity,
                m_MovementSmoothing);

        }

        // If the player should jump...
        if (jump)
        {
            if (m_Grounded)
            {
                // Add a vertical force to the player.
                m_Grounded = false;
                _isJumpCalled = true;
            }

            if (_jumpTimeInterval > jump_maxTime)
            {
                return;
            }

            _jumpTimeInterval += Time.deltaTime;

            if (_constantForce2D.force.y != 0f)
            {
                m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x,
                    m_JumpForce * -Mathf.Sign(_constantForce2D.force.y));
            }
            else if (_constantForce2D.force.x != 0f)
            {
                m_Rigidbody2D.velocity = new Vector2(m_JumpForce * -Mathf.Sign(_constantForce2D.force.x),
                    m_Rigidbody2D.velocity.y);
            }
        }
    }


    public void SetWeapon(Weapon weapon)
    {
        _weapon = weapon;
        Debug.Log($"Weapon Initialized {_weapon.subWeaponType}");

        foreach (var sprite in _weaponSprites)
            sprite.SetActive(false);

        foreach (var icon in _weaponIcons)
            icon.SetActive(false);

        int number = (int)weapon.subWeaponType;
        _weaponSprites[number].SetActive(true);
        _weaponIcons[number].SetActive(true);

    }

    public void Attack(Vector2 postion)
    {
        if (_attack)
            return;

        _attack = true;
        OnAttack?.Invoke(_weapon.subWeaponType);
        switch (_weapon.weaponType)
        {
            case WeaponType.Melee:

                _animator.SetTrigger("melee");
                MeleeAttack();

                break;
            case WeaponType.Range:

                _animator.SetTrigger("range");
                RangeAttack(postion);

                break;
            default:
                break;
        }

    }

    private void RangeAttack(Vector2 postion)
    {
        var projectile = Instantiate(_weapon.projectilePrefab, _porojectilePos.position, Quaternion.identity).GetComponent<Projectile>();
        projectile.Setup(postion, _weapon, CharacterType.Boss, gameObject);
    }

    private void MeleeAttack()
    {
        var hits = Physics2D.OverlapCircleAll(_meleePosDetection.position, _weapon.rangeDetect);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<IDamageable>(out var enemy))
            {
                if (enemy.CharacterType == CharacterType.Boss)
                {
                    enemy.TakeDamage(_weapon.weaponDamage);
                    OnMeleeAttackDone?.Invoke(_weapon.subWeaponType);
                }
            }
        }
    }

    private void CheckFlip()
    {
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var direction = mousePosition.x - transform.position.x;

        // Multiply the player's x local scale by -1.

        if (direction >= 0)
        {
            transform.localRotation = Quaternion.Euler(new Vector3(transform.localRotation.eulerAngles.x, 0,
                transform.localRotation.eulerAngles.z));
            m_FacingRight = true;
        }
        else
        {
            transform.localRotation = Quaternion.Euler(new Vector3(transform.localRotation.eulerAngles.x, 180,
                transform.localRotation.eulerAngles.z));
            m_FacingRight = false;
        }


    }

    public void OnPlayerAttack(SubWeaponType weaponType)
    {
        switch (weaponType)
        {
            case SubWeaponType.Sword:
                AudioManager.instance.PlaySoundEffect(weaponAudioSource,AudioTypes.Slicing);
                StartCoroutine(Player.ExecuteParticleSystem(meleeWeaponParticleSystem));
                break;
            case SubWeaponType.Axe:
                AudioManager.instance.PlaySoundEffect(weaponAudioSource,AudioTypes.AxeSlash);
                StartCoroutine(Player.ExecuteParticleSystem(meleeWeaponParticleSystem));
                break;
            case SubWeaponType.Bomb:
                AudioManager.instance.PlaySoundEffect(weaponAudioSource,AudioTypes.BombThrow);
                break;
            case SubWeaponType.Bullet:
                AudioManager.instance.PlaySoundEffect(weaponAudioSource,AudioTypes.ShurikenThrow);
                break;
            case SubWeaponType.CastFireBall:
                break;
            case SubWeaponType.FastFireBall:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(weaponType), weaponType, null);
        }
    }

   
}