using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Owls; 





public class EndGamePitchModifier : MonoBehaviour
{

    float currentPitch = 1.0f;
    [SerializeField] float speedFactor = 0.1f;
    [SerializeField] float speedThreshold = 1.05f;
    [SerializeField] float timeThreshold = 20f;
    // Start is called before the first frame update
    void Start()
    {   
        
    }

    // Update is called once per frame
    void Update()
    {
        GameObject timerObject = GameObject.FindGameObjectWithTag("TimeKeeper");
        TimeKeeper timeKeeper = timerObject.GetComponent<TimeKeeper>();
        int secondsLeft = timeKeeper.TimeRemaining;   

        
        if (secondsLeft < timeThreshold && secondsLeft > 1)
        {

            AudioMixer mixer = Resources.Load("MainMixer") as AudioMixer;
            mixer.SetFloat("MusicPitch", speedThreshold*currentPitch + speedFactor/secondsLeft);

        }

        else if (secondsLeft <= 1)
        {

            AudioMixer mixer = Resources.Load("MainMixer") as AudioMixer;
            mixer.SetFloat("MusicPitch", currentPitch + speedFactor);

        }

        
    }
}
