using System;
using System.Collections;
using System.Collections.Generic;
using Invector.vCharacterController;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Random = UnityEngine.Random;


public abstract class UserManagerAbstract : MonoBehaviourPunCallbacks, IPunObservable
{
    public static GameObject UserMeInstance;

    private List<Transform> spawPoints;
    [SerializeReference]
    private Slider Healthbar;
    private Canvas CanvasPlayer;
    private TMP_Text TimerText;
    private GameObject BodyPlayer;
    private GameObject CameraPlayer;
    
    private int currentHealth;
    private float respawnTime;
    private bool spawned;
    
    private GameConfig gameConfig;

    protected void Init()
    {
        gameConfig = GameConfigLoader.Instance.gameConfig;
        Healthbar.maxValue = Healthbar.value = currentHealth = gameConfig.LifeNumber;
        
        //TODO Remove by loading list of spawn Points
        spawPoints = new List<Transform>();
        spawPoints.Add(GameObject.Find("SpawnArea").transform);
        spawned = true;
    }
    


    // Start is called before the first frame update
    public void Awake()
    {
       Init();
    }

    // Update is called once per frame
    public void Update()
    {
        if (!photonView.IsMine) return;
        
    }

    public void TakeDamage()
    {
        Healthbar.value = --currentHealth;

    }

    #region Photon

    public abstract void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info);

    #endregion
}
