using System;
using DefaultNamespace;
using Managers;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace Objects
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class ObjectController : MonoBehaviour, IObject
    {
        #region Properties

        private IPlayer _player;
        public Transform Transform => transform;
        public Collider2D Collider2D => _collider2D;
        public Rigidbody2D Rigidbody2D => _rigidbody2D;
        public bool IsEnable { get; private set; }
        public ObjectType ObjectType => _objectType;

        #endregion

        [SerializeField] private ObjectType _objectType;

        [Header("Win Transition Time")] [SerializeField]
        private float _transitionTime = 0.1f;

        #region Events

        [Header("Events")] [SerializeField] private UnityEvent OnTransitioned;
        [SerializeField] private UnityEvent OnPlayerGotOut;

        #endregion

        protected Collider2D _collider2D;
        protected Rigidbody2D _rigidbody2D;


        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _collider2D = GetComponent<Collider2D>();

            Disable();
        }

        private void Update()
        {
            if (_player != null)
                _player.Transform.position = transform.position;
        }

        public void SetPlayer(IPlayer player)
        {
            _player = player;

            Enable();

            OnTransitioned.Invoke();
            //TODO : Maybe Enable Item Somewhere Else.
        }

        public void PlayerGotOut()
        {
            _player = null;

            Disable();

            OnPlayerGotOut.Invoke();
        }

        public void Destroy(bool isDestroySelf)
        {
            if (_player != null)
                _player.Die();

            if (isDestroySelf)
            {
                // Destroy Animation
                UnityEngine.Object.Destroy(gameObject);
            }
        }

        #region Private Methods

        protected virtual void Enable()
        {
            IsEnable = true;
            // _rigidbody2D.isKinematic = false;
        }

        protected virtual void Disable()
        {
            IsEnable = false;

            _rigidbody2D.velocity = Vector2.zero;
            // _rigidbody2D.isKinematic = true;
        }

        #endregion

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.CompareTag("Deadly"))
            {
                Destroy(true);
            }

            if (collision.collider.CompareTag("Win"))
            {
                if (_player != null)
                {
                    LeanTween.move(gameObject, collision.collider.transform, _transitionTime);
                    LeanTween.scale(gameObject, Vector3.zero, _transitionTime)
                        .setOnComplete(GameManager.instance.WinGame);
                }
            }
        }
    }
}