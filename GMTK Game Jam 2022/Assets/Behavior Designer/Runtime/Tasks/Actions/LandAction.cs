using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class LandAction : EnemyAction
    {
        [SerializeField] private float _landingDistance;

        [SerializeField] private float _landingSpeed;
        
        protected override TaskStatus ExecuteAction()
        {
            return TaskStatus.Running;
        }
    }
}