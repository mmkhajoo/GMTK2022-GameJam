using System;
using System.Collections.Generic;
using UnityEngine;

namespace Managers.Audio_Manager
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager instance;

        [SerializeField] private AudioClip[] _audioClips;

        private Dictionary<AudioTypes, AudioClip> _audioClipsDic;

        [SerializeField] private AudioSource _mainSoundSource;

        private void Awake()
        {
            if (instance == null)
                instance = this;

            DontDestroyOnLoad(gameObject);

            _audioClipsDic = new Dictionary<AudioTypes, AudioClip>();

            foreach (var audioClip in _audioClips)
            {
                if (Enum.TryParse(audioClip.name, out AudioTypes audioType))
                {
                    _audioClipsDic[audioType] = audioClip;
                }
                else
                {
                    throw new MissingMemberException($"Audio Clip {audioClip.name} Naming Wrong.");
                }
            }
            
            PlayMainSound();
        }

        private void PlayMainSound()
        {
            _mainSoundSource.clip = _audioClipsDic[AudioTypes.MainMusic];
            _mainSoundSource.Play();
        }

        public void PlaySoundEffect(AudioSource audioSource,AudioTypes audioType)
        {
            audioSource.clip = _audioClipsDic[audioType];
            audioSource.Play();
        }

        public void StopSoundEffect(AudioSource audioSource)
        {
            audioSource.Stop();
        }
        public void PauseSoundEffect(AudioSource audioSource)
        {
            _mainSoundSource.Pause();
        }
        public void ContinueSoundEffect(AudioSource audioSource)
        {
            _mainSoundSource.Play();
        }
    }
}