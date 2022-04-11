using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

 public class CountdownScript : MonoBehaviour {
    public TextMeshProUGUI CountdownText;
     public int duration = 5;
     public int timeRemaining;
     public bool isCountingDown = false;
     public void Begin()
     {
         if (!isCountingDown) {
           Debug.Log("Start timer");
             isCountingDown = true;
             timeRemaining = duration;
             CountdownText.text = timeRemaining.ToString();
             Invoke ( "_tick", 1f );
         }
     }
 
     private void _tick() {
         timeRemaining--;
         CountdownText.text = timeRemaining.ToString();
         if(timeRemaining > 0) {
            Invoke ( "_tick", 1f );
         } else {
            isCountingDown = false;
            State.Instance.showAnswers();
         }
     }
 }