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
    [SerializeField] private Transform scoreList;
    public TextMeshProUGUI QuestionText;
    public TextMeshProUGUI A_AnswerText;
    public TextMeshProUGUI B_AnswerText;
    public TextMeshProUGUI C_AnswerText;
    public TextMeshProUGUI A_TitleText;
    public TextMeshProUGUI B_TitleText;
    public TextMeshProUGUI C_TitleText;
    

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


    public void redrawScoreboard() {
        Debug.Log("redrawScoreboard func...");
        State.Instance.menuScript.closeAllPanels();
        // clear the list
        foreach (Transform child in scoreList) {
            GameObject.Destroy(child.gameObject);
        }
        // for each player in state, draw their name
        for (int i=0; i< State.Instance.playerList.Count; i++) {
            Debug.Log(State.Instance.playerList[i].Id + " - " + State.Instance.playerList[i].Name + State.Instance.playerList[i].Score);

            GameObject newMessage = (GameObject)Instantiate(playerPrefab, scoreList.transform.position + new Vector3(0.0f, -200.0f * i, 0.0f), playersList.transform.rotation);
            newMessage.transform.SetParent(scoreList);
            newMessage.transform.SetSiblingIndex(scoreList.childCount - 2);

            Debug.Log("playerList.childCount: " + scoreList.childCount);
            var texts = newMessage.GetComponentsInChildren<Text>();
            Debug.Log(texts);
            if (texts != null
                && texts.Length > 0)
            {
                texts[0].text = State.Instance.playerList[i].Name + " - " + State.Instance.playerList[i].Score;
            }
        }

        State.Instance.menuScript.showScoreboardPanel();
    }

    public void drawQuestion(Question question) {
        QuestionText.enabled = false;
        A_AnswerText.enabled = A_TitleText.enabled = false;
        B_AnswerText.enabled = B_TitleText.enabled = false;
        C_AnswerText.enabled = C_TitleText.enabled = false;
        A_AnswerText.color = A_TitleText.color = new Color32(255, 255, 255, 255);
        B_AnswerText.color = B_TitleText.color = new Color32(255, 255, 255, 255);
        C_AnswerText.color = C_TitleText.color = new Color32(255, 255, 255, 255);

        State.Instance.menuScript.showQuestionPanel();
        QuestionText.text = question.question;
        A_AnswerText.text = question.shuffled[0];
        B_AnswerText.text = question.shuffled[1];
        C_AnswerText.text = question.shuffled[2];
        
        FunctionTimer.Create(() => {
            SoundManager.Instance.Play(SoundManager.Instance.EffectsSource.clip); 
            QuestionText.enabled = true;
        }, 1f);
        FunctionTimer.Create(() => {
          A_AnswerText.enabled = A_TitleText.enabled = true;
          SoundManager.Instance.Play(SoundManager.Instance.Swoosh1.clip);
         }, 4f);
        FunctionTimer.Create(() => {
          B_AnswerText.enabled = B_TitleText.enabled = true;
          SoundManager.Instance.Play(SoundManager.Instance.Swoosh2.clip);
        }, 5f);
        FunctionTimer.Create(() => {
          SoundManager.Instance.Play(SoundManager.Instance.Swoosh3.clip);
          C_AnswerText.enabled = C_TitleText.enabled = true;
        }, 6f);
    }
}
