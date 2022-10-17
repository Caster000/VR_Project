using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RoomListItem : MonoBehaviourPunCallbacks
{
    [Header("Prefab texts")] 
    public TMP_Text RoomName;
    public TMP_Text PlayerNumber;
    public TMP_Text Config;

    private Button btn;

    public void Start()
    {
        btn = GetComponent<Button>();
    }

    public void JoinRoom()
    {
        Debug.Log("Join Room...");
        PhotonNetwork.JoinRoom(RoomName.text);
    }
    
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogWarningFormat("OnJoinRandomFailed() was called by PUN. {0}: {1}", returnCode, message);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom() ItemList");
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1 && RoomList.listing)
        {
            Debug.Log("We load the scene 'GameScene'");

            PhotonNetwork.LoadLevel("GameScene");
        }
    }

}
