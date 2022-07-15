using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class StatueTrigger : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            director.Play();
            gameObject.SetActive(false);
        }
    }
}
