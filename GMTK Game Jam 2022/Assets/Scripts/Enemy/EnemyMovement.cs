using DefaultNamespace;
using UnityEngine;

namespace Enemy
{
    public class EnemyMovement : PlayerMovement
    {
        private float _horizontalValue;

        private Transform _playerTransform;

        private void Start()
        {
            _playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        }

        protected override void Update()
        {
            _horizontalMove = _horizontalValue * runSpeed;
            
            Flip();
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
        
        
        private void Flip()
        {
            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var direction = _playerTransform.position.x - transform.position.x;

            // Multiply the player's x local scale by -1.

            if (direction >= 0)
            {
                transform.localRotation = Quaternion.Euler(new Vector3(transform.localRotation.eulerAngles.x, 0,
                    transform.localRotation.eulerAngles.z));
                
            }
            else
            {
                transform.localRotation = Quaternion.Euler(new Vector3(transform.localRotation.eulerAngles.x, 180,
                    transform.localRotation.eulerAngles.z));
            }
        }
    }
}