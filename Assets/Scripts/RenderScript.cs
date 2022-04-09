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

    public TextMeshProUGUI QuestionText;
    public TextMeshProUGUI A_AnswerText;
    public TextMeshProUGUI B_AnswerText;
    public TextMeshProUGUI C_AnswerText;
    void Start()
    {

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
        QuestionText.text = question.question;
        A_AnswerText.text = question.answer;
        B_AnswerText.text = question.first_incorrect;
        C_AnswerText.text = question.second_incorrect;
        GameObject.Find("Canvas").GetComponent<MenuScript>().showQuestionPanel();
    }


}
