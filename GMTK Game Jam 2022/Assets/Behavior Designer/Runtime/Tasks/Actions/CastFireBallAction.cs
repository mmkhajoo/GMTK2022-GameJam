using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using WeaponSystem;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class CastFireBallAction : EnemyAction
    {

        [SerializeField] private Transform _projectileInitPos;
        [SerializeField] private GameObject _fireBallGO;

        private bool _castTimeEnded;

        private Weapon _weapon;

        public SharedTransform _targetTransform;
        private Vector3 _targetPosition;


        public override void OnStart()
        {
            base.OnStart();

            if (_weapon == null)
            {
                _weapon = _weaponData.GetWeapon(SubWeaponType.CastFireBall);
            }

            _castTimeEnded = false;

            _targetPosition = _targetTransform.Value.position;

            StartCoroutine(CastAction());

            _enemyEvents.OnStartCasting();
        }

        private IEnumerator CastAction()
        {
            yield return new WaitForSeconds(_weapon.fireRate);

            var fireBall = GameObject.Instantiate(_fireBallGO, _projectileInitPos.position, Quaternion.identity).GetComponent<FireBall>();

            fireBall.Setup(_targetPosition, _weapon, CharacterType.Player, gameObject);

            _enemyEvents.OnEndCasting();

            _castTimeEnded = true;

        }

        protected override TaskStatus ExecuteAction()
        {
            if (!_castTimeEnded)
                return TaskStatus.Running;

            return TaskStatus.Success;
        }

    }
}