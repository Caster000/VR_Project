using System.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class RoomList : MonoBehaviourPunCallbacks
{
    public TMP_Text MessageToUser;
    public GameObject MessageToUserContainer;
    //
    [Header("Prefab RoomItemPrefab")] 
    public GameObject RoomItemPrefab;
    // public TMP_Text PlayerNumber;
    // public TMP_Text Config;
    // public Button JoinBtn;

    // [Header("Room Config")] 
    // private Hashtable RoomOptions;
    // public Room Room;
    
    [Header("Join Room Inputs")] 
    [SerializeField] private TMP_InputField ipJoinInputField;
    [SerializeField] private TMP_InputField portJoinField;
    [SerializeField] private TMP_InputField nickNameJoinInputField;

        
    private TypedLobby customLobby = new TypedLobby(null, LobbyType.Default);

    private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    void Start()
    {
        // try connecting to the PUN server
        // progressLabel.SetActive(false);
        // controlPanel.SetActive(true);
        
        ipJoinInputField.text = "10.169.128.201";
        portJoinField.text = "5055";
    }
    
    public void ConnectToServer()
    {
        Debug.Log(string.Format("Connect event server PhotonNetwork.IsConnected {0}", PhotonNetwork.IsConnected));
        PhotonNetwork.NickName = nickNameJoinInputField.text;
       
        Debug.LogFormat("Use OnPremise Server - Connect to {0}:{1}",
            ipJoinInputField.text,
            portJoinField.text);
        PhotonNetwork.PhotonServerSettings.AppSettings.UseNameServer = false;
        PhotonNetwork.PhotonServerSettings.AppSettings.Server = ipJoinInputField.text;
        PhotonNetwork.PhotonServerSettings.AppSettings.Port = int.Parse(portJoinField.text);
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = null;
        PhotonNetwork.PhotonServerSettings.AppSettings.Protocol =
            ExitGames.Client.Photon.ConnectionProtocol.Udp;
        
        PhotonNetwork.ConnectUsingSettings();
        MessageToUser.text = "Connecting...";
        MessageToUserContainer.SetActive(true);
    }


    public void LoadRooms()
    {
        try
        {
            if (CheckCreateInputFields())
            {
                Debug.Log("Config good");
                //todo export config
                MessageToUser.text = "Config loading...";
                MessageToUserContainer.SetActive(true);
            }
        }
        catch (Exception e)
        {
            MessageToUser.text = e.Message;
            MessageToUserContainer.SetActive(true);
        }
        PhotonNetwork.NickName = nickNameJoinInputField.text;
        ConnectToServer();
        // PhotonNetwork.JoinRoom(Room.Name);
    }


    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        for(int i=0; i<roomList.Count; i++)
        {
            RoomInfo info = roomList[i];
            if (info.RemovedFromList)
            {
                cachedRoomList.Remove(info.Name);
            }
            else
            {
                cachedRoomList[info.Name] = info;
            }
        }
    }
    
    public bool CheckCreateInputFields()
    {
        if (
            ipJoinInputField.text=="" ||
            portJoinField.text=="" ||
            nickNameJoinInputField.text==""
        ) 
            throw new Exception("Room Settings Fields can't be empty") ;
            
        return true;
    }
    
    public void LoadListRoom()
    {
        Debug.Log("cachedRoomList :"+cachedRoomList.Count);
        foreach (KeyValuePair<string,RoomInfo> roomInfo in cachedRoomList )
        {
            GameObject roomItem =  Instantiate(RoomItemPrefab,transform);
            RoomListItem script = roomItem.GetComponent<RoomListItem>();
            Debug.Log(script);
            script.RoomName.text = roomInfo.Key;
            script.PlayerNumber.text = roomInfo.Value.PlayerCount+"/"+roomInfo.Value.MaxPlayers;
            script.Config.text = roomInfo.Value.CustomProperties.ToString();
        }
        MessageToUserContainer.SetActive(false);

    }
    public void JoinLobby()
    {
        PhotonNetwork.JoinLobby(customLobby);
    }
    
    #region MonoBehaviourPunCallbacks Callbacks
    
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to server");
        JoinLobby();
        int count = PhotonNetwork.NetworkingClient.RoomsCount;
        Debug.Log(PhotonNetwork.CountOfRooms);
        Debug.Log("COunt"+count);
        if (count>0)
        {
            LoadListRoom();
        }
        else
        {
            MessageToUser.text = "No room available";
            MessageToUserContainer.SetActive(true);
        }
    }
    
    
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("OnRoomListUpdate : "+ roomList.Count);
        if (roomList.Count>0)
        {
            UpdateCachedRoomList(roomList);
            LoadListRoom();
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        // cachedRoomList.Clear();
        Debug.LogWarningFormat("OnDisconnected() was called by PUN with reason {0}", cause);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogWarningFormat("OnJoinRandomFailed() was called by PUN. {0q}: {1}", returnCode, message);
        MessageToUser.text = "Failed to join";
        MessageToUserContainer.SetActive(true);
    }

    public override void OnJoinedRoom()
    {
        // cachedRoomList.Clear();
        Debug.Log("OnJoinedRoom()");

        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            PhotonNetwork.LoadLevel("TutoScene");
        }
    }

    public override void OnLeftLobby()
    {
        // cachedRoomList.Clear();
    }

    #endregion
}

