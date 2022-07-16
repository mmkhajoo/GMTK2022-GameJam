using UnityEngine;
using UnityEngine.Events;

namespace Enemy
{
    public class EnemyEvents : MonoBehaviour
    {
        [SerializeField] private UnityEvent _eventChargeDelay;
        [SerializeField] private UnityEvent _eventChargeStart;
        [SerializeField] private UnityEvent _eventChargeEnd;


        [SerializeField] private UnityEvent _eventStartMoving;
        [SerializeField] private UnityEvent _eventStartLanding;
        [SerializeField] private UnityEvent _eventEndLanding;
        
        
        [SerializeField] private UnityEvent _eventStartCasting;
        [SerializeField] private UnityEvent _eventEndCasting;
        [SerializeField] private UnityEvent _eventProjectileCollision;

        [SerializeField] private UnityEvent _eventFireProjectile;
        [SerializeField] private UnityEvent _eventProjectileCollisionSmallBullet;

        [SerializeField] private UnityEvent<Vector3> _eventLightLand;

        public void OnChargeDelay()
        {
            _eventChargeDelay?.Invoke();
        }

        public void OnChargeStart()
        {
            _eventChargeStart?.Invoke();
        }

        public void OnChargeEnd()
        {
            _eventChargeEnd?.Invoke();
        }

        public void OnStartMoving()
        {
            _eventStartMoving?.Invoke();
        }

        public void OnStartLanding()
        {
            _eventStartLanding?.Invoke();
        }

        public void OnEndLanding()
        {
            _eventEndLanding?.Invoke();
        }

        public void OnStartCasting()
        {
            _eventStartCasting?.Invoke();
        }

        public void OnEndCasting()
        {
            _eventEndCasting?.Invoke();
        }

        public void OnProjectileCollision()
        {
            _eventProjectileCollision?.Invoke();
        }

        public void OnFireProjectile()
        {
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