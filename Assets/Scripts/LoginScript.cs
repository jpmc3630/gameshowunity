using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BestHTTP;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LoginScript : MonoBehaviour
{
    private userObject user = new userObject();
    public Text Message;
    public Text StorageText;
    public InputField EmailField;
    public InputField Password;
    public GameObject Login;
    public GameObject LogoutButton;
    public GameObject RawImage;
    EventSystem m_EventSystem;
    
    private const string baseUrl = "https://d9f8-61-245-129-196.au.ngrok.io";
    
    // Start is called before the first frame update
    void Start()
    {
        isLoggedIn();
        m_EventSystem = EventSystem.current;
        // m_EventSystem.SetSelectedGameObject(null);
        RawImage.SetActive(false);

    }

    public void LoginButton() {
        Debug.Log("Login");
        HTTPRequest request = new HTTPRequest(new Uri(baseUrl + "/api/login"), HTTPMethods.Post, OnLoginRequestFinished);
        request.SetHeader("Accept", "application/json");
        request.AddField("email", EmailField.text);
        request.AddField("password", Password.text);
        request.AddField("device_name", SystemInfo.deviceModel);
        request.Send();

    }

    public void RegisterAndLoginButton() {
        Debug.Log("Register and Login");
        HTTPRequest request = new HTTPRequest(new Uri(baseUrl + "/api/register"), HTTPMethods.Post, OnRegisterRequestFinished);
        request.SetHeader("Accept", "application/json");
        request.AddField("email", EmailField.text);
        request.AddField("password", Password.text);
        request.AddField("device_name", SystemInfo.deviceModel);
        request.Send();
    }

    public void ForgotPasswordButton() {
        Debug.Log("Forgot Password Button");
    }

    public void isLoggedIn() {
        Debug.Log("Checking for stored token...");

        if (PlayerPrefs.HasKey("user")) {
            String userString = PlayerPrefs.GetString("user", null);
            Debug.Log("userString: " + userString);
            if (userString != null) {
                user = JsonUtility.FromJson<userObject>(userString);
                EmailField.text = user.email;
                this.authenticateUser(user.token);
            }
        }
    }

    public void authenticateUser(String token) {
        Debug.Log("Device model: " + SystemInfo.deviceModel);

        HTTPRequest request = new HTTPRequest(new Uri(baseUrl + "/api/user"), onAuthRequestFinished);
        request.SetHeader("Accept", "application/json");
        request.SetHeader("Authorization", "Bearer " + token);
        request.Send();
    }

    public void storeUser(userObject user) {
        Debug.Log("Setting String");
        String json = JsonUtility.ToJson(user);
        PlayerPrefs.SetString("user", json);

        // and hide the login panel i guess!
    }
    
    public void logout() {
        Debug.Log("Logging out!");
        PlayerPrefs.DeleteKey("user");
        RawImage.SetActive(true);
    }

    void OnLoginRequestFinished(HTTPRequest request, HTTPResponse response)
    {
      Debug.Log("Request Finished! Text received: " + response.DataAsText);
      user.token = response.DataAsText;
      Message.text = user.token;
      this.authenticateUser(user.token);
    }
    void OnRegisterRequestFinished(HTTPRequest request, HTTPResponse response)
    {
      Debug.Log("Register finished - Text received: " + response.DataAsText);
      
      if (response.StatusCode == 200) {
          Debug.Log("Successfully registered user");
          
          registrationObject registered = JsonUtility.FromJson<registrationObject>(response.DataAsText);
          registered.user.token = registered.token;
          user = registered.user;
          this.storeUser(user);
          
          // hide the login screen
          RawImage.SetActive(false);
      } else {
          Debug.Log("Failed to authenticate user - Error: " + response.StatusCode);
          RawImage.SetActive(true);
      }
    }

    void onAuthRequestFinished(HTTPRequest request, HTTPResponse response)
    {
      Debug.Log("onAuthRequestFinished! Text received: " + response.DataAsText);
      Message.text = response.DataAsText;

      if (response.StatusCode == 200) {
          Debug.Log("Successfully authenticated user");
          
          String tokenTemp = user.token;
          user = JsonUtility.FromJson<userObject>(response.DataAsText);
          user.token = tokenTemp;
          Debug.Log("------------------------------------");
          DumpToConsole(user);
          Debug.Log("------------------------------------");
          
          this.storeUser(user);
          
          // hide the login screen
          RawImage.SetActive(false);
      } else {
          Debug.Log("Failed to authenticate user - Error: " + response.StatusCode);
          RawImage.SetActive(true);
      }
    }

    // public void selectPasswordField()
    // {
    //     m_EventSystem.SetSelectedGameObject(null);
    //     m_EventSystem.SetSelectedGameObject(Password.gameObject);
    // }

    // public void selectLoginButton()
    // {
    //     Login.Select();
    //     // m_EventSystem.SetSelectedGameObject(null);
    //     // m_EventSystem.SetSelectedGameObject(Login);
    // }

    public static void DumpToConsole(object obj)
    {
        var output = JsonUtility.ToJson(obj, true);
        Debug.Log(output);
    }

}

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

