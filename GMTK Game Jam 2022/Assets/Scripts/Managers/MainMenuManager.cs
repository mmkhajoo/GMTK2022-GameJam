using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class MainMenuManager : MonoBehaviour
    {
        public void StartGame()
        {
            GameManager.instance.LoadLevel();
        }

        public void Exit()
        {
            Application.Quit();
        }

        public void ResetGame()
        {
            SceneManager.LoadScene("Main Menu");
            PlayerPrefs.DeleteAll();
        }
    }
}