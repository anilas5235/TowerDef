using System;
using Background;
using Scrips.Background;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace UIScripts
{
    public class UIMaster : MonoBehaviour
    {
        public static UIMaster Instance;

        [SerializeField] private GameObject[] windowControllers;
        [SerializeField] private Slider main, music, effects;
        [SerializeField] private AudioMixer mainAudioMixer;
        
        private UIStates currentUIState;

        private enum UIStates
        {
            Play = 0,
            Pause = 1,
            Options = 2,
            AudioOptions = 3,
        }

        private void Awake()
        {
            if (!Instance) Instance = this;
            else Destroy(this);
        }

        private void Start()
        {
            LoadFromSaveText();
            ChangeUIState(UIStates.Play);
        }

        private void Update()
        {
            if (Input.GetButtonDown("Cancel"))UIEsc();
        }

        private void ChangeUIState(UIStates newState)
        {
            switch (currentUIState)
            {
                case UIStates.Play: Time.timeScale = 0;
                    break;
                case UIStates.Pause: windowControllers[0].SetActive(false);
                    break;
                case UIStates.Options: windowControllers[1].SetActive(false);
                    break;
                case UIStates.AudioOptions: SaveOptionsToText(); windowControllers[2].SetActive(false); 
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            currentUIState = newState;

            switch (currentUIState)
            {
                case UIStates.Play: Time.timeScale = Shop.Instance.currentSpeedMode +1f;
                    break;
                case UIStates.Pause: windowControllers[0].SetActive(true);
                    break;
                case UIStates.Options:  windowControllers[1].SetActive(true);
                    break;
                case UIStates.AudioOptions: windowControllers[2].SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        public void ChangeUIStateWithIndex(int index)
        {
            UIStates state = index switch
            {
                0 => UIStates.Play,
                1 => UIStates.Pause,
                2 => UIStates.Options,
                3 => UIStates.AudioOptions,
                _ => throw new ArgumentException("No defined State for this Index")
            };
            ChangeUIState(state);
        }

        public void UIEsc()
        {
            UIStates state = currentUIState;
            bool set = true;
            switch (currentUIState)
            {
                case UIStates.Play: state = UIStates.Pause;
                    break;
                case UIStates.Pause: state = UIStates.Play;
                    break;
                case UIStates.Options: state = UIStates.Pause;
                    break;
                case UIStates.AudioOptions: state = UIStates.Options;
                    break;
                default: set = false; break;
            }

            if (set) ChangeUIState(state); 
        }
        public void UpdateSoundOptions()
        {
            mainAudioMixer.SetFloat("Master", ConvertSliderValueTodB(main.value));
            mainAudioMixer.SetFloat("Music", ConvertSliderValueTodB(music.value));
            mainAudioMixer.SetFloat("Effects", ConvertSliderValueTodB(effects.value));
        }

        private void SaveOptionsToText()
        {
            mainAudioMixer.GetFloat("Master", out SaveSystem.instance.GetActiveSave().audioOptions[0]);
            mainAudioMixer.GetFloat("Music", out SaveSystem.instance.GetActiveSave().audioOptions[1]);
            mainAudioMixer.GetFloat("Effects", out SaveSystem.instance.GetActiveSave().audioOptions[2]);
        }
        private void LoadFromSaveText()
        {
            float[] optionsValues = SaveSystem.instance.GetActiveSave().audioOptions;
            mainAudioMixer.SetFloat("Master", optionsValues[0]);
            mainAudioMixer.SetFloat("Music", optionsValues[1]);
            mainAudioMixer.SetFloat("Effects", optionsValues[2]);

            main.value = ConvertDBToSliderValue(optionsValues[0]);
            music.value = ConvertDBToSliderValue(optionsValues[1]);
            effects.value = ConvertDBToSliderValue(optionsValues[2]);
        }

        private float ConvertSliderValueTodB(float sliderValue)
        {
            return Mathf.Log10(sliderValue) * 20f;
        }

        private float ConvertDBToSliderValue(float dBValue)
        {
            return Mathf.Pow(10,(dBValue) / 20f);
        }
    }
}
