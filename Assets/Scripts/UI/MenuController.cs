using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class MenuController : MonoBehaviour
    {
        public void GoToLobby()
        {
            SceneManager.LoadScene("LobbyTests");
        }

        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public void Quit()
        {
            Application.Quit();
        }
    }
}
