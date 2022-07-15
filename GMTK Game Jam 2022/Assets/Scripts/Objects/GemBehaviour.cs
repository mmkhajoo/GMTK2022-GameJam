using System;
using Managers;
using UnityEngine;

namespace Objects
{
    public class GemBehaviour : MonoBehaviour
    {
        private bool _triggered;

        private string gemName;

        private void Awake()
        {
            var level = GameManager.instance.CurrentLevel;
            gemName = "Gem " + "Level " + level + name;
            
            if (PlayerPrefs.HasKey(gemName))
                gameObject.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                if(_triggered)
                    return;
                
                AddGem();
                Destroy(gameObject);
                _triggered = true;
            }

            if (collision.CompareTag("Transitionable"))
            {
                if (collision.GetComponent<ObjectController>().IsEnable)
                {
                    AddGem();
                    Destroy(gameObject);
                }
            }
        }
        
        private void AddGem()
        {
            GameManager.instance.AddGem();
            PlayerPrefs.SetInt(gemName, 0);
        }
    }
}