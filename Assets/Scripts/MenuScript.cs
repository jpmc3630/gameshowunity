using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuScript : MonoBehaviour
{
    EventSystem m_EventSystem;
    public GameObject Canvas;
    public GameObject StartButton;
    public GameObject LobbyPanel;
    public GameObject SettingsPanel;
    public GameObject LoadingPanel;
    public GameObject QuestionPanel;
    public GameObject MenuPanel;
    // public GameObject LogoutButton;
    public GameObject StartGameButton;
    public GameObject LoginPanel;
    public GameObject EmailField;
    public GameObject BackToMenuButton;
    public GameObject ResumeGameButton;
    // public ScriptName Pusher = PusherGameObject.GetComponent<PusherManager>();
    void Start()
    {
        m_EventSystem = EventSystem.current;
    }

    public void closeAllPanels() {
        MenuPanel.SetActive(false);
        LoginPanel.SetActive(false);
        LobbyPanel.SetActive(false);
        SettingsPanel.SetActive(false);
        LoadingPanel.SetActive(true);
    }

    public void MenuButtonClick() {
        Debug.Log("MenuButtonClick");
        this.closeAllPanels();
        MenuPanel.SetActive(true);
        UnityEngine.tvOS.Remote.allowExitToHome = true;
        m_EventSystem.SetSelectedGameObject(null);
        m_EventSystem.SetSelectedGameObject(ResumeGameButton.gameObject);

    }

    public void BackToMenuButtonClick() {
        Debug.Log("BackToMenuButtonClick");
        // SettingsPanel.SetActive(false);
        this.closeAllPanels();
        MenuPanel.SetActive(true);
        UnityEngine.tvOS.Remote.allowExitToHome = true;
        m_EventSystem.SetSelectedGameObject(null);
        m_EventSystem.SetSelectedGameObject(ResumeGameButton.gameObject);
    }

    public void SettingsButtonClick() {
        Debug.Log("SettingsButtonClick");
        this.closeAllPanels();
        SettingsPanel.SetActive(true);
        UnityEngine.tvOS.Remote.allowExitToHome = false;
        m_EventSystem.SetSelectedGameObject(null);
        m_EventSystem.SetSelectedGameObject(BackToMenuButton.gameObject);
    }

    public void ResumeGameButtonClick() {
        Debug.Log("ResumeGameButton");
        UnityEngine.tvOS.Remote.allowExitToHome = false;
        // MenuPanel.SetActive(false);
        this.closeAllPanels();
        if (Canvas.GetComponent<LoginScript>().isLoggedIn()) {
            LobbyPanel.SetActive(true);
            m_EventSystem.SetSelectedGameObject(null);
            m_EventSystem.SetSelectedGameObject(StartGameButton.gameObject);
        } else {
            LoginPanel.SetActive(true);
            m_EventSystem.SetSelectedGameObject(null);
            m_EventSystem.SetSelectedGameObject(EmailField.gameObject);
        }
    }

    public void showQuestionPanel() {
        State.Instance.currentScreen = "question";
        LoadingPanel.SetActive(false);
        QuestionPanel.SetActive(true);
    }

    public void StartGameButtonClick() {
        // play the music
        SoundManager.Instance.PlayMusic(SoundManager.Instance.MusicSource.clip);
        State.Instance.startGame();
    }

}
