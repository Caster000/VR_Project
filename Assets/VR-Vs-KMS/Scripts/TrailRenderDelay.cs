using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class TrailRenderDelay : MonoBehaviour, IOnEventCallback
{
    public TrailRenderer TrailRenderer;
    public const byte SendStartTimerEventCode = 1;

    public void SendStartTimerEvent()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(SendStartTimerEventCode, timer, raiseEventOptions, SendOptions.SendReliable);
    }
    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == SendStartTimerEventCode)
        {
            StartTimer();
        }
    }
    
    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    private float timer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            TrailRenderer.enabled = true;
        }

        if (timer < 0)
        {
            TrailRenderer.enabled = false;
        }
        
    }

    public void StartTimer()
    {
        timer = 1.5f;
    }
    
}
