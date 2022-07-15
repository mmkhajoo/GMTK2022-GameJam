using UnityEngine;

namespace Managers
{
    public class IntroManager : MonoBehaviour
    {
        public void GoNextLevel()
        {
            GameManager.instance.GoToNextLevel();
        }
    }
}