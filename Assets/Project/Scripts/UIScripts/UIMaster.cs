using System;
using Background.Audio;
using Project.Scripts.Background;
using UIScripts.ShopUi;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UIScripts
{
    public class UIMaster : Singleton<UIMaster>
    {

        [SerializeField] private GameObject[] windowControllers;
        [SerializeField] private Slider main, music, effects;
        [SerializeField] private AudioMixer mainAudioMixer;
        [SerializeField] private Scenes scene;
        
        private UIStates currentUIState;
        
        public enum Scenes
        {
            Title =0,
            Main = 1,
        }

        public enum UIStates
        {
            Normal = 0,
            Pause = 1,
            Options = 2,
            AudioOptions = 3,
            Lose = 5,
            Winn = 6,
        }

        private void Start()
        {
            LoadFromSaveText();

            switch (scene)
            {
                case Scenes.Title: ChangeUIStateInTitle(UIStates.Normal);
                    break;
                case Scenes.Main: ChangeUIStateInGame(UIStates.Normal);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Update()
        {
            if (Input.GetButtonDown("Cancel")) UIEsc();
            if (currentUIState == UIStates.AudioOptions) UpdateSoundOptions();
        }

        private void ChangeUIStateInGame(UIStates newState)
        {
            switch (currentUIState)
            {
                case UIStates.Normal: Time.timeScale = 0;
                    break;
                case UIStates.Pause: windowControllers[0].SetActive(false);
                    break;
                case UIStates.Options: windowControllers[1].SetActive(false);
                    break;
                case UIStates.AudioOptions: SaveOptionsToText(); windowControllers[2].SetActive(false); 
                    break;
                case UIStates.Lose: windowControllers[3].SetActive(false);
                    break;
                case UIStates.Winn: windowControllers[4].SetActive(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            currentUIState = newState;

            switch (currentUIState)
            {
                case UIStates.Normal: Time.timeScale = Shop.Instance.CurrentSpeedMode;
                    break;
                case UIStates.Pause: windowControllers[0].SetActive(true);
                    break;
                case UIStates.Options:  windowControllers[1].SetActive(true);
                    break;
                case UIStates.AudioOptions: windowControllers[2].SetActive(true);
                    break;
                case UIStates.Lose: windowControllers[3].SetActive(true); AudioManager.Instance.StopMusic();
                    AudioManager.Instance.PlayLoseSound();
                    break;
                case UIStates.Winn: windowControllers[4].SetActive(true); AudioManager.Instance.StopMusic();
                    AudioManager.Instance.PlayWinSound();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        private void ChangeUIStateInTitle(UIStates newState)
        {
            switch (currentUIState)
            {
                case UIStates.Normal: windowControllers[0].SetActive(false);
                    break;
                case UIStates.Options: windowControllers[1].SetActive(false);
                    break;
                case UIStates.AudioOptions: SaveOptionsToText(); windowControllers[2].SetActive(false);
                    break;
            }

            currentUIState = newState;

            switch (currentUIState)
            {
                case UIStates.Normal: windowControllers[0].SetActive(true);
                    break;
                case UIStates.Options: windowControllers[1].SetActive(true);
                    break;
                case UIStates.AudioOptions: windowControllers[2].SetActive(true);
                    break;
            }
        }

        public void ChangeUIState(UIStates newState)
        {
            switch (scene)
            {
                case Scenes.Title: ChangeUIStateInTitle(newState);
                    break;
                case Scenes.Main: ChangeUIStateInGame(newState);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        

        public void ChangeUIStateWithIndex(int index)
        {
            UIStates state;
            switch (scene)
            {
                case Scenes.Title:
                    state = index switch
                    {
                        0 => UIStates.Normal,
                        2 => UIStates.Options,
                        3 => UIStates.AudioOptions,
                        _ => throw new ArgumentException("No defined State for this Index")
                    };
                    ChangeUIStateInTitle(state);
                    break;
                case Scenes.Main:
                    state = index switch
                    {
                        0 => UIStates.Normal,
                        1 => UIStates.Pause,
                        2 => UIStates.Options,
                        3 => UIStates.AudioOptions,
                        4 => UIStates.Lose,
                        5 => UIStates.Winn,
                        _ => throw new ArgumentException("No defined State for this Index")
                    };
                    ChangeUIStateInGame(state);
                    break;
                default:
                    Debug.Log("Scene not registered");
                    break;
            }

        }

        public void UIEsc()
        {
            UIStates state = currentUIState;
            bool set = true;
            switch (scene)
            {
                case Scenes.Title:
                    switch (currentUIState)
                    {
                       case UIStates.Options:
                           state = UIStates.Normal;
                            break;
                        case UIStates.AudioOptions:
                            state = UIStates.Options;
                            break;
                        default:
                            set = false;
                            break;
                    }
                    if(set) ChangeUIStateInTitle(state);
                    break;
                case Scenes.Main:
                    switch (currentUIState)
                    {
                        case UIStates.Normal:
                            state = UIStates.Pause;
                            break;
                        case UIStates.Pause:
                            state = UIStates.Normal;
                            break;
                        case UIStates.Options:
                            state = UIStates.Pause;
                            break;
                        case UIStates.AudioOptions:
                            state = UIStates.Options;
                            break;
                        default:
                            set = false;
                            break;
                    }
                    if (set) ChangeUIStateInGame(state); 
                    break;
            }
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

        private float ConvertSliderValueTodB(float sliderValue) { return Mathf.Log10(sliderValue) * 20f; }
        private float ConvertDBToSliderValue(float dBValue) { return Mathf.Pow(10,(dBValue) / 20f); }

        public void QuitApplication() => Application.Quit();

        public void ChangeScene(Scenes scene)
        {
            switch (scene)
            {
                case Scenes.Title:SceneManager.LoadScene("TitleScreen");
                    break;
                case Scenes.Main: SceneManager.LoadScene("MainScene");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(scene), scene, null);
            }
        }

        public void ChangeScene(int id)=> SceneManager.LoadScene(id);
    }
}
