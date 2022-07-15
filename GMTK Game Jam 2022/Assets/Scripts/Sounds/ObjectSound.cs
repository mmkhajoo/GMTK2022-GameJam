using System;
using System.Collections;
using System.Collections.Generic;
using Managers.Audio_Manager;
using Objects;
using UnityEngine;

public class ObjectSound : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private ObjectType objectType;
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == 7)
        {
            if(objectType==ObjectType.Metal) 
                AudioManager.instance.PlaySoundEffect(audioSource,AudioTypes.MetalImpact);
            else if(objectType==ObjectType.Forceable)
                AudioManager.instance.PlaySoundEffect(audioSource,AudioTypes.PaperImpact);
        }
    }
}
