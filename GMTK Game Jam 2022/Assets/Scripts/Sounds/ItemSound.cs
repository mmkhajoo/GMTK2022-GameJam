using System.Collections;
using System.Collections.Generic;
using Items;
using Managers.Audio_Manager;
using UnityEngine;

public class ItemSound : MonoBehaviour
{
    [SerializeField] private AudioSource activateAudioSource;

    public void OnActivate(ItemType itemType)
    {
        if(itemType==ItemType.Magnet)
            AudioManager.instance.PlaySoundEffect(activateAudioSource,AudioTypes.MagnetActivate);
        else if(itemType==ItemType.Baloons)
            AudioManager.instance.PlaySoundEffect(activateAudioSource,AudioTypes.BalloonActivate);
    }
    public void OnDeActivate()
    {
        AudioManager.instance.StopSoundEffect(activateAudioSource);
    }
}
