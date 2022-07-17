using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class SetRandomDiceAction : EnemyAction
    {
        public SharedInt _maxDice;

        public SharedInt _currentChildIndex;
        
        private System.Random _random;
        
        private Enemy.Enemy _enemy;
        
        private TaskStatus _taskStatus;

        [SerializeField] private List<ActionTypes> _actionTypesList;

        public override void OnAwake()
        {
            base.OnAwake();
            
            _random = new System.Random();
            
            _enemy = GetComponent<Enemy.Enemy>();
        }

        public override void OnStart()
        {
            base.OnStart();

            _taskStatus = TaskStatus.Running;

            StartCoroutine(ChangeDice());
        }

        protected override TaskStatus ExecuteAction()
        {
            return _taskStatus;
        }

        public override void OnReset()
        {
            // Reset the public properties back to their original values
            _random = new System.Random();
        }
        
        private void SetRandomDiceFunction()
        {
            var dice = _random.Next(0, _maxDice.Value);
            
            _currentChildIndex = dice;
        }
        
        private IEnumerator ChangeDice()
        {
            SetRandomDiceFunction();

            _enemy.ShowDiceAnimation();
            
            yield return new WaitForSeconds(1f);
            
            _enemy.ShowActionIcon(_actionTypesList[_currentChildIndex.Value]);

            _enemy.HideDiceAnimation();

            _taskStatus = TaskStatus.Success;
        }
    }
}