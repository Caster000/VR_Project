using System;
using System.Collections;
using System.Collections.Generic;
using Invector.vCharacterController;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;
using VR_Vs_KMS.Scripts;
using Random = UnityEngine.Random;

public class VRPlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static GameObject UserMeInstance;
    
    [SerializeField] private List<Transform> spawPoints;
    [SerializeField] private Slider Healthbar;
    [SerializeField] private Canvas CanvasPlayer;
    [SerializeField] private TMP_Text TimerText;
    [SerializeField] private GameObject CameraPlayer;
    
    private int currentHealth;
    private float respawnTime;
    private bool spawned;
    private GameConfig gameConfig;
    
    // Start is called before the first frame update
    void Awake()
    {
        gameConfig = GameConfigLoader.Instance.gameConfig;
        Healthbar.maxValue = Healthbar.value = currentHealth = gameConfig.LifeNumber;

        if (NetworkManager.isMulti)
        {
            if (photonView.IsMine)
            {
                Debug.LogFormat("Avatar UserMe created for userId {0}", photonView.ViewID);
                UserMeInstance = gameObject;
                FindObjectOfType<DelayedTeleportation>().teleportationProvider =
                    GetComponent<TeleportationProvider>();
            }
            CameraPlayer.SetActive(photonView.IsMine);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine && NetworkManager.isMulti) return;
    }
    
    #region Photon

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(currentHealth);
            // stream.SendNext(BodyPlayer.activeSelf);
//            stream.SendNext(GetComponent<Collider>().enabled);
        }
        else
        {
            currentHealth = (int)stream.ReceiveNext();
            // BodyPlayer.SetActive((bool)stream.ReceiveNext());
           // GetComponent<Collider>().enabled = (bool)stream.ReceiveNext();
        }
    }

    #endregion
}
