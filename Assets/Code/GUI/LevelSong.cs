using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Owls; 

public class LevelSong : MonoBehaviour
{

    AudioSource levelSource;
    

    // Start is called before the first frame update
    void Start()
    {
        levelSource = GetComponent<AudioSource>();
        levelSource.enabled = true; 
        levelSource.loop = true;
    }

    // Update is called once per frame
    void Update()
    {
        GameObject timerObject = GameObject.FindGameObjectWithTag("TimeKeeper");
        TimeKeeper timeKeeper = timerObject.GetComponent<TimeKeeper>();
        int seconds = timeKeeper.TimeRemaining;   

                
        if (seconds <= 10)
        {

            levelSource.enabled = false; 

        }

        
        
    }

}

