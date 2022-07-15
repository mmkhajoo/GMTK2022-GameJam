using Managers;
using UnityEngine;

namespace Objects
{
    public class CheckpointBehaviour : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                Debug.Log("Checkpoint Seted");
                
                var level = GameManager.instance.CurrentLevel+1;
                
                PlayerPrefs.SetInt("PreviousCheckpoint",PlayerPrefs.GetInt("Checkpoint",level));

                PlayerPrefs.SetInt("Checkpoint", level);
                
                //TODO : Change Head Sprite
            }
        }
    }
}