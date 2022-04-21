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
using TMPro;
// using Newtonsoft.Json;
// using static RenderScript;

public sealed class State
{
    public string baseUrl = "https://4ad3-61-245-129-196.au.ngrok.io";
    public string token = null;
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

        // play the music
        SoundManager.Instance.PlayMusic(SoundManager.Instance.MusicSource.clip);

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

        TextMeshProUGUI A_Title = GameObject.Find("A_TitleText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI B_Title = GameObject.Find("B_TitleText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI C_Title = GameObject.Find("C_TitleText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI A_Text = GameObject.Find("A_AnswerText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI B_Text = GameObject.Find("B_AnswerText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI C_Text = GameObject.Find("C_AnswerText").GetComponent<TextMeshProUGUI>();

        A_Text.color = A_Title.color = question.answerPosition == "A" ? new Color32(0, 255, 0, 255) : new Color32(255, 0, 0, 255);
        B_Text.color = B_Title.color = question.answerPosition == "B" ? new Color32(0, 255, 0, 255) : new Color32(255, 0, 0, 255);
        C_Text.color = C_Title.color = question.answerPosition == "C" ? new Color32(0, 255, 0, 255) : new Color32(255, 0, 0, 255);

        // TODO: Show the answer screen here + show who got it right and wrong
        // OTHER TODO:
        // keep scores, show results, etc.
        // enter player name
        // choose player avatar
        // sound fx manager
        // settings screen

        FunctionTimer.Create(() => State.Instance.startGame(), 10f, "", true);
    }

    public void setPlayerAnswer(String user_id, String answer) {
      // if timer isn't running, start it
      if (question.playersAnswered == 0) {
          FunctionTimer.Create(() => State.Instance.showAnswers(), 5f, "Timeout", true);
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
        FunctionTimer.StopAllTimersWithName("Timeout");
        State.Instance.showAnswers();
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