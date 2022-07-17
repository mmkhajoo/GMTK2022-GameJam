using System;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Managers;
using Managers.Audio_Manager;
using UnityEngine;
using UnityEngine.Events;

namespace Enemy
{
    public class Enemy : MonoBehaviour, IDamageable
    {
        private BehaviorTree _behaviorTree;
        [SerializeField] private List<GameObject> _actionIcons;

        [SerializeField] private GameObject _iconSpriteParent;
        [SerializeField] private GameObject _defaultBody;
        
        [SerializeField] private GameObject _diceAnimationGO;


        [SerializeField] private UnityEvent OnTakeDamage;


        [Header("Audio Source")] [SerializeField]
        private AudioSource _audioSource;

        [SerializeField] private int _health;

        private int _phaseNumber = 1;

        private float _healthDivided;

        public CharacterType CharacterType => CharacterType.Boss;

        private const string DiceValueName = "DiceNumber";

        private Dictionary<ActionTypes, GameObject> actionSpriteDic = new Dictionary<ActionTypes, GameObject>();

        private void Start()
        {

            foreach (var icon in _actionIcons)
            {
                if (Enum.TryParse(icon.name, out ActionTypes actionTypes))
                {
                    actionSpriteDic[actionTypes] = icon;
                }
            }
            
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

        public void ShowDiceAnimation()
        {
            _iconSpriteParent.SetActive(false);
            _defaultBody.SetActive(false);
            _diceAnimationGO.SetActive(true);
        }

        public void HideDiceAnimation()
        {
            _iconSpriteParent.SetActive(true);
            _defaultBody.SetActive(true);
            _diceAnimationGO.SetActive(false);
        }

        public void ShowActionIcon(ActionTypes actionTypes)
        {
            foreach (var icon in _actionIcons)
            {
                icon.SetActive(false);
            }

            actionSpriteDic[actionTypes].SetActive(true);
        }

        public void Die()
        {
            Disable();

            AudioManager.instance.PlaySoundEffect(_audioSource, AudioTypes.Die);

            //TODO : Play Die Animation;

            GameManager.instance.WinGame();
        }

        public void Disable()
        {
        }
    }
}