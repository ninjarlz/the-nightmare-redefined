using GameLogic;
using UnityEngine;

namespace UI
{
    public class ButtonsControll : MonoBehaviour
    {
        public static bool screensOver = false;
        public void SkipWelcomeScreen()
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);
        }

        public void SkipControls()
        {
            transform.GetChild(1).gameObject.SetActive(false);
            screensOver = true;
            GameManager.IsListeningForReady = true;
        }
    }
}
