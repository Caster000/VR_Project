using System;
using System.Collections.Generic;
using System.IO;
using ExitGames.Client.Photon;
using Newtonsoft.Json;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviourPunCallbacks
{
    /// <summary>
    /// The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created.
    /// </summary>
    [Tooltip(
        "The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
    [SerializeField]
    private byte maxPlayersPerRoom = 4;

    [SerializeField] private float timeoutCreateRoom = 10;

    public List<string> mapScene;
    public TMP_Text MessageToUser;
    public GameObject MessageToUserContainer;

    [Header("Level To Load")] 
    public string TutoScene = "TutoScene";

    [Header("Panels and buttons")] 
    public GameObject BackButtonCreateRoomObject;
    public GameObject BackButtonJoinRoomObject;
    public GameObject StartMenuPanel;
    public GameObject CreateRoomObject;
    public GameObject JoinRoomObject;
    
    [Header("Servers & Player Inputs")] 
    [SerializeField] private TMP_InputField ipInputField;
    [SerializeField] private TMP_InputField portInputField;
    [SerializeField] private TMP_InputField roomNameField;
    [SerializeField] private TMP_InputField nickNameInputField;

    [Header("GameConfig Inputs")] 
    [SerializeField] private TMP_InputField lifeNumberField;
    [SerializeField] private TMP_InputField delayShootField;
    [SerializeField] private TMP_InputField delayTeleportField;
    [SerializeField] private TMP_InputField killToVictoryField;
    [SerializeField] private TMP_InputField timeToAreaContaminationField;
    [SerializeField] private TMP_InputField timeToRespawnField;
    [SerializeField] private Toggle contaminationVictoryField;
    [SerializeField] private Toggle killToVictory;
    [SerializeField] private TMP_Dropdown mapDropdown;
    [SerializeField] private TMP_Dropdown mapConfigDropdown;

    private GameConfig gameConfig;
    /// <summary>
    /// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon,
    /// we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
    /// Typically this is used for the OnConnectedToMaster() callback.
    /// this parameter is true when we click on the button Start and false if we come back from a game to the Lobby.
    /// </summary>
    private bool isConnecting;
    private bool isCreating;
    private float timeout;

    #region Awake/Start/Update

    private void Awake()
    {
        gameConfig = GameConfigLoader.Instance.gameConfig;
        PhotonNetwork.AutomaticallySyncScene = true;
        LoadMapDropdown();

    }
    
    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity during initialization phase.
    /// </summary>
    void Start()
    {
        // try connecting to the PUN server
        // progressLabel.SetActive(false);
        // controlPanel.SetActive(true);
        
        isConnecting = false;
        ipInputField.text = "10.169.128.201";
        portInputField.text = "5055";
        timeout = timeoutCreateRoom;
    }

    private void Update()
    {
        if (isCreating && timeout>0)
        {
            timeout -= Time.deltaTime;
        }

        if (isCreating && timeout<=0)
        {
            DisplayMessageError("Can't create or connect to server");
            isCreating = false;
            timeout = timeoutCreateRoom;
        }
    }

    public void DisplayMessageError(string message)
    {
        MessageToUser.text = message;
        MessageToUserContainer.SetActive(true);

    }

    #endregion

    #region Switch Panel Methods

    
    private void LoadMapDropdown()
    {
        foreach (string map in mapScene)
        {
            mapDropdown.options.Add(new TMP_Dropdown.OptionData(map));
        }
    }

    private void LoadMapConfig()
    {
        DirectoryInfo d = new DirectoryInfo("Assets/StreamingAssets/"+LevelConfig.PATH);
        Debug.Log(d);
        FileInfo[] Files = d.GetFiles("*.json");
        LoadMapConfigDropdown(Files);
    }

    public void LoadMapConfigDropdown(FileInfo[] files)
    {
        foreach (FileInfo file in files)
        {
            mapConfigDropdown.options.Add(new TMP_Dropdown.OptionData(Path.GetFileNameWithoutExtension(file.Name)));
        }
    }
    
    public void LoadTutorialScene()
    {
        SceneManager.LoadScene(TutoScene);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void BackButtonCreateRoom()
    {
        StartMenuPanel.SetActive(true);
        CreateRoomObject.SetActive(false);
        BackButtonCreateRoomObject.SetActive(false);
        isCreating = false;
    }
    
    public void BackButtonJoinRoom()
    {
        StartMenuPanel.SetActive(true);
        JoinRoomObject.SetActive(false);
        BackButtonJoinRoomObject.SetActive(false);
        //rest room list
        PhotonNetwork.Disconnect();
        RoomList.EmptyList();
        RoomList.listing = false;
    }

    public void DisplayCreatePanel()
    {
        lifeNumberField.text = gameConfig.LifeNumber.ToString();
        delayShootField.text = gameConfig.DelayShoot.ToString();
        delayTeleportField.text = gameConfig.DelayTeleport.ToString();
        killToVictoryField.text = gameConfig.NbContaminatedPlayerToVictory.ToString();
        timeToAreaContaminationField.text = gameConfig.TimeToAreaContamination.ToString();
        timeToRespawnField.text = gameConfig.RespawnTime.ToString();
        LoadMapConfig();
        CreateRoomObject.SetActive(true);
        StartMenuPanel.SetActive(false);
        BackButtonCreateRoomObject.SetActive(true);
    }
    
    public void DisplayJoinPanel()
    {
        JoinRoomObject.SetActive(true);
        StartMenuPanel.SetActive(false);
        BackButtonJoinRoomObject.SetActive(true);
    }

    #endregion


    #region Photon Methods

    public void ConnectToServer()
    {
        Debug.Log(string.Format("Connect event server PhotonNetwork.IsConnected {0}", PhotonNetwork.IsConnected));
        PhotonNetwork.NickName = nickNameInputField.text;
       
        Debug.LogFormat("Use OnPremise Server - Connect to {0}:{1}",
            ipInputField.text,
            portInputField.text);
        PhotonNetwork.PhotonServerSettings.AppSettings.UseNameServer = false;
        PhotonNetwork.PhotonServerSettings.AppSettings.Server = ipInputField.text;
        PhotonNetwork.PhotonServerSettings.AppSettings.Port = int.Parse(portInputField.text);
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = null;
        PhotonNetwork.PhotonServerSettings.AppSettings.Protocol =
            ExitGames.Client.Photon.ConnectionProtocol.Udp;
        
        PhotonNetwork.ConnectUsingSettings();
    }
    /// <summary>
    /// Start the connection process when click on the Start button.
    /// - If already connected, we attempt joining a random room
    /// - if not yet connected, Connect this application instance to Photon Cloud Network
    /// </summary>
    public void CreateRoom()
    {
        try
        {
            CheckCreateInputFields();
            Debug.Log("Config good");
            MessageToUser.text = "Config loading...";
            MessageToUserContainer.SetActive(true);
            
        }
        catch (Exception e)
        {
            MessageToUser.text = e.Message;
            MessageToUserContainer.SetActive(true);
            return;
        }
        isCreating = true;
        ConnectToServer();
    }
    
    public void CheckCreateInputFields()
    {
        Debug.Log(roomNameField.text=="");
        if (!contaminationVictoryField.isOn && !killToVictory.isOn)
            throw new Exception("Choose at least one victory condition") ;

        if (
            delayShootField.text=="" ||
            lifeNumberField.text=="" ||
            delayTeleportField.text=="" ||
            killToVictoryField.text=="" ||
            timeToRespawnField.text=="" ||
            timeToAreaContaminationField.text== ""
            ) throw new Exception("Game Config Fields can't be empty") ;
        
        if (
            ipInputField.text=="" ||
            portInputField.text=="" ||
            roomNameField.text=="" ||
            nickNameInputField.text==""
            ) 
            throw new Exception("Room Settings Fields can't be empty") ;
    }

    public string formatGameConfig()
    {
        gameConfig.LifeNumber = int.Parse(lifeNumberField.text);
        gameConfig.DelayShoot = float.Parse(delayShootField.text);
        gameConfig.DelayTeleport = float.Parse(delayTeleportField.text);
        gameConfig.NbContaminatedPlayerToVictory = int.Parse(killToVictoryField.text);
        gameConfig.TimeToAreaContamination = float.Parse(timeToAreaContaminationField.text);
        gameConfig.killToVictory = killToVictory.isOn;
        gameConfig.contaminationVictory = contaminationVictoryField.isOn;

        return JsonUtility.ToJson(gameConfig);
    }
    
    #endregion
    
    #region MonoBehaviourPunCallbacks Callbacks
    
    public override void OnConnectedToMaster()
    {
        // VHD attention, cet événement est appelé lorsque on quitte une room et que l'on revient sur le Lobby.
        if (isCreating)
        {
            Debug.Log("Connected to server");

            // Create Room
            RoomOptions roomOptions = new RoomOptions()
            {
                IsVisible = true
                , IsOpen = true
                , MaxPlayers =  maxPlayersPerRoom
                , PlayerTtl = 60000
                , EmptyRoomTtl = 60000
            };
            roomOptions.CustomRoomProperties = new Hashtable();

            LevelConfigLoader.Instance.levelConfig.Load(mapConfigDropdown.options[mapConfigDropdown.value].text);
            string levelConfig = JsonUtility.ToJson(LevelConfigLoader.Instance.levelConfig);
            
            roomOptions.CustomRoomProperties.Add("LevelConfig",levelConfig);
            string gameConfigCustom = formatGameConfig();
            
            roomOptions.CustomRoomProperties.Add("GameConfig",gameConfigCustom);
            roomOptions.CustomRoomPropertiesForLobby = new string[1] { "GameConfig" };

            bool roomCreated = PhotonNetwork.CreateRoom(roomNameField.text, roomOptions, TypedLobby.Default);
            Debug.Log("roomCreated "+roomCreated);
        }
    }


    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("OnDisconnected() was called by PUN with reason {0}", cause);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogWarningFormat("OnJoinRandomFailed() was called by PUN. {0}: {1}", returnCode, message);

        Debug.Log("Create Room");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom()");
    
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1 && isCreating)
        {
            Debug.Log("We load the scene "+mapDropdown.options[mapDropdown.value].text);
            PhotonNetwork.LoadLevel(mapDropdown.options[mapDropdown.value].text);
        }
    }

    #endregion

}
