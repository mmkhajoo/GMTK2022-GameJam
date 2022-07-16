using Enemy;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public abstract class EnemyAction : Action
    {
        [SerializeField] private float _pauseTime;

        [SerializeField] private int _damage;

        private float _startTime;
        
        protected EnemyEvents _enemyEvents;

        public override void OnStart()
        {
            base.OnStart();

            _startTime = Time.time;

            _enemyEvents = GetComponent<EnemyEvents>();
        }

        public override TaskStatus OnUpdate()
        {
            OnPauseTimeStart();
            var passesTime = Time.time - _startTime;

            if (passesTime < _pauseTime)
            {
                OnPauseTimeEnd();
                return TaskStatus.Running;
            }

            return ExecuteAction();
        }

        protected abstract TaskStatus ExecuteAction();

        protected virtual void OnPauseTimeStart()
        {
        }

        protected virtual void OnPauseTimeEnd()
        {
        }
    }
}