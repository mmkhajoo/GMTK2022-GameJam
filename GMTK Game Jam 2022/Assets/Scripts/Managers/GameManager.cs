using System;
using DefaultNamespace;
using Managers.Audio_Manager;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        [SerializeField] private GameObject _panel;

        [SerializeField] private AudioSource audioSource;
        [SerializeField] private Player _player;
        [SerializeField] private TextMeshProUGUI _timerText;
        [SerializeField] private Slider _playerhealthSlider;

        [SerializeField] private float _playerDiceTimer;
        private float tempTimer;



        private void Awake()
        {
            if (instance == null)
                instance = this;

            DontDestroyOnLoad(gameObject);


          //  LoadMainMenu();
        }

        private void Start()
        {
            tempTimer = _playerDiceTimer;
            _playerhealthSlider.maxValue = _player.Health;
            _playerhealthSlider.value = _player.Health;
        }

        private void Update()
        {
            if(tempTimer > 0)
            {
                tempTimer -= Time.deltaTime;
                _timerText.text = (int)tempTimer + " s";
                if (tempTimer <= 0)
                {
                    var rand = UnityEngine.Random.Range(0, 4);
                    _player.RollOftheDice();
                }
            }
        }

        public void OnRollDiceOfPlayerDone()
        {
            tempTimer = _playerDiceTimer;
        }

        public void OnHealthUpdate()
        {
            _playerhealthSlider.value = _player.Health;
        }

        public void LoseGame()
        {
            Debug.Log("You Lost Game.");

            SceneManager.LoadScene(0);

            //TODO : Show the Lose Panel;
        }


        public void WinGame()
        {
            Debug.Log("You Won Game");

            //TODO : Show the Win Panel;
        }

        public void ResetLevel()
        {
            LoadLevel();
        }


        private void LoadMainMenu()
        {
            SceneManager.LoadScene("Main Menu");
        }

        public void LoadLevel()
        {
            SceneManager.LoadScene(1);
        }

    }
}