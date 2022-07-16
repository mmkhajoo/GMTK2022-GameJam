using Enemy;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class MoveTowardsWithDistance : Action
    {
        public SharedTransform _targetTransform;
        
        private Vector3 _targetPosition;
        private EnemyMovement _enemyMovement;
        private Vector3 _direction;

        private bool _isPositionsAreSame;

        public override void OnStart()
        {
            base.OnStart();

            _enemyMovement = GetComponent<EnemyMovement>();

            _targetPosition = _targetTransform.Value.transform.position;

            _isPositionsAreSame = false;

            if ((Mathf.Abs(transform.position.x - _targetPosition.x) < 0.5f))
            {
                _isPositionsAreSame = true;
            }
            
            _direction = (_targetPosition - transform.position).normalized;
            _direction.y = 0;
        }


        public override TaskStatus OnUpdate()
        {
            if (_isPositionsAreSame)
            {
                return TaskStatus.Success;
            }
            
            if (IsArrive())
            {
                _enemyMovement.StopMovement();
                return TaskStatus.Success;
            }
            
            bool isRight;
            
            if (_direction.x > 0)
                isRight = true;
            else
                isRight = false;

            _enemyMovement.SetHorizontalValue(isRight);

            return TaskStatus.Running;
        }

        private bool IsArrive()
        {
            if (Mathf.Abs(transform.position.x - _targetPosition.x) < 0.5f)
            {
                return true;
            }

            return false;
        }
    }
}