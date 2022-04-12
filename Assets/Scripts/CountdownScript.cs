using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

 public class CountdownScript : MonoBehaviour {
    public TextMeshProUGUI CountdownText;
     public int duration = 5;
    //  public string purpose = "question";
    //  private void countdownCallback;

    public delegate void countdownDelegate(); // This defines what type of method you're going to call.
    public countdownDelegate m_methodToCall; // This is the variable holding the method you're going to call.

    public int timeRemaining;
    public bool isCountingDown = false;
    //  public void Begin(int duration, string purpose)
    public void Begin(int duration, countdownDelegate myCallback)
     {
        if (!isCountingDown) {
            this.m_methodToCall = myCallback;
            this.duration = duration;
            // this.purpose = purpose;
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
            // if (this.purpose == "question") {
            //     Debug.Log("Question time is up");
            //     State.Instance.showAnswers();
            // } else if (this.purpose == "answer") {
            //     Debug.Log("Answer time is up");
            //     State.Instance.startGame();
            // }
            
        }
    }
 }