using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

 public class CountdownScript : MonoBehaviour {
    public TextMeshProUGUI CountdownText;
    public delegate void countdownDelegate(); // This defines what type of method you're going to call.
    public countdownDelegate m_methodToCall; // This is the variable holding the method you're going to call.
    public int timeRemaining;
    public bool isCountingDown = false;
    public void Begin(int duration, countdownDelegate onExpiryCallback)
     {
        if (!isCountingDown) {
            this.m_methodToCall = onExpiryCallback; // Like a little constructor for our callback delegate
            timeRemaining = duration;
            Debug.Log("Start timer");
            isCountingDown = true;
            timeRemaining = duration;
            CountdownText.text = timeRemaining.ToString();
            Invoke ( "_tick", 1f );
        }
    }
 
    private void _tick() {
        timeRemaining--;
        Debug.Log(timeRemaining);
        CountdownText.text = timeRemaining.ToString();
        if(timeRemaining > 0) {
            Invoke ( "_tick", 1f );
        } else {
            CountdownText.text = "";
            isCountingDown = false;
            this.m_methodToCall();
        }
    }
 }