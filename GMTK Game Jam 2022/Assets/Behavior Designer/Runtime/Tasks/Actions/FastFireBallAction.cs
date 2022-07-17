using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponSystem;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class FastFireBallAction : EnemyAction
    {
        [SerializeField] private int _fireBallNumber;

        [SerializeField] private Transform _projectileInitPos;
        [SerializeField] private GameObject _fireBallGO;

        private bool _fireBallEnded;

        private Weapon _weapon;

        public SharedTransform _targetTransform;
        private Vector3 _targetPosition;


        public override void OnStart()
        {
            base.OnStart();

            if (_weapon == null)
            {
                _weapon = _weaponData.GetWeapon(SubWeaponType.FastFireBall);
            }

            _fireBallEnded = false;

            _targetPosition = _targetTransform.Value.position;

            StartCoroutine(ShootFireBall());

        }

        private IEnumerator ShootFireBall()
        {
            for (int i = 0; i < _fireBallNumber; i++)
            {
                var f1 = GameObject.Instantiate(_fireBallGO, _projectileInitPos.position, Quaternion.identity).GetComponent<FireBall>();
                f1.Setup(_targetPosition, _weapon, CharacterType.Player, gameObject);
                _enemyEvents.OnFireProjectile();

                yield return new WaitForSeconds(_weapon.fireRate);
            }

            _enemyEvents.OnFireProjectileEnded();
            _fireBallEnded = true;
        }

        protected override TaskStatus ExecuteAction()
        {
            if (!_fireBallEnded)
                return TaskStatus.Running;

            return TaskStatus.Success;
        }
    }
}