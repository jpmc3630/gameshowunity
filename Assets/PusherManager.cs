using System;
using System.Threading.Tasks;
using PusherClient;
using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

using Newtonsoft.Json;

public class PusherManager : MonoBehaviour
{
    public Text MessageText;
    // A mutation of https://unity3d.com/learn/tutorials/projects/2d-roguelike-tutorial/writing-game-manager
    public static PusherManager instance = null;
    private Pusher _pusher;
    private Channel _channel;
    private const string APP_KEY = "81019b1380702f6af7e4";
    private const string APP_CLUSTER = "ap4";

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
        DontDestroyOnLoad(gameObject);
        await InitialisePusher();
        Console.WriteLine("Starting");
        MessageText.text = "The start";
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
                // Authorizer = new FakeAuthoriser()
            });

            // _pusher.Error += OnPusherOnError;
            _pusher.ConnectionStateChanged += PusherOnConnectionStateChanged;
            _pusher.Connected += PusherOnConnected;

            // Subscribe
            _channel = await _pusher.SubscribeAsync("my-channel");
            // _channel = await _pusher.SubscribeAsync("private-chat-channel-1").ConfigureAwait(false);
            // Assert.AreEqual(false, _channel.IsSubscribed);

			      _pusher.Subscribed += OnChannelOnSubscribed;

            // Connect
            await _pusher.ConnectAsync();
            // await _pusher.ConnectAsync().ConfigureAwait(false);
        }
        else
        {
            Debug.LogError("APP_KEY and APP_CLUSTER must be correctly set. Find how to set it at https://dashboard.pusher.com");
        }
    }

    private void PusherOnConnected(object sender)
    {
        Debug.Log("Connected");
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
            Debug.Log("----------------------------------------------------------------------Message Recieved---------------------------------------------------------------");
        });
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
