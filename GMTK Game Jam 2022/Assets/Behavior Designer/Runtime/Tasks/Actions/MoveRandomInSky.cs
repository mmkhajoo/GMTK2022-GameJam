using UnityEngine;
using Random = UnityEngine.Random;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class MoveRandomInSky : EnemyAction
    {
        public SharedTransform _startTransform;
        public SharedTransform _endTransform;


        [SerializeField] private float _speed;

        [SerializeField] private float _offset;
        

        private TaskStatus _taskStatus;

        private Vector3 _targetPosition;

        public override void OnStart()
        {
            base.OnStart();

            _taskStatus = TaskStatus.Running;

            var x = Random.Range(_startTransform.Value.position.x + _offset, _endTransform.Value.position.x - _offset);
            var y = Random.Range(_startTransform.Value.position.y + _offset, _endTransform.Value.position.y - _offset);

            _targetPosition = new Vector3(x, y);

            var distance = Mathf.Sqrt(Mathf.Pow(_targetPosition.x - transform.position.x,2) +
                                      Mathf.Pow(_targetPosition.y - transform.position.y,2));

            var move = LeanTween.move(gameObject, _targetPosition, distance / _speed);
            move.setOnComplete(OnComplete);
        }

        private void OnComplete()
        {
            _taskStatus = TaskStatus.Success;
        }

        protected override TaskStatus ExecuteAction()
        {
            return _taskStatus;
        }
    }
}