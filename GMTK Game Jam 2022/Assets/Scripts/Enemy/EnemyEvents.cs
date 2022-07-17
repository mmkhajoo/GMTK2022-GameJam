using UnityEngine;
using UnityEngine.Events;

namespace Enemy
{
    public class EnemyEvents : MonoBehaviour
    {
        [SerializeField] private Animator _animator;

        [SerializeField] private UnityEvent _eventChargeDelay;
        [SerializeField] private UnityEvent _eventChargeStart;
        [SerializeField] private UnityEvent _eventChargeEnd;


        [SerializeField] private UnityEvent _eventStartMoving;
        [SerializeField] private UnityEvent _eventDelayLanding;
        [SerializeField] private UnityEvent _eventStartLanding;
        [SerializeField] private UnityEvent _eventEndLanding;
        
        
        [SerializeField] private UnityEvent _eventStartCasting;
        [SerializeField] private UnityEvent _eventEndCasting;
        [SerializeField] private UnityEvent _eventProjectileCollision;

        [SerializeField] private UnityEvent _eventFireProjectile;
        [SerializeField] private UnityEvent _eventProjectileCollisionSmallBullet;
        [SerializeField] private UnityEvent _eventFireProjectileEnded;

        [SerializeField] private UnityEvent<Vector3> _eventLightLand;

        public void OnChargeDelay()
        {
            _eventChargeDelay?.Invoke();
        }

        public void OnChargeStart()
        {
            _animator.SetBool("charge", true);
            _eventChargeStart?.Invoke();
        }

        public void OnChargeEnd()
        {
            _animator.SetBool("charge", false);
            _eventChargeEnd?.Invoke();
        }

        public void OnStartMoving()
        {
            _eventStartMoving?.Invoke();
        }

        public void OnDelayLanding()
        {
            _animator.SetTrigger("landidle");
            _eventDelayLanding?.Invoke();
        }

        public void OnStartLanding()
        {
            _animator.SetBool("land", true);
            _eventStartLanding?.Invoke();
        }

        public void OnEndLanding()
        {
            _animator.SetBool("land", false);
            _eventEndLanding?.Invoke();
        }

        public void OnStartCasting()
        {
            _animator.SetBool("castbig", true);
            _eventStartCasting?.Invoke();
        }

        public void OnEndCasting()
        {
            _animator.SetBool("castbig", false);
            _eventEndCasting?.Invoke();
        }

        public void OnProjectileCollision()
        {
            _eventProjectileCollision?.Invoke();
        }

        public void OnFireProjectile()
        {
            _animator.SetBool("castsmall", true);
            _eventFireProjectile?.Invoke();
        }

        public void OnFireProjectileEnded()
        {
            _animator.SetBool("castsmall", false);
            _eventFireProjectile?.Invoke();
        }

        public void OnSmallProjectileCollision()
        {
            _eventProjectileCollisionSmallBullet?.Invoke();
        }

        public void OnLightLand(Vector3 position)
        {
            _eventLightLand?.Invoke(position);
        }
    }
}