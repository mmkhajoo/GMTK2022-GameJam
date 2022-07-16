namespace BehaviorDesigner.Runtime.Tasks
{
    public class RandomDice : Composite
    {
        public SharedInt _maxDice;
        
        private int _currentChildIndex;

        private System.Random _random;
        
        // The task status of the last child ran.
        private TaskStatus executionStatus = TaskStatus.Inactive;

        public override void OnAwake()
        {
            _random = new System.Random();
        }
        
        public override void OnStart()
        {
            SetRandomDice();
        }

        public override int CurrentChildIndex()
        {
            // Peek will return the index at the top of the stack.
            return _currentChildIndex;
        }

        public override void OnChildExecuted(TaskStatus childStatus)
        {
            executionStatus = childStatus;
        }
        
        public override void OnEnd()
        {
            // All of the children have run. Reset the variables back to their starting values.
            executionStatus = TaskStatus.Inactive;
        }

        public override void OnReset()
        {
            // Reset the public properties back to their original values
            _random = new System.Random();
        }

        private void SetRandomDice()
        {
            var dice = _random.Next(0, _maxDice.Value);
            _currentChildIndex = dice;
        }
    }
}