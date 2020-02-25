using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RedRunner.UI
{
    public class EndScreen : UIScreen
    {
        [SerializeField]
        protected Button ResetButton = null;
        [SerializeField]
        protected Button HomeButton = null;
        [SerializeField]
        protected Button ExitButton = null;

        private void Start()
        {
            ResetButton.SetButtonAction(() =>
            {
                Debug.Log("shiver");
                GameManager.Singleton.Reset();
                Debug.Log("me");
                var ingameScreen = UIManager.Singleton.GetUIScreen(UIScreenInfo.IN_GAME_SCREEN);
                Debug.Log("god");
                UIManager.Singleton.OpenScreen(ingameScreen);
                Debug.Log("dammed");
                GameManager.Singleton.StartGame();
                Debug.Log("timbers");
            });
        }

        public override void UpdateScreenStatus(bool open)
        {
            base.UpdateScreenStatus(open);
        }
    }

}