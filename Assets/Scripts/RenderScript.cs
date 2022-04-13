using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using UnityEngine.UI;
using TMPro;

public class RenderScript : MonoBehaviour
{
    
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform playersList;
    public CountdownScript countdownScript;
    public TextMeshProUGUI QuestionText;
    public TextMeshProUGUI A_AnswerText;
    public TextMeshProUGUI B_AnswerText;
    public TextMeshProUGUI C_AnswerText;
    public TextMeshProUGUI A_TitleText;
    public TextMeshProUGUI B_TitleText;
    public TextMeshProUGUI C_TitleText;
    void Start()
    {
        countdownScript = GameObject.Find("Canvas").GetComponent<CountdownScript>();
    }

    public void redrawPlayerList() {
        // clear the list
        foreach (Transform child in playersList) {
            GameObject.Destroy(child.gameObject);
        }
        // for each player in state, draw their name
        for (int i=0; i< State.Instance.playerList.Count; i++) {
            Debug.Log(State.Instance.playerList[i].Id + " - " + State.Instance.playerList[i].Name);

            GameObject newMessage = (GameObject)Instantiate(playerPrefab, playersList.transform.position + new Vector3(0.0f, -200.0f * i, 0.0f), playersList.transform.rotation);
            newMessage.transform.SetParent(playersList);
            newMessage.transform.SetSiblingIndex(playersList.childCount - 2);

            Debug.Log("playersList.childCount: " + playersList.childCount);
            var texts = newMessage.GetComponentsInChildren<Text>();
            Debug.Log(texts);
            if (texts != null
                && texts.Length > 0)
            {
                texts[0].text = State.Instance.playerList[i].Name;
            }
        }
    }

    public void drawQuestion(Question question) {
        QuestionText.enabled = false;
        A_AnswerText.enabled = false;
        B_AnswerText.enabled = false;
        C_AnswerText.enabled = false;
        A_TitleText.enabled = false;
        B_TitleText.enabled = false;
        C_TitleText.enabled = false;
        State.Instance.menuScript.showQuestionPanel();
        QuestionText.text = question.question;
        A_AnswerText.text = question.shuffled[0];
        B_AnswerText.text = question.shuffled[1];
        C_AnswerText.text = question.shuffled[2];
        countdownScript.Begin(1, false, RenderQuestion);
    }

    // daisy chaining these timed calls is pretty wild, but damn it works good!
    // Alternative is something thread blocking like ... System.Threading.Thread.Sleep(50);
    public void RenderQuestion() {
        QuestionText.enabled = true; 
        countdownScript.Begin(3, false, RenderA);
    }
    public void RenderA() {
        A_AnswerText.enabled = true;
        A_TitleText.enabled = true;
        countdownScript.Begin(1, false, RenderB);
    }
    public void RenderB() {
        B_AnswerText.enabled = true;
        B_TitleText.enabled = true;
        countdownScript.Begin(1, false, RenderC);
    }
    public void RenderC() {
        C_TitleText.enabled = true;
        C_AnswerText.enabled = true;
    }


}
