using System;
using BehaviorDesigner.Runtime;
using Managers;
using Managers.Audio_Manager;
using UnityEngine;
using UnityEngine.Events;

namespace Enemy
{
    public class Enemy : MonoBehaviour , IDamageable
    {
        private BehaviorTree _behaviorTree;
        
        [SerializeField] private UnityEvent OnTakeDamage;

        [Header("Audio Source")] [SerializeField]
        private AudioSource _audioSource;

        [SerializeField]
        private int _health;

        private int _phaseNumber = 1;

        private float _healthDivided;
        
        public CharacterType CharacterType => CharacterType.Boss;

        private const string DiceValueName = "DiceNumber";

        private void Start()
        {
            _behaviorTree = GetComponent<BehaviorTree>();

            _healthDivided = _health / 4f;
            
            OnTakeDamage.AddListener(OnHealthChange);
        }

        private void OnHealthChange()
        {
            var leftHealth = _health - _healthDivided * (_phaseNumber + 1);

            if (_health < leftHealth)
            {
               var variable = _behaviorTree.GetVariable(DiceValueName);
               variable.SetValue(_phaseNumber);
               
               _behaviorTree.SetVariable(DiceValueName, variable);
            }
        }


        public void TakeDamage(int hitDamage)
        {
            var tempHealth = _health - hitDamage;
            if (tempHealth <= 0) 
            {
                Die();
                return;
            }

            _health -= hitDamage;
            OnTakeDamage?.Invoke();
        }
        
        public void Die()
        {
            Disable();
            
            AudioManager.instance.PlaySoundEffect(_audioSource,AudioTypes.Die);
            
            //TODO : Play Die Animation;

            GameManager.instance.WinGame();
        }
        
        public void Disable()
        {
        }
    }
}