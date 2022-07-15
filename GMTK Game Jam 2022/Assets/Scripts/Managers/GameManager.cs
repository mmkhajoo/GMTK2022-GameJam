using System;
using Managers.Audio_Manager;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        [SerializeField] private GameObject _panel;

        [SerializeField] private TextMeshProUGUI _gemText;


        [SerializeField] private GameObject _resetButton;
        [SerializeField] private GameObject _gemPanel;


        public int CurrentLevel => PlayerPrefs.GetInt("Level", 1);


        private const String Level = "Level";

        private MapManager _mapManager;

        [Header("Gem Events")] [SerializeField]
        private UnityEvent _onGemAdded;

        [SerializeField] private UnityEvent _onGedDecreased;
        [SerializeField] private AudioSource audioSource;


        private void Awake()
        {
            if (instance == null)
                instance = this;

            DontDestroyOnLoad(gameObject);

            if (!PlayerPrefs.HasKey("Level"))
            {
                PlayerPrefs.SetInt("Level", 1);
            }

            LoadMainMenu();
        }

        private void Start()
        {
            _mapManager = GetComponent<MapManager>();

            UpdateGem();
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                _panel.SetActive(!_panel.activeInHierarchy);
            }
#endif
        }

        public void LoseGame()
        {
            Debug.Log("You Lost Game.");

            if (PlayerPrefs.HasKey("Checkpoint"))
            {
                if (PlayerPrefs.HasKey("DeadMan"))
                {
                    _mapManager.DeadManGoNextLevel(() =>
                    {
                        if (Check_Checkpoint())
                            return;

                        Invoke("LoadLevel", 0.5f);
                    });
                }
                else
                {
                    _mapManager.FirstTimeShowMap(() => { Invoke("LoadLevel", 0.5f); });
                }

                return;
            }

            Invoke("LoadLevel", 0.5f);

            //TODO : Show the Lose Panel;
        }

        private bool Check_Checkpoint()
        {
            if (PlayerPrefs.GetInt("DeadMan") == CurrentLevel)
            {
                PlayerPrefs.SetInt("Gems", PlayerPrefs.GetInt("Gems") - 1);

                UpdateGem();

                _onGedDecreased?.Invoke();

                _mapManager.ResetToLastCheckPoint();

                PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Checkpoint"));

                _mapManager.FirstTimeShowMap(() => { Invoke("LoadLevel", 0.5f); });

                return true;
            }

            return false;
        }

        public void WinGame()
        {
            Debug.Log("You Won Game");

            if (PlayerPrefs.HasKey("Checkpoint"))
            {
                _mapManager.PlayerSetPosition();
            }

            IncreaseLevel();

            LoadLevel();

            //TODO : Show the Win Panel;
        }

        public void ResetLevel()
        {
            LoadLevel();
        }

        public void GoToNextLevel()
        {
            IncreaseLevel();

            LoadLevel();
        }

        public void GoPreviousLevel()
        {
            DecreaseLevel();

            LoadLevel();
        }

        private void IncreaseLevel()
        {
            PlayerPrefs.SetInt(Level, PlayerPrefs.GetInt(Level) + 1);
        }

        private void DecreaseLevel()
        {
            PlayerPrefs.SetInt(Level, PlayerPrefs.GetInt(Level) - 1);
        }

        private void LoadMainMenu()
        {
            SceneManager.LoadScene("Main Menu");
        }

        public void LoadLevel()
        {
            int level = PlayerPrefs.GetInt(Level, 1);

            if (level == SceneManager.sceneCountInBuildSettings)
            {
                level = SceneManager.sceneCountInBuildSettings - 2;
                PlayerPrefs.SetInt(Level, level);
            }
            else if (level <= 1)
            {
                PlayerPrefs.SetInt(Level, 2);
                level = 2;
            }

            if (level >= 3)
            {
                _resetButton.SetActive(true);
            }
            else
            {
                _resetButton.SetActive(false);
            }
            

            if (level >= 5)
            {
                _gemPanel.SetActive(true);
            }
            else
            {
                _gemPanel.SetActive(false);
            }

            if (level == 2)//Intro
            {
                AudioManager.instance.PauseSoundEffect(null);
            }
            if (level == 3)//Level 1
            {
                AudioManager.instance.ContinueSoundEffect(null);
            }
            SceneManager.LoadScene(level);
        }

        #region Gem

        public void AddGem()
        {
            PlayerPrefs.SetInt("Gems", PlayerPrefs.GetInt("Gems", 0) + 1);

            _gemText.SetText(PlayerPrefs.GetInt("Gems").ToString());

            AudioManager.instance.PlaySoundEffect(audioSource,AudioTypes.SoulShardCollect);
            _onGemAdded?.Invoke();
        }

        private void UpdateGem()
        {
            _gemText.SetText(PlayerPrefs.GetInt("Gems").ToString());
        }

        #endregion
    }
}