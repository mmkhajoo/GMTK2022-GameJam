using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class LandAction : EnemyAction
    {
        [SerializeField] private float _landingDistance;
        
        [SerializeField] private float _landingSpeed;

        [SerializeField] private float _landingDelay;

        public SharedTransform _targetTransform;

        private TaskStatus _taskStatus = TaskStatus.Running;

        public override void OnStart()
        {
            base.OnStart();

            _taskStatus = TaskStatus.Running;

            _enemyEvents.OnStartMoving();

            var targetPosition = new Vector3(_targetTransform.Value.position.x, _landingDistance);

            var moveY = LeanTween.move(gameObject, targetPosition, _landingDistance / _landingSpeed);
            moveY.setOnComplete(OnPlayerIsInSky);
        }

        private void OnPlayerIsInSky()
        {
            gameObject.transform.position =
                new Vector3(_targetTransform.Value.position.x, gameObject.transform.position.y);

            _enemyEvents.OnStartLanding();

            LeanTween.delayedCall(gameObject, _landingDelay, PlayerLanding);
        }

        private void PlayerLanding()
        {
            var moveY = LeanTween.moveY(gameObject, -5, _landingDistance / _landingSpeed);
            moveY.setOnComplete(OnEnemyLandOnPlayer);
        }

        private void OnEnemyLandOnPlayer()
        {
            _enemyEvents.OnEndLanding();

            _taskStatus = TaskStatus.Success;
        }

        protected override TaskStatus ExecuteAction()
        {
            return _taskStatus;
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
                
                _enemyEvents.OnChargeEnd();
            }
        }
    }
}