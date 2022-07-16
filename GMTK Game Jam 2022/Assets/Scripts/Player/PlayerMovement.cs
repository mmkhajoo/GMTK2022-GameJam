using System;
using UnityEngine;

namespace DefaultNamespace
{
    [RequireComponent(typeof(CharacterController2D))]
    public class PlayerMovement : MonoBehaviour
    {
        public event Action OnJump;
        public event Action OnLand;

        public Vector3 Direction => transform.right.normalized;

        public float VerticalMove => _verticalMove;
        public float HorizontalMove => _horizontalMove;
        public bool IsGrounded => controller.IsGrounded;
        public bool IsAttackDone => controller.IsAttackDone;


        public CharacterController2D controller;

        protected float defaultRunSpeed;

        [SerializeField] protected float runSpeed = 40f;

        protected float _verticalMove = 0f;
        protected float _horizontalMove = 0f;

        protected bool _jump = false;
        private bool _attack = false;
        private bool _canAttack;
        private Vector2 _mousePosition;
        protected bool _waitForJumpButtonUp;

        private ConstantForce2D _constantForce2D;

        //TODO : Add Events for Play Idle and Move Animation on Player

        private void Awake()
        {
            defaultRunSpeed = runSpeed;

            _constantForce2D = GetComponent<ConstantForce2D>();
            _canAttack = true;

            controller.OnJumpAvailable += JumpAvailable;
            controller.OnLandEvent.AddListener(() =>
            {
                OnLand?.Invoke();
            });
        }

        private void JumpAvailable()
        {
            if (_jump)
            {
                _waitForJumpButtonUp = true;
            }
        }

        protected virtual void Update()
        {
            _verticalMove = Input.GetAxisRaw("Vertical") * runSpeed;
            _horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;


            // Debug.DrawLine(transform.position, transform.position + transform.right * 10, Color.yellow);
            if (IsAttackDone)
                _canAttack = true;

            GetInputs();
        }


        private void GetInputs()
        {
            if (Input.GetMouseButton(0) && _canAttack)
            {
                controller.SetAttackTimer();
                _attack = true;
                _canAttack = false;
                _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            }

            if (Input.GetButton("Jump") && !_waitForJumpButtonUp)
            {
                _jump = true;
            }
            else
            {
                _jump = false;
            }

            if (Input.GetButtonDown("Jump") && controller.IsGrounded)
            {
                OnJump?.Invoke();

                _waitForJumpButtonUp = false;
            }


            if (Input.GetButtonUp("Jump"))
            {
                if (controller.IsGrounded)
                    _waitForJumpButtonUp = false;
                else
                    _waitForJumpButtonUp = true;
            }
        }

        private void FixedUpdate()
        {
            if (_constantForce2D.force.y != 0f)
            {
                _verticalMove = 0;
            }
            else if (_constantForce2D.force.x != 0f)
            {
                _horizontalMove = 0;
            }

            controller.Move(_verticalMove * Time.fixedDeltaTime, _horizontalMove * Time.fixedDeltaTime, false, _jump);

            if (_attack)
            {
                controller.Attack(_mousePosition);
                _attack = false;
            }
        }
    }
}