using System;
using UnityEngine;

namespace Managers
{
    public class MapManager : MonoBehaviour
    {
        [SerializeField] private GameObject _deadMan;
        [SerializeField] private GameObject _player;
        [SerializeField] private GameObject _panel;
        [SerializeField] private GameObject[] _paths;

        private const string DeadMan = "DeadMan";

        private void Start()
        {
            SetPlayerAndDeadManPosition();
        }

        public void FirstTimeShowMap(Action onDone = null)
        {
            PlayerPrefs.SetInt(DeadMan, 1);

            ShowMap(() =>
            {
                MovePlayerAndDeadMan(() =>
                {
                    onDone?.Invoke();
                    CloseMap();

                });
            });
        }

        public void ResetToLastCheckPoint()
        {
            PlayerPrefs.SetInt(DeadMan, PlayerPrefs.GetInt("PreviousCheckpoint"));
            
            SetPlayerAndDeadManPosition();
        }


        public void DeadManGoNextLevel(Action onDone = null)
        {
            IncreaseDeadManLevel();

            ShowMap(() =>
            {
                var deadman = PlayerPrefs.GetInt(DeadMan);
                LeanTween.move(_deadMan, _paths[deadman - 1].transform, 1f).setOnComplete(() =>
                {
                    onDone?.Invoke();
                    CloseMap();
                });
            });
        }

        public void PlayerSetPosition()
        {
            var level = GameManager.instance.CurrentLevel;
            _player.transform.position = _paths[level - 1].transform.position;
        }
        
        private void SetPlayerAndDeadManPosition()
        {
            if(!PlayerPrefs.HasKey("Checkpoint"))
                return;
            
            var level = GameManager.instance.CurrentLevel;
            var deadman = PlayerPrefs.GetInt(DeadMan);


            _player.transform.position = _paths[level - 1].transform.position;
            _deadMan.transform.position = _paths[deadman - 1].transform.position;
        }

        private void ShowMap(Action onMapOpened = null)
        {
            _panel.SetActive(true);
            
            _panel.transform.localScale = Vector3.zero;

            LeanTween.scale(_panel, Vector3.one, 1f).setOnComplete(onMapOpened);
        }

        private void CloseMap(Action onMapClosed = null)
        {
            LeanTween.scale(_panel, Vector3.zero, 1f).setOnComplete(() =>
            {
                onMapClosed?.Invoke();
                _panel.SetActive(false);
            });
        }
        
        private void MovePlayerAndDeadMan(Action onDone = null)
        {
            var level = GameManager.instance.CurrentLevel;
            var deadman = PlayerPrefs.GetInt(DeadMan);

            LeanTween.move(_player, _paths[level - 1].transform, 1f);
            LeanTween.move(_deadMan, _paths[deadman - 1].transform, 1f).setOnComplete(onDone);
        }

        private void IncreaseDeadManLevel()
        {
            PlayerPrefs.SetInt(DeadMan, PlayerPrefs.GetInt(DeadMan) + 1);
        }

        private void Update()
        {
#if UNITY_EDITOR
            
            if (Input.GetKeyDown(KeyCode.I))
            {
                FirstTimeShowMap();
            }

            if (Input.GetKeyDown(KeyCode.O))
            {
                DeadManGoNextLevel();
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                _deadMan.transform.position = _paths[0].transform.position;
                _player.transform.position = _paths[3].transform.position;
            }
#endif
        }
    }
}