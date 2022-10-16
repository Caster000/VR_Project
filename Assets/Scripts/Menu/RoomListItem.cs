using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RoomListItem : MonoBehaviour
{
    [Header("Prefab texts")] 
    public TMP_Text RoomName;
    public TMP_Text PlayerNumber;
    public TMP_Text Config;
    // public Button JoinBtn;

    
    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(RoomName.text);
    }

}
