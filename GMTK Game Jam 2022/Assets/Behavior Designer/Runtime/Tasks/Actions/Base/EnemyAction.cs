using Enemy;
using UnityEngine;
using WeaponSystem;

namespace BehaviorDesigner.Runtime.Tasks
{
    public abstract class EnemyAction : Action
    {
        [SerializeField] private float _pauseTime;

        [SerializeField] protected int _damage;

        protected WeaponData _weaponData;

        private float _startTime;
        
        protected EnemyEvents _enemyEvents;

        public override void OnStart()
        {
            base.OnStart();

            var weaponManager = Resources.Load("WeaponManager") as WeaponManager;
            _weaponData = weaponManager.Get(CharacterType.Boss);

            _startTime = Time.time;

            _enemyEvents = GetComponent<EnemyEvents>();

            OnPauseTimeStart();
        }

        public override TaskStatus OnUpdate()
        {
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