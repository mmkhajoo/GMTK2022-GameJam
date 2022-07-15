using System;
using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
    public bool IsGrounded => m_Grounded;

    public event Action OnJumpAvailable;


    [SerializeField] private float m_JumpForce = 400f; // Amount of force added when the player jumps.

    [SerializeField] private float jump_maxTime = 1f;

    private float _jumpTimeInterval = 0f;


    [Range(0, 1)] [SerializeField]
    private float m_CrouchSpeed = .36f; // Amount of maxSpeed applied to crouching movement. 1 = 100%

    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f; // How much to smooth out the movement
    [SerializeField] private bool m_AirControl = false; // Whether or not a player can steer while jumping;
    [SerializeField] private LayerMask m_WhatIsGround; // A mask determining what is ground to the character
    [SerializeField] private Transform m_GroundCheck; // A position marking where to check if the player is grounded.
    [SerializeField] private Transform m_CeilingCheck; // A position marking where to check for ceilings
    [SerializeField] private Collider2D m_CrouchDisableCollider; // A collider that will be disabled when crouching

    const float k_GroundedRadius = .01f; // Radius of the overlap circle to determine if grounded
    private bool m_Grounded; // Whether or not the player is grounded.
    const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
    private Rigidbody2D m_Rigidbody2D;
    private ConstantForce2D _constantForce2D;
    private bool m_FacingRight = true; // For determining which way the player is currently facing.
    private Vector3 m_Velocity = Vector3.zero;

    [Header("Events")] [Space] public UnityEvent OnLandEvent;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool>
    {
    }

    public BoolEvent OnCrouchEvent;
    private bool m_wasCrouching = false;


    private bool _isJumpCalled;

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        _constantForce2D = GetComponent<ConstantForce2D>();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();

        if (OnCrouchEvent == null)
            OnCrouchEvent = new BoolEvent();
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
                    OnLandEvent.Invoke();

                    OnJumpAvailable?.Invoke();
                    _isJumpCalled = false;

                    _jumpTimeInterval = 0;
                }
            }
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
                    OnCrouchEvent.Invoke(true);
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
            
            
            // If the input is moving the player right and the player is facing left...
            if (horizontalMove != 0f)
            {
                if (horizontalMove > 0 )
                {
                    if (transform.rotation.eulerAngles.z == 0)
                    {
                        transform.localRotation = Quaternion.Euler(new Vector3(transform.localRotation.eulerAngles.x, 0,
                            transform.localRotation.eulerAngles.z));
                    }
                    else
                    {
                        transform.localRotation = Quaternion.Euler(new Vector3(transform.localRotation.eulerAngles.x, 180,
                            transform.localRotation.eulerAngles.z));
                    }
                }
                else
                {
                    if (transform.rotation.eulerAngles.z == 0)
                    {
                        transform.localRotation = Quaternion.Euler(new Vector3(transform.localRotation.eulerAngles.x, 180,
                            transform.localRotation.eulerAngles.z));
                    }
                    else
                    {
                        transform.localRotation = Quaternion.Euler(new Vector3(transform.localRotation.eulerAngles.x, 0,
                            transform.localRotation.eulerAngles.z));
                    }
                }
            }


            if (verticalMove != 0)
            {
                if (verticalMove > 0 )
                {
                    if (transform.rotation.eulerAngles.z == 90)
                    {
                        transform.localRotation = Quaternion.Euler(new Vector3(0, transform.localRotation.eulerAngles.y,
                            transform.localRotation.eulerAngles.z));
                    }
                    else
                    {
                        transform.localRotation = Quaternion.Euler(new Vector3(180, transform.localRotation.eulerAngles.y,
                            transform.localRotation.eulerAngles.z));
                    }
                }
                else
                {
                    if (transform.rotation.eulerAngles.z == 90)
                    {
                        transform.localRotation = Quaternion.Euler(new Vector3(180, transform.localRotation.eulerAngles.y,
                            transform.localRotation.eulerAngles.z));
                    }
                    else
                    {
                        transform.localRotation = Quaternion.Euler(new Vector3(0, transform.localRotation.eulerAngles.y,
                            transform.localRotation.eulerAngles.z));
                    }
                }
            }
            
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


    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        // Multiply the player's x local scale by -1.

        if (transform.localRotation.eulerAngles.y == 0)
        {
            transform.localRotation = Quaternion.Euler(new Vector3(transform.localRotation.eulerAngles.x, 180,
                transform.localRotation.eulerAngles.z));
        }
        else
        {
            transform.localRotation = Quaternion.Euler(new Vector3(transform.localRotation.eulerAngles.x, 0,
                transform.localRotation.eulerAngles.z));
        }

        // transform.Rotate(Vector3.up * 180);
    }
}