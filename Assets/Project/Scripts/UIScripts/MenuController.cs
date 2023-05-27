using Background.Audio;
using UIScripts;
using UnityEngine;

namespace Scripts.UIScripts
{
    public class MenuController : MonoBehaviour
    {
        public void Esc()
        {
            UIMaster.Instance.UIEsc();
            AudioManager.Instance.ButtonClicked();
        }

        public void ChangeScene(int index)
        {
            UIMaster.Instance.ChangeScene(index);
            AudioManager.Instance.ButtonClicked();
        }

        public void OpenOptionsMenu()
        {
            UIMaster.Instance.ChangeUIState(UIMaster.UIStates.Options);
            AudioManager.Instance.ButtonClicked();
        }

        public void OpenAudioOptionsMenu()
        {
            UIMaster.Instance.ChangeUIState(UIMaster.UIStates.AudioOptions);
            AudioManager.Instance.ButtonClicked();
        }

        public void ResumeGame()
        {
            UIMaster.Instance.ChangeUIState(UIMaster.UIStates.Normal);
            AudioManager.Instance.ButtonClicked();
        }

        public void Quit()
        {
            UIMaster.Instance.QuitApplication();
            AudioManager.Instance.ButtonClicked();
        }
    }
}