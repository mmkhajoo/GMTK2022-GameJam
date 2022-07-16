using DefaultNamespace;

namespace Enemy
{
    public class EnemyMovement : PlayerMovement
    {
        private float _horizontalValue;
        
        protected override void Update()
        {
            _horizontalMove = _horizontalValue * runSpeed;
        }

        public void SetHorizontalValue(bool isRight)
        {
            if (isRight)
                _horizontalValue = 1f;
            else
            {
                _horizontalValue = -1f;
            }
        }

        public void SetSpeed(float speed)
        {
            runSpeed = speed;
        }

        public void ResetSpeed()
        {
            runSpeed = defaultRunSpeed;
        }

        public void StopMovement()
        {
            _horizontalValue = 0;
        }
    }
}