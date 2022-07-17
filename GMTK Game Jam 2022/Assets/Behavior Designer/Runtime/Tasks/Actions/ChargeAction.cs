using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class ChargeAction : EnemyAction
    {
        public SharedTransform _targetTransform;
        
        [SerializeField] private float _chargeSpeed;

        private Vector3 _targetPosition;
        
        private bool _collided;

        public override void OnStart()
        {
            base.OnStart();
            
            _collided = false;
            
            _targetPosition = _targetTransform.Value.position;
            
            var distance = Mathf.Sqrt(Mathf.Pow(_targetPosition.x - transform.position.x,2) +
                                      Mathf.Pow(_targetPosition.y - transform.position.y,2));

            var move = LeanTween.move(gameObject, _targetPosition, distance/_chargeSpeed);
            move.setOnComplete(OnComplete);

            _enemyEvents.OnChargeStart();
        }

        private void OnComplete()
        {
            _collided = true;
            _enemyEvents.OnChargeEnd();
        }


        protected override TaskStatus ExecuteAction()
        {
            if (!_collided)
                return TaskStatus.Running;
            
            return TaskStatus.Success;
        }

        protected override void OnPauseTimeStart()
        {
            base.OnPauseTimeStart();
            
            _enemyEvents.OnChargeDelay();
        }

        public override void OnCollisionEnter2D(Collision2D collision)
        {
            base.OnCollisionEnter2D(collision);
            
            if (collision.collider.gameObject != null)
            {
                if (collision.collider.gameObject.transform.parent != null)
                {
                    if(collision.collider.gameObject.transform.parent.name == "DownGround" || collision.collider.gameObject.transform.parent.name == "UpGround")
                        return;
                }

                if (collision.gameObject.TryGetComponent<IDamageable>(out var target))
                {
                    if (target.CharacterType == CharacterType.Player)
                        target.TakeDamage(_damage);
                }

                _collided = true;
                
                _enemyEvents.OnChargeEnd();
            }
        }
    }
}