using Project.Scripts.Background;
using UnityEngine;

namespace Background.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : Singleton<AudioManager>
    {
        [SerializeField] private AudioSource _musicPlayer,_audioSource1,_audioSource2;
        private  AudioClip[] plopAudioClips, shootAudioClips;
        private AudioClip Win, Lose, _buttonClick;
        private int plopIndex = 0;

        protected override void Awake()
        {
            base.Awake();
            _audioSource1 = gameObject.GetComponent<AudioSource>();
            plopAudioClips = Resources.LoadAll<AudioClip>("Audio/SoundEffects/Plops/");
            shootAudioClips = new []
            {
                Resources.Load<AudioClip>("Audio/SoundEffects/Shoot/cartoon-plug-1"),
                Resources.Load<AudioClip>("Audio/SoundEffects/Shoot/cartoon-plug-2"),
                Resources.Load<AudioClip>("Audio/SoundEffects/Shoot/cartoon-plug-3"),
                Resources.Load<AudioClip>("Audio/SoundEffects/Shoot/cartoon-plug-4"),
                Resources.Load<AudioClip>("Audio/SoundEffects/Shoot/cartoon-plug-5"),
                Resources.Load<AudioClip>("Audio/SoundEffects/Shoot/cartoon-plug-6"),
                Resources.Load<AudioClip>("Audio/SoundEffects/Shoot/cartoon-plug-7"),
                Resources.Load<AudioClip>("Audio/SoundEffects/Shoot/cartoon-plug-8"),
                Resources.Load<AudioClip>("Audio/SoundEffects/Shoot/cartoon-plug-9"),
                Resources.Load<AudioClip>("Audio/SoundEffects/Shoot/cartoon-plug-10"),
                Resources.Load<AudioClip>("Audio/SoundEffects/Shoot/plopp-84863"),
                Resources.Load<AudioClip>("Audio/SoundEffects/Shoot/pop-1-35897"),
                Resources.Load<AudioClip>("Audio/SoundEffects/Shoot/wet-slop-plopwav0"),
            };
            _buttonClick = Resources.Load<AudioClip>("Audio/SoundEffects/Button/click-21156");
            Win = Resources.Load<AudioClip>("Audio/SoundEffects/Event/success-fanfare-trumpets-6185");
            Lose  = Resources.Load<AudioClip>("Audio/SoundEffects/Event/failure-1-89170");
        }
        public void EnemyDeathSound()
        {
            if (plopIndex > plopAudioClips.Length-1) plopIndex = 0;
            _audioSource1.PlayOneShot(plopAudioClips[plopIndex]);
            plopIndex++;
        }

        public void PlayShootSound(int index)
        {
            if (index < 0 || index >= shootAudioClips.Length)return;
            _audioSource2.PlayOneShot(shootAudioClips[index]);
        }
        
        public void PlayShootSound(int indexLowerBound, int indexUpperBound)
        {
            if (indexLowerBound < 0 || indexUpperBound > shootAudioClips.Length || indexLowerBound > indexUpperBound)return;
            _audioSource2.PlayOneShot(shootAudioClips[Random.Range(indexLowerBound,indexUpperBound)]);
        }

        public void StopMusic() => _musicPlayer.Stop();

        public void ButtonClicked() => _audioSource1.PlayOneShot(_buttonClick);

        public void PlayWinSound() => _audioSource1.PlayOneShot(Win);
        public void PlayLoseSound() => _audioSource1.PlayOneShot(Lose);
    }
}