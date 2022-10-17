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

    [Header("Join Room Inputs")] 
    [SerializeField] private TMP_InputField ipJoinInputField;
    [SerializeField] private TMP_InputField portJoinField;
    [SerializeField] private TMP_InputField nickNameJoinInputField;

        
    private TypedLobby customLobby = new TypedLobby(null, LobbyType.Default);

    private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();
    public static List<GameObject> cachedRoomListItem = new List<GameObject>();
    public static bool listing;
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

    public static void EmptyList()
    {
        foreach (GameObject item in cachedRoomListItem)
        {
            Destroy(item);
        }
        cachedRoomListItem = new List<GameObject>();
    }
    public void LoadRooms()
    {
        listing = true;
        Debug.Log("LoadRooms");
        //PhotonNetwork.Disconnect();
        EmptyList();
        try
        {
            CheckServerInputFields();
            MessageToUser.text = "List loading...";
            MessageToUserContainer.SetActive(true);
            
        }
        catch (Exception e)
        {
            MessageToUser.text = e.Message;
            MessageToUserContainer.SetActive(true);
            return;
        }
        PhotonNetwork.NickName = nickNameJoinInputField.text;
        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("InLobby "+ PhotonNetwork.InLobby);
            if (PhotonNetwork.InLobby)
            {
                JoinLobby();
            }
            Debug.Log("InRoom "+ PhotonNetwork.InRoom);
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
            }
            Load();
        }
        else
        {
            ConnectToServer();
        }
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

    public void Load()
    {
        int count = PhotonNetwork.NetworkingClient.RoomsCount;
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
    public void CheckServerInputFields()
    {
        if (
            string.IsNullOrEmpty(ipJoinInputField.text) ||
            string.IsNullOrEmpty(portJoinField.text) ||
            string.IsNullOrEmpty(nickNameJoinInputField.text)
        ) throw new Exception("Server Settings Fields can't be empty");
    }
    
    public void LoadListRoom()
    {
        Debug.Log("cachedRoomList :"+cachedRoomList.Count);
        EmptyList();
        foreach (KeyValuePair<string,RoomInfo> roomInfo in cachedRoomList )
        {
            GameObject roomItem =  Instantiate(RoomItemPrefab,transform);
            RoomListItem script = roomItem.GetComponent<RoomListItem>();
            cachedRoomListItem.Add(roomItem);
            Hashtable serverConfig = roomInfo.Value.CustomProperties;
            Debug.Log(roomInfo.Value);
            GameConfig gameConfigRoom = JsonUtility.FromJson<GameConfig>((string)serverConfig["GameConfig"]);
            Debug.Log(serverConfig["GameConfig"]);
            script.RoomName.text = roomInfo.Key;
            script.PlayerNumber.text = roomInfo.Value.PlayerCount+"/"+roomInfo.Value.MaxPlayers;
            if (gameConfigRoom != null)
            {
                string config = gameConfigRoom.killToVictory ? "killToVictory" : "";
                config += gameConfigRoom.contaminationVictory ? " contaminationVictory" : "";
                script.Config.text = config;
            }
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
        if (listing)
        {
            Debug.Log("Connected to server RoomList");
            JoinLobby();
            // Load();
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

    // public override void OnDisconnected(DisconnectCause cause)
    // {
    //     cachedRoomList.Clear();
    //     Debug.LogWarningFormat("OnDisconnected() was called by PUN with reason {0}", cause);
    // }

    // public override void OnJoinRandomFailed(short returnCode, string message)
    // {
    //     Debug.LogWarningFormat("OnJoinRandomFailed() was called by PUN. {0q}: {1}", returnCode, message);
    //     MessageToUser.text = "Failed to join";
    //     MessageToUserContainer.SetActive(true);
    // }
    

    // public override void OnLeftLobby()
    // {
    //     // cachedRoomList.Clear();
    // }

    #endregion
}

