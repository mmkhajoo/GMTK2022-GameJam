using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimeLineManager : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;

    private bool _isDirectorPlayed;
    
    public void PlayDirectorOneTime()
    {
        if(_isDirectorPlayed)
            return;

        _isDirectorPlayed = true;
        director.Play();
    }
}
