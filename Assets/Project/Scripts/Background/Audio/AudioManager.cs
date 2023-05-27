using Project.Scripts.Background;
using UnityEngine;

namespace Background.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : Singleton<AudioManager>
    {
        [SerializeField] private AudioSource _audioSource1,_audioSource2;
        private AudioClip[] plopAudioClips, shootAudioClips;
        private int plopIndex = 0;

        protected override void Awake()
        {
            base.Awake();
            _audioSource1 = gameObject.GetComponent<AudioSource>();
            plopAudioClips = Resources.LoadAll<AudioClip>("Audio/SoundEffects/Plops");
            if(plopAudioClips.Length < 1) Debug.Log("no AudioClips loaded");
            shootAudioClips = Resources.LoadAll<AudioClip>("Audio/Shoot");
        }
        public void EnemyDeathSound()
        {
            if (plopIndex > plopAudioClips.Length-1) plopIndex = 0;
            _audioSource1.PlayOneShot(plopAudioClips[plopIndex]);
            plopIndex++;
        }

        public void PlayShootSound()
        {
            _audioSource2.PlayOneShot(shootAudioClips[0]);
        }
    }
}