using System;
using System.Threading.Tasks;
using PusherClient;
using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

// using Newtonsoft.Json;

public sealed class State
{
    // public bool testVar { get; set; }

    public List<Player> playerList = new List<Player> ();
    private static object _lockObj = new object();
    private static State instance;
    public static State Instance

    {
        get
        {
            //only request the access lock if it's null to begin with
            if (instance == null)
            {
                //obtain the access lock to ensure only one thread gets in at a time
                lock(_lockObj)
                {
                    //check again to see if it's null so if a previous thread created it, don't do it again
                    if(instance == null)
                    {
                        instance = new State();
                        
                    }
                }
            }
           
            return instance;
        }
    }
 
    public void addPlayerToPlayerList(Player newPlayer) {
      Debug.Log("yayayayayaayayay hello: " + newPlayer.Name);
      instance.playerList.Add(newPlayer);

      for (int i=0; i< instance.playerList.Count; i++) {
        Debug.Log(instance.playerList[i].Id + " - " + instance.playerList[i].Name);
      }

      // if we are in lobby, update the lobby playerList game object
      
      // instance.updateLobbyPlayerList();
      // maybe we could move a render method in a MonoBehavior script called Renderers or something
    }

    // private void updateLobbyPlayerList() {

        // something like ...
      // if (GameObject.Find("LobbyPlayerList") != null) {
      //   GameObject.Find("LobbyPlayerList").GetComponent<LobbyPlayerList>().updatePlayerList();
      // }

    //     GameObject newMessage = (GameObject)UnityEngine.Object.Instantiate(playerPrefab, playersList.transform.position + new Vector3(0.0f, -200.0f * playersList.childCount, 0.0f), playersList.transform.rotation);
    //     newMessage.transform.SetParent(playersList);
    //     newMessage.transform.SetSiblingIndex(playersList.childCount - 2);

    //     Debug.Log("playersList.childCount: " + playersList.childCount);
    //     var texts = newMessage.GetComponentsInChildren<Text>();
    //     Debug.Log(texts);
    //     if (texts != null
    //         && texts.Length > 0)
    //     {
    //         texts[0].text = newPlayer.Name;
    //     }
    // }
}

// class ChatMember
// {
//     public string Name { get; set; }
// }

// class Player
// {
//     public string Name { get; set; }
//     public string Id { get; set; }
//     public int Score { get; set; }
// }

public class Player
{
    public string Name { get; set; }
    public string Id { get; set; }
    public int Score { get; set; }
}
