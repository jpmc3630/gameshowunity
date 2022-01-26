using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static LoginScript;

public class MenuScript : MonoBehaviour
{
    public GameObject SettingsPanel;
    public GameObject MenuPanel;
    public GameObject LogoutButton;
    public GameObject LoginPanel;
    public GameObject BackToMenuButton;
    public GameObject ResumeGameButton;
    EventSystem m_EventSystem;
    void Start()
    {
        m_EventSystem = EventSystem.current;
        MenuPanel.SetActive(false);
        // LoginPanel.SetActive(false);
        LogoutButton.SetActive(false);
        SettingsPanel.SetActive(false);

        // LoginScript.isLoggedIn();
    }

    public void MenuButtonClick() {
        Debug.Log("MenuButtonClick");
        MenuPanel.SetActive(true);
        m_EventSystem.SetSelectedGameObject(null);
        m_EventSystem.SetSelectedGameObject(ResumeGameButton.gameObject);
    }

    public void BackToMenuButtonClick() {
        Debug.Log("BackToMenuButtonClick");
        SettingsPanel.SetActive(false);
        MenuPanel.SetActive(true);
        m_EventSystem.SetSelectedGameObject(null);
        m_EventSystem.SetSelectedGameObject(ResumeGameButton.gameObject);
    }

    public void SettingsButtonClick() {
        Debug.Log("SettingsButtonClick");
        SettingsPanel.SetActive(true);
        m_EventSystem.SetSelectedGameObject(null);
        m_EventSystem.SetSelectedGameObject(BackToMenuButton.gameObject);
    }

    public void ResumeGameButtonClick() {
        Debug.Log("ResumeGameButton");
        MenuPanel.SetActive(false);
    }
}
