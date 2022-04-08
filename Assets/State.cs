using System;
using System.Threading.Tasks;
using PusherClient;
using UnityEngine;
using UnityEngine.UI;
using BestHTTP;

using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

// using Newtonsoft.Json;
// using static RenderScript;

public sealed class State
{
    // public bool testVar { get; set; }
    public GameObject Canvas;
    public string currentScreen;

    public string baseUrl = "https://fdac-61-245-129-196.au.ngrok.io";
    public string token = null;

    public List<Player> playerList = new List<Player> ();
    private static object _lockObj = new object();
    private static State instance;
    public static State Instance

    {
        get
        {
            //only request the access lock if it's null to begin with
            if (instance == null)
            {
                //obtain the access lock to ensure only one thread gets in at a time
                lock(_lockObj)
                {
                    //check again to see if it's null so if a previous thread created it, don't do it again
                    if(instance == null)
                    {
                        instance = new State();
                        
                    }
                }
            }
           
            return instance;
        }
    }
 
    public void addPlayerToPlayerList(Player newPlayer) {
      Debug.Log("New Player Joined: " + newPlayer.Name);
      instance.playerList.Add(newPlayer);
      GameObject.Find("Canvas").GetComponent<RenderScript>().redrawPlayerList();
    }

    public void removePlayerById(String id) {
      for (int i=0; i< instance.playerList.Count; i++) {
        if (instance.playerList[i].Id == id) {
          instance.playerList.RemoveAt(i);
          break;
        }
      }
      GameObject.Find("Canvas").GetComponent<RenderScript>().redrawPlayerList();
    }

    public async void startGame() {

        // Hide the lobby
        GameObject.Find("Canvas").GetComponent<MenuScript>().closeAllPanels();

        // fetch a question
        Debug.Log("Requesting create room...");
        var request = new HTTPRequest(new Uri(State.Instance.baseUrl + "/api/question"));
        request.SetHeader("Accept", "application/json");
        request.SetHeader("Authorization", "Bearer " + State.Instance.token);
        
        try {
            string result = await request.GetAsStringAsync();
            Debug.Log("Question: " + result);
            // Room = result;

        } catch(Exception ex)
        {
            Debug.Log("Create room request failed:");
            Debug.LogException(ex);
        }
        // Show the QUESTION screen

        // Listen for answers

        // Start the timer on first answer recieved

        // When timer expires OR everyone has answered, show the ANSWER and RESULTS screen

    }
}



public class Player
{
    public string Name { get; set; }
    public string Id { get; set; }
    public int Score { get; set; }
}
