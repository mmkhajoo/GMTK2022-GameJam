using Enemy;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class ChargeAction : EnemyAction
    {
        public SharedTransform _targetTransform;
        
        private EnemyMovement _enemyMovement;
        
        [SerializeField] private float _chargeSpeed;

        private Vector3 _direction;

        private Vector3 _targetPosition;

        private bool _isRight;

        private bool _collided;
        
        public override void OnAwake()
        {
            base.OnAwake();
            
            _enemyMovement = GetComponent<EnemyMovement>();
        }

        public override void OnStart()
        {
            base.OnStart();
            
            _collided = false;
            
            _targetPosition = _targetTransform.Value.position;

            _direction = (_targetPosition - transform.position).normalized;

            if (_direction.x > 0)
            {
                _isRight = true;
            }
            else
            {
                _isRight = false;
            }

            _enemyMovement.SetSpeed(_chargeSpeed);
            
            _enemyMovement.SetHorizontalValue(_isRight);
            
            _enemyEvents.OnChargeStart();
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
                
                //TODO : Add Damage Here;
                
                _collided = true;
                _enemyMovement.ResetSpeed();
                _enemyMovement.StopMovement();
                
                _enemyEvents.OnChargeEnd();
            }
        }
    }
}