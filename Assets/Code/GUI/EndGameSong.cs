using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Owls; 

public class EndGameSong : MonoBehaviour
{

    AudioSource endsource;

    // Start is called before the first frame update
    void Start()
    {
        endsource = GetComponent<AudioSource>();
        endsource.enabled = false; 
    }

    // Update is called once per frame
    void Update()
    {
        GameObject timerObject = GameObject.FindGameObjectWithTag("TimeKeeper");
        TimeKeeper timeKeeper = timerObject.GetComponent<TimeKeeper>();
        int seconds = timeKeeper.TimeRemaining;   

                
        if (seconds <= 10)
        {

            endsource.enabled = true; 

        }

        
        
    }

}
