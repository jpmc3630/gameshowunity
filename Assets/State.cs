using System;
using System.Threading.Tasks;
using PusherClient;
using UnityEngine;
using UnityEngine.UI;
using BestHTTP;

using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
// using System.Random;

// using Newtonsoft.Json;
// using static RenderScript;

public sealed class State
{
    public string baseUrl = "https://da34-61-245-129-196.au.ngrok.io";
    public string token = null;
    public CountdownScript countdownScript = GameObject.Find("Canvas").GetComponent<CountdownScript>();
    public RenderScript renderScript = GameObject.Find("Canvas").GetComponent<RenderScript>();
    public MenuScript menuScript = GameObject.Find("Canvas").GetComponent<MenuScript>();
    public GameObject Canvas;
    public Question question;
    public string currentScreen;

    // public int playersAnswered = 0;
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
      renderScript.redrawPlayerList();
    }

    public void removePlayerById(String id) {
      for (int i=0; i< instance.playerList.Count; i++) {
        if (instance.playerList[i].Id == id) {
          instance.playerList.RemoveAt(i);
          break;
        }
      }
      renderScript.redrawPlayerList();
    }

    public async void startGame() {

        // Hide the lobby
        menuScript.closeAllPanels();

        // fetch a question
        question = new Question();
        Debug.Log("Fetching question...");
        try {
            var request = new HTTPRequest(new Uri(State.Instance.baseUrl + "/api/question"));
            request.SetHeader("Accept", "application/json");
            request.SetHeader("Authorization", "Bearer " + State.Instance.token);
            question = await request.GetFromJsonResultAsync<Question>();
            Debug.Log("Question: " + question.question);
        } catch(Exception ex)
        {
            Debug.Log("Question Fetch Failed:");
            Debug.LogException(ex);
        }
        
        // shuffle the question
        string[] myArray = {
            question.answer,
            question.first_incorrect,
            question.second_incorrect
        };
        System.Random rnd=new System.Random();
        rnd.Shuffle(myArray);
        question.shuffled = myArray;
        
        // find the position of correct answer
        string[] positions = {"A", "B", "C"};
        question.answerPosition = positions[Array.FindIndex(myArray, x => x.Contains(question.answer))];

        // Show the QUESTION
        renderScript.drawQuestion(question);

        // set all players answers to null
        for (int i=0; i< instance.playerList.Count; i++) {
          instance.playerList[i].Answer = null;
        }
        
        // Tell players to show question screen
        FunctionTimer.Create(() => PusherManager.instance.PlayerMode("buttons", ""), 6f);

        // Listen for answers
        // this is handled in pusher manager bind event ...
        // if flag currentScreen == "question" then call setPlayerAnswer with player id and answer
    }

    public void showAnswers() {
        // When timer expires OR everyone has answered, show the ANSWER and RESULTS screen

        // Tell players to show answer screen
        PusherManager.instance.PlayerMode("answer", question.answerPosition);
        Debug.Log("Show Answers");

        instance.currentScreen = "answer";

        // TODO: Show the answer screen here + show who got it right and wrong
        // OTHER TODO:
        // keep scores, show results, etc.
        // enter player name
        // choose player avatar
        // sound fx manager
        // settings screen

        countdownScript.Begin(10, true, State.Instance.startGame);
    }

    public void setPlayerAnswer(String user_id, String answer) {
      // if timer isn't running, start it
      if (!countdownScript.isCountingDown) {
          countdownScript.Begin(5, true, State.Instance.showAnswers); // TODO: move these constants into a settings folder
      }

      // set the player's answer
      for (int i=0; i< instance.playerList.Count; i++) {
        if (instance.playerList[i].Id == user_id && instance.playerList[i].Answer == null) {
          instance.playerList[i].Answer = answer;
          Debug.Log("Player " + instance.playerList[i].Name + " answered: " + instance.playerList[i].Answer);
          break;
        }
      }
      
      question.playersAnswered++;
      if (question.playersAnswered >= instance.playerList.Count) {
        // everyone has answered
        Debug.Log("Everyone has answered");
        countdownScript.SkipToTheEnd();
      }
    }

}



public class Player
{
    public string Name { get; set; }
    public string Id { get; set; }
    public int Score { get; set; }
    public string Answer { get; set; }
}


public class Question
{
    public int id { get; set; }
    public string question { get; set; }
    public string answer { get; set; }
    public string first_incorrect { get; set; }
    public string second_incorrect { get; set; }
    public string category { get; set; }
    public string[] shuffled {get; set;}
    public string answerPosition { get; set; }
    public int playersAnswered = 0;
}

static class RandomExtensions
{
    public static void Shuffle<T> (this System.Random rng, T[] array)
    {
        int n = array.Length;
        while (n > 1) 
        {
            int k = rng.Next(n--);
            T temp = array[n];
            array[n] = array[k];
            array[k] = temp;
        }
    }
}