using System;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    private float timeoutRoom = 10;
    private float timeoutRoomTemp;
    public static bool isMulti;
    public static NetworkManager Instance;

    [Tooltip("The prefab to use for representing the user on a PC. Must be in Resources folder")]
    public GameObject playerPrefabPC;

    [Tooltip("The prefab to use for representing the user in VR. Must be in Resources folder")]
    public GameObject playerPrefabVR;

    #region Photon Callbacks

    /// <summary>
    /// Called when the local player left the room. 
    /// </summary>
    public override void OnLeftRoom()
    {
        Debug.Log("OnLeftRoom");
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("Menu");
    }

    /// <summary>
    /// Called when Other Player enters the room and Only other players
    /// </summary>
    /// <param name="other"></param>
    public override void OnPlayerEnteredRoom(Player other)
    {
        Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting
        // TODO: 
    }

    /// <summary>
    /// Called when Other Player leaves the room and Only other players
    /// </summary>
    /// <param name="other"></param>
    public override void OnPlayerLeftRoom(Player other)
    {
        Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects
        // TODO: 
    }

    #endregion

    // Start is called before the first frame update

    #region Public Methods

    /// <summary>
    /// Our own function to implement for leaving the Room
    /// </summary>
    public void LeaveRoom()
    {
        if (SceneManager.GetActiveScene().name == "TutoSceneSolo")
        {
            SceneManager.LoadScene("Menu");
        }
        else
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    private void updatePlayerNumberUI()
    {
        // TODO: Update the playerNumberUI
    }

    void Start()
    {
        Instance = this;
        timeoutRoomTemp = timeoutRoom;
        #region TO debug

        Debug.Log("device:" + UserDeviceManager.GetDeviceUsed());
        Debug.Log("prefab:" + UserDeviceManager.GetPrefabToSpawnWithDeviceUsed(playerPrefabPC, playerPrefabVR));

        #endregion

        GameObject playerPrefab = UserDeviceManager.GetPrefabToSpawnWithDeviceUsed(playerPrefabPC, playerPrefabVR);
        if (playerPrefab == null)
        {
            Debug.LogErrorFormat(
                "<Color=Red><a>Missing</a></Color> playerPrefab Reference for device {0}. Please set it up in GameObject 'NetworkManager'",
                UserDeviceManager.GetDeviceUsed());
        }
        else
        {
            // TODO: Instantiate the prefab representing my own avatar only if it is UserMe
            if (UserManager.UserMeInstance == null)
            {
                Vector3 pos = SceneManager.GetActiveScene().name == "Outdoor map"?new Vector3(23f, 4f, 30f):new Vector3(0f, 1f, 0f);
                              Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
                // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                Vector3 initialPos = UserDeviceManager.GetDeviceUsed() == UserDeviceType.HTC
                    ? pos + new Vector3(0,0.5f,0)
                    : pos;
                if (SceneManager.GetActiveScene().name == "TutoSceneSolo")
                {
                    isMulti = false;
                    Instantiate(playerPrefab, initialPos, Quaternion.Euler(0, 180, 0));
                }
                else
                {
                    isMulti = true;
                    PhotonNetwork.Instantiate("Prefabs/" + playerPrefab.name, initialPos, Quaternion.Euler(0, 180, 0), 0);
                }
            }
            else
            {
                Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
            }
        }
    }

    private void Update()
    {
        // if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        // {
            // Code to leave the room by pressing CTRL + the Leave button
            if (Input.GetButtonUp("Leave"))
            {
                LeaveRoom();
            }

    }
    #endregion
}