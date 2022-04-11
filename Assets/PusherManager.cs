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
    private GenericPresenceChannel<Player> _channel;
    public string APP_KEY = "81019b1380702f6af7e4";
    public string APP_CLUSTER = "ap4";



  public Text my_text;


    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject); // <----------------------- this causing bug?
        // await InitialisePusher();
    }

    public async Task InitialisePusher()
    {
        //Environment.SetEnvironmentVariable("PREFER_DNS_IN_ADVANCE", "true");

        if (State.Instance.token == null || State.Instance.baseUrl == null)
        {
            Debug.Log("No State.Instance.token or State.Instance.baseUrl, not starting Pusher");
            return;
        }
        Debug.Log("Initialising pusher with token: " + State.Instance.token);

        _pusher = new Pusher(APP_KEY, new PusherOptions()
        {
            Cluster = APP_CLUSTER,
            Encrypted = true,
            Authorizer = new MyAuthorizer(State.Instance.baseUrl + "/broadcasting/auth")
        });

        // _pusher.Error += OnPusherOnError;
        _pusher.ConnectionStateChanged += PusherOnConnectionStateChanged;
        _pusher.Connected += PusherOnConnected;

        
        // Create the room - Send http request to create our room and get the name of room back
        Debug.Log("Requesting create room...");
        var request = new HTTPRequest(new Uri(State.Instance.baseUrl + "/api/new-room"));
        request.SetHeader("Accept", "application/json");
        request.SetHeader("Authorization", "Bearer " + State.Instance.token);
        
        try {
        string result = await request.GetAsStringAsync();


        Debug.Log("Room name is: " + result);
        Room = result;

        } catch(Exception ex)
        {
            Debug.Log("Create room request failed:");
            Debug.LogException(ex);
        }

        // Display the room name
        JoinCodeText.text = Room;

        // Subscribe to the room
        Debug.Log("Subscribing to: " + "presence-" + Room);

        _channel = await _pusher.SubscribePresenceAsync<Player>("presence-" + Room).ConfigureAwait(false);
        _channel.MemberAdded += PlayerAdded;
        _channel.MemberRemoved += PlayerRemoved;

        // Assert.AreEqual(false, _channel.IsSubscribed);
        _pusher.Subscribed += OnChannelOnSubscribed;

        // Connect
        await _pusher.ConnectAsync();
        // await _pusher.ConnectAsync().ConfigureAwait(false);
    }


    // Lists all current presence channel members
    void ListMembers(GenericPresenceChannel<Player> channel)
    {
        Dictionary<string, Player> members = channel.GetMembers();
        foreach (var member in members)
        {
            // Trace.TraceInformation($"Id: {member.Key}, Name: {member.Value.Name}");
            Debug.Log($"Id: {member.Key}, Name: {member.Value.Name}");
        }
    }

    // MemberAdded event handler
    void PlayerAdded(object sender, KeyValuePair<string, Player> player)
    {
        Player newPlayer = new Player()
        {
            Name = player.Value.Name,
            Id = player.Value.Id
        };
        TheDispatcher.RunOnMainThread(() => State.Instance.addPlayerToPlayerList(newPlayer));
    }

    // MemberRemoved event handler
    void PlayerRemoved(object sender, KeyValuePair<string, Player> player)
    {
        TheDispatcher.RunOnMainThread(() => State.Instance.removePlayerById(player.Value.Id));
    }

    public static void DumpToConsole(object obj)
    {
        var output = JsonUtility.ToJson(obj, true);
        Debug.Log(output);
    }

    private void PusherOnConnected(object sender)
    {
        Debug.Log("Connected");
        _channel.Bind("client-answer", (String data) =>
        {
           
          Debug.Log("-----------------------------Player Answer Recieved -----------------------------");
            try
            {
                var theData = data.ToString()
                    .Replace("\\\"", "\"")
                    .Replace("\"{", "{")
                    .Replace("}\"", "}");
                    Debug.Log(theData);
                var received = JsonConvert.DeserializeObject<AnswerEvent>(theData);
                // AnswerEvent answerEvent = JsonUtility.FromJson<AnswerEvent>(data);
                if (received != null)
                {   
                    Debug.Log("-----------------------------Received: -----------------------------");
                    // if state is waiting for answer, then set the answer for that player
                    if (State.Instance.currentScreen == "question")
                    {
                        
                    }
                    TheDispatcher.RunOnMainThread(() => State.Instance.setPlayerAnswer(received.user_id, received.data.answer));
                }
            }
            catch(Exception ex)
            { 
              Debug.Log(ex);
            }

        });

        _channel.Bind("client-my-event", (String data) =>
        {

          Debug.Log("-----------------------------Client Message Recieved -----------------------------");
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
            //         TheDispatcher.RunOnMainThread(() => AddPlayer(received.Data.Message, received.Data.Name));
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
                    // TheDispatcher.RunOnMainThread(() => AddPlayer(received.Data.Message, received.Data.Name));
                // MessageText.text = "my-event received";
            // Debug.Log("my-event received");
            Debug.Log("----------------------------- Server Message Recieved -----------------------------");
        });
    }

    public void MyTriggerEvent()
    {
        MessageToSend messageToSend = new MessageToSend
        {
            ChannelId = "this-can-be-anything",
            Message = "Room",
            SocketId = _pusher.SocketID,
            UserId = "DrTvOS"
        };
        _channel.Trigger("client-my-event", messageToSend);
        Debug.Log("----------------------------- Triggering client event -----------------------------");
    }


    public void PlayerMode(string mode)
    {
        _channel.Trigger("client-mode", mode);
        Debug.Log("----------------------------- Player Mode: " + mode + " -----------------------------");
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
        Debug.Log("----------------------------- Subscribed -----------------------------");
    }

    public async Task OnApplicationQuit()
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
            // webClient.Headers.Add("Authorization", "Bearer 8|7k4AAxZuGlZbdRENbIHVC1R1lK4OIUqsfjyUM5Da"); // tvos
            Debug.Log("token" + State.Instance.token);
            webClient.Headers.Add("Authorization", "Bearer " + State.Instance.token); // tvos
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

[Serializable]
public class ModeChange
{
    public int Mode;
}

[Serializable]
public class AnswerEvent
{
    public AnswerData data;

    public string user_id;
}
[Serializable]
public class AnswerData
{
    public string answer;
}

