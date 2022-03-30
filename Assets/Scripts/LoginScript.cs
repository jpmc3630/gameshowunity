using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BestHTTP;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[Serializable]
public class userObject
{
    public String email;
    public String token;
    public String name;
    public int id;

}

[Serializable]
public class registrationObject
{
    public String token;
    public userObject user;
}

[Serializable]
public class responseObject
{
    public String message;
    public errorsObject errors;
}

[Serializable]
public class errorsObject
{
    public String[] email;
    public String[] password;
}

public class LoginScript : MonoBehaviour
{
    // [SerializeField] private PusherManager pusherScript;
    private userObject user = new userObject();
    public Text MessageText;
    public Text UserText;
    public InputField EmailField;
    public InputField PasswordField;
    // public GameObject LogoutButton;
    public GameObject LoginPanel;
    public GameObject SettingsPanel;
    public GameObject MenuPanel;
    public GameObject LobbyPanel;
    public GameObject StartGameButton;
    EventSystem m_EventSystem;
    
    public const string baseUrl = "https://ddb2-61-245-129-196.au.ngrok.io";
    
    // Start is called before the first frame update
    void Start()
    {
        m_EventSystem = EventSystem.current;
        this.getUser();
    }

    public void LoginButton() {
        Debug.Log("Login");
        HTTPRequest request = new HTTPRequest(new Uri(baseUrl + "/api/login"), HTTPMethods.Post, OnLoginRequestFinished);
        request.SetHeader("Accept", "application/json");
        request.AddField("email", EmailField.text);
        request.AddField("password", PasswordField.text);
        request.AddField("device_name", SystemInfo.deviceModel);
        request.Send();
    }

    public void RegisterAndLoginButton() {
        Debug.Log("Register and Login");
        HTTPRequest request = new HTTPRequest(new Uri(baseUrl + "/api/register"), HTTPMethods.Post, OnRegisterRequestFinished);
        request.SetHeader("Accept", "application/json");
        request.AddField("email", EmailField.text);
        request.AddField("password", PasswordField.text);
        request.AddField("device_name", SystemInfo.deviceModel);
        request.Send();
    }

    public void ForgotPasswordButton() {
        Debug.Log("Forgot Password Button");
    }

    public void getUser() {
        Debug.Log("Checking for stored token...");
        if (PlayerPrefs.HasKey("user")) {
            String userString = PlayerPrefs.GetString("user", null);
            Debug.Log("userString: " + userString);
            if (userString != null) {
                user = JsonUtility.FromJson<userObject>(userString);
                EmailField.text = user.email;
                this.authenticateUser(user.token);
            }
        } else {
            LoginPanel.SetActive(true);
            m_EventSystem.SetSelectedGameObject(null);
            m_EventSystem.SetSelectedGameObject(EmailField.gameObject);
        }
    }

    public Boolean isLoggedIn () {
        Debug.Log("isLoggedIn running...");
        if (PlayerPrefs.HasKey("user")) {
            String userString = PlayerPrefs.GetString("user", null);
            if (userString != null) {
                user = JsonUtility.FromJson<userObject>(userString);
                if (user.token != null) {
                    return true;
                }
            }
        }
        return false;
    }

    public void authenticateUser(String token) {
        Debug.Log("Authenticating user...");
        HTTPRequest request = new HTTPRequest(new Uri(baseUrl + "/api/user"), onAuthRequestFinished);
        request.SetHeader("Accept", "application/json");
        request.SetHeader("Authorization", "Bearer " + token);
        request.Send();
    }

    public async void storeUser(userObject user) {
        Debug.Log("Storing User:");
        String json = JsonUtility.ToJson(user);
        PlayerPrefs.SetString("user", json);
        UserText.text = user.email;
        MessageText.text = "";
        DumpToConsole(user);
        // LogoutButton.SetActive(true);
        LoginPanel.SetActive(false);
        LobbyPanel.SetActive(true);
        m_EventSystem.SetSelectedGameObject(null);
        m_EventSystem.SetSelectedGameObject(StartGameButton.gameObject);

        PusherManager.instance.token = user.token;
        PusherManager.instance.baseUrl = baseUrl;
        await PusherManager.instance.InitialisePusher();

    }
    
    public async void logout() {
        Debug.Log("Logging out!");
        PlayerPrefs.DeleteKey("user");
        UserText.text = "Not currently logged in!";
        SettingsPanel.SetActive(false);
        MenuPanel.SetActive(false);
        LoginPanel.SetActive(true);
        m_EventSystem.SetSelectedGameObject(null);
        m_EventSystem.SetSelectedGameObject(EmailField.gameObject);
        await PusherManager.instance.OnApplicationQuit();
    }

    void showError(String responseText) {
          responseObject resp = new responseObject();
          resp = JsonUtility.FromJson<responseObject>(responseText);
          if (resp.errors != null) {
              String errorMessage = ""; 
              if (resp.errors.email != null) {
                  errorMessage += string.Join("\n", resp.errors.email) + "\n";
              }
              if (resp.errors.password != null) {
                  errorMessage += string.Join("\n", resp.errors.password);
                  errorMessage = errorMessage.Replace("The password must be between 9 and 255 characters.\n", "");
                  errorMessage = errorMessage.Replace("The password must be at least 9 characters.\nThe password must contain at least one number", "The password must be at least 9 characters and contain at least one number");
              }
              MessageText.text = errorMessage;
          }
    }
    void OnLoginRequestFinished(HTTPRequest request, HTTPResponse response)
    {
      Debug.Log("Login Response recieved: " + response.DataAsText);
      if (response.StatusCode != 200) {
          this.showError(response.DataAsText);
          return;
      }
          user.token = response.DataAsText;
          this.authenticateUser(user.token);
    }
    void OnRegisterRequestFinished(HTTPRequest request, HTTPResponse response)
    {
        Debug.Log("Register response receieved: " + response.DataAsText);
        if (response.StatusCode != 200) {
            this.showError(response.DataAsText);
            LoginPanel.SetActive(true);
            return;
        }

        registrationObject registered = JsonUtility.FromJson<registrationObject>(response.DataAsText);
        registered.user.token = registered.token;
        user = registered.user;
        this.storeUser(user);
    }

    void onAuthRequestFinished(HTTPRequest request, HTTPResponse response)
    {
      Debug.Log("Auth response: " + response.DataAsText);
      if (response.StatusCode == 200) {
          Debug.Log("Successfully authenticated user");
          String tokenTemp = user.token;
          user = JsonUtility.FromJson<userObject>(response.DataAsText);
          user.token = tokenTemp;
          this.storeUser(user);
      } else {
          Debug.Log("Failed to authenticate user - Error: " + response.StatusCode);
          LoginPanel.SetActive(true);
          m_EventSystem.SetSelectedGameObject(null);
          m_EventSystem.SetSelectedGameObject(EmailField.gameObject);
      }
    }

    public static void DumpToConsole(object obj)
    {
        var output = JsonUtility.ToJson(obj, true);
        Debug.Log(output);
    }


 
    // public void enablePusherScript(token) {
        // pusherScript.PusherManagerStart("12|vSN7TDH6vtNVR0UsL3XBk8gAZxifCdZVwfHYcnAD");
    // }

    // public void disablePusherScript() {
    //     pusherScript.enabled = false;
    // }

}