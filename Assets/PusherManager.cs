using System;
using System.Threading.Tasks;
using PusherClient;
using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

using BestHTTP;

using Newtonsoft.Json;
using TMPro;

public class PusherManager : MonoBehaviour
{
    // A mutation of https://unity3d.com/learn/tutorials/projects/2d-roguelike-tutorial/writing-game-manager
    public static PusherManager instance = null;
    public String Room;
    public TextMeshProUGUI JoinCodeText;
    private Pusher _pusher;
    private Channel _channel;
    private const string APP_KEY = "81019b1380702f6af7e4";
    private const string APP_CLUSTER = "ap4";

  public const string baseUrl = "https://15e0-61-245-129-196.au.ngrok.io";
  public const string token = "1|R8Gize55jT26Uj8hrXP7UuSgrn8IKTTXT2Vk6pYd";
public Text my_text;

    async Task Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        // DontDestroyOnLoad(gameObject); <----------------------- this causing bug?
        await InitialisePusher();
    }

    private async Task InitialisePusher()
    {
        //Environment.SetEnvironmentVariable("PREFER_DNS_IN_ADVANCE", "true");

        if (_pusher == null && (APP_KEY != "APP_KEY") && (APP_CLUSTER != "APP_CLUSTER"))
        {
            _pusher = new Pusher(APP_KEY, new PusherOptions()
            {
                Cluster = APP_CLUSTER,
                Encrypted = true,
                Authorizer = new MyAuthorizer(baseUrl + "/broadcasting/auth")
            });

            // _pusher.Error += OnPusherOnError;
            _pusher.ConnectionStateChanged += PusherOnConnectionStateChanged;
            _pusher.Connected += PusherOnConnected;

            
            // Create the room - Send http request to create our room and get the name of room back
            Debug.Log("Requesting create room...");
            var request = new HTTPRequest(new Uri(baseUrl + "/api/new-room"));
            request.SetHeader("Accept", "application/json");
            request.SetHeader("Authorization", "Bearer " + token);
            
            string result = await request.GetAsStringAsync();
            Debug.Log("Room name is: " + result);
            Room = result;

            // Display it
            JoinCodeText.text = Room;

            // Subscribe to it

              // if (response.StatusCode == 200) {
                  _channel = await _pusher.SubscribeAsync("private-" + result);

                  // _channel = await _pusher.SubscribeAsync("private-chat-channel-1").ConfigureAwait(false);
                  // Assert.AreEqual(false, _channel.IsSubscribed);

                  _pusher.Subscribed += OnChannelOnSubscribed;

                  // Connect
                  await _pusher.ConnectAsync();
                  // await _pusher.ConnectAsync().ConfigureAwait(false);
              // }

        }
        else
        {
            Debug.LogError("APP_KEY and APP_CLUSTER must be correctly set. Find how to set it at https://dashboard.pusher.com");
        }
    }

    private void PusherOnConnected(object sender)
    {
        Debug.Log("Connected");

        _channel.Bind("client-my-event", (String data) =>
        {

          Debug.Log("--------------------------------------------------------------------Client Message Recieved ---------------------------------------------------------");
          Debug.Log(data);
          
            // try
            // {
            //     var theData = data.ToString()
            //         .Replace("\\\"", "\"")
            //         .Replace("\"{", "{")
            //         .Replace("}\"", "}");
            //     var received = JsonConvert.DeserializeObject<ChatMessage>(theData);
            //     if (received != null)
            //     {   
            //         TheDispatcher.RunOnMainThread(() => AddMessage(received.Data.Message, received.Data.Name));
            //     }
            // }
            // catch(Exception ex)
            // { }
        });
        _channel.Bind("my-event", (String data) =>
        {
                // var theData = data.ToString()
                //     .Replace("\\\"", "\"")
                //     .Replace("\"{", "{")
                //     .Replace("}\"", "}");
                // var received = JsonConvert.DeserializeObject<ChatMessage>(theData);
                // if (received != null)
                // {
                    // TheDispatcher.RunOnMainThread(() => AddMessage(received.Data.Message, received.Data.Name));
                // MessageText.text = "my-event received";
            // Debug.Log("my-event received");
            Debug.Log("------------------------------------------------------------------Server Message Recieved ---------------------------------------------------------");
        });
    }

    public void MyTriggerEvent()
    {
        MessageToSend messageToSend = new MessageToSend
        {
            ChannelId = "private-setup-channel",
            Message = "Room",
            SocketId = _pusher.SocketID,
            UserId = "DrTvOS"
        };
        _channel.Trigger("client-my-event", messageToSend);
        Debug.Log("---------------------------------------------------------------------Triggering client event ---------------------------------------------------");
    }

// private static void PushData(PusherAction action, string eventName, dynamic data)
    public void WriteToBox()
        {
          // Debug.Log("WriteToBox");

          // MessageText.text = "my-event received";

        // my_text = GameObject.Find("MessageText").GetComponent<Text>();
        // my_text.text = "Testing...";

            // RefreshDataPusherModel refreshDataPusherModel = new RefreshDataPusherModel()
            // {
            //     Action = action,
            //     EventName = eventName,
            //     Data = data,
            // };
            // _eventAggregator.GetEvent<RefreshData>().Publish(refreshDataPusherModel);
        }

    private void PusherOnConnectionStateChanged(object sender, ConnectionState state)
    {
        Debug.Log("Connection state changed");
    }

    // private void OnPusherOnError(object s, PusherException e)
    // {
    //     Debug.Log("Errored");
    // }

    private void OnChannelOnSubscribed(object s, Channel channel)
    {
        Debug.Log("----------------------------------------------------------------------Subscribed---------------------------------------------------------------");
    }

    async Task OnApplicationQuit()
    {
        if (_pusher != null)
        {
            await _pusher.DisconnectAsync();
        }
    }

    private string generateRoomName()
    {
        // return "room-" + Guid.NewGuid().ToString().Substring(0, 5);
        String myString = "";
        const string glyphs = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        for(int i=0; i<4; i++)
        {
             myString += glyphs[UnityEngine.Random.Range(0, glyphs.Length)];
        }
        return myString;
    }
}


public class MyAuthorizer : IAuthorizer
{
    private Uri _authEndpoint;
    public MyAuthorizer (string authEndpoint)
    {
        _authEndpoint = new Uri(authEndpoint);
    }

    public string Authorize(string channelName, string socketId)
    {
        string authToken = null;

        using (var webClient = new System.Net.WebClient())
        {
            string data = String.Format("channel_name={0}&socket_id={1}", channelName, socketId);
            webClient.Headers.Set("Content-Type", "application/x-www-form-urlencoded");
            webClient.Headers.Add("Accept", "application/json");
            // webClient.Headers.Add("Authorization", "Bearer 1|AfT4WRoTqLfgaDZhX2pxb7wLI748AIHuCPCZuF2K"); // my computer
            webClient.Headers.Add("Authorization", "Bearer 1|R8Gize55jT26Uj8hrXP7UuSgrn8IKTTXT2Vk6pYd"); // tvos
            authToken = webClient.UploadString(_authEndpoint, "POST", data);
        }
        return authToken;
    }
}

// public class ChatMessage
// {
//     public Data Data { get; set; }
// }
// public class Data
// {
//     public string Id { get; set; }
//     public string Message { get; set; }
//     public string UserId { get; set; }
//     public string Name { get; set; }
//     public DateTime When { get; set; }
//     public string Channel { get; set; }
// }

[Serializable]
public class MessageToSend
{
    public String ChannelId;
    public String Message;
    public String SocketId;
    public String UserId;
}
