using Project.Scripts.Background;
using UnityEngine;

namespace Background.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : Singleton<AudioManager>
    {
        [SerializeField] private AudioSource _musicPlayer,_audioSource1,_audioSource2;
        private AudioClip[] plopAudioClips, shootAudioClips;
        private AudioClip Win, Lose;
        private int plopIndex = 0;

        protected override void Awake()
        {
            base.Awake();
            _audioSource1 = gameObject.GetComponent<AudioSource>();
            plopAudioClips = Resources.LoadAll<AudioClip>("Audio/SoundEffects/Plops");
            if(plopAudioClips.Length < 1) Debug.Log("no AudioClips loaded");
            shootAudioClips = Resources.LoadAll<AudioClip>("Audio/SoundEffects/Shoot");
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

        public void StopMusic()
        {
            _musicPlayer.Stop();
        }

        public void PlayWinSound() => _audioSource1.PlayOneShot(Win);
        public void PlayLoseSound() => _audioSource1.PlayOneShot(Lose);
    }
}