using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DefaultNamespace
{
    public class GravityController : MonoBehaviour
    {
        [SerializeField] private float _gravityValue = 30f;


        [SerializeField] private ConstantForce2D _constantForce2D;
        [SerializeField] private PlayerMovement _playerMovement;

        public Dictionary<DirectionType, Transform> gorundDictionary;

        private DirectionType _currentDirectionType;

        private void Awake()
        {
            _constantForce2D = GetComponent<ConstantForce2D>();
            _playerMovement = GetComponent<PlayerMovement>();

            gorundDictionary = new Dictionary<DirectionType, Transform>();

            var directions = Enum.GetValues(typeof(DirectionType)).Cast<DirectionType>()
                .Where(x => x != DirectionType.None);

            foreach (var directionType in directions)
            {
                var ground = GameObject.Find(directionType.ToString() + "Ground");

                gorundDictionary.Add(directionType, ground.transform);
            }
        }


        private void Update()
        {
            float distance = Single.MaxValue;
            
            foreach (var ground in gorundDictionary)
            {
                float groundDistance;

                switch (ground.Key)
                {
                    case DirectionType.Up: case DirectionType.Down:
                        groundDistance = Mathf.Abs(transform.position.y - ground.Value.position.y);
                        break;
                    case DirectionType.Left : case DirectionType.Right :
                        groundDistance =  Mathf.Abs(transform.position.x - ground.Value.position.x);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (groundDistance < distance)
                {
                    _currentDirectionType = ground.Key;
                    distance = groundDistance;
                }
            }
        }
        
        public void SetGravity()
        {
            SetGravity(_currentDirectionType);
        }
        
        public void SetGravity(DirectionType directionType)
        {
            switch (directionType)
            {
                case DirectionType.Up:
                    
                    transform.rotation = Quaternion.Euler(new Vector3(0,0,180));
                    _constantForce2D.force = new Vector2(0, _gravityValue);
                    
                    break;
                
                case DirectionType.Down:
                    
                    transform.rotation = Quaternion.Euler(new Vector3(0,0,0));
                    _constantForce2D.force = new Vector2(0, -_gravityValue);
                    
                    break;
                
                case DirectionType.Left:
                    
                    transform.rotation = Quaternion.Euler(new Vector3(0,0,-90));
                    _constantForce2D.force = new Vector2(-_gravityValue, 0);
                    
                    break;
                case DirectionType.Right:
                    
                    transform.rotation = Quaternion.Euler(new Vector3(0,0,90));
                    _constantForce2D.force = new Vector2(_gravityValue, 0);
                    
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    }
}