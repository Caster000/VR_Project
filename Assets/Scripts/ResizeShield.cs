using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ResizeShield : MonoBehaviourPunCallbacks, IPunObservable
{
    public void ResizeRPC(Vector3 size)
    {
        photonView.RPC("Resize", RpcTarget.AllViaServer,size);
    }
    
    [PunRPC]
    public void Resize(Vector3 size)
    {
        gameObject.transform.localScale -= size;
        if (gameObject.transform.localScale.x < 0.5)
        {
            Toggle(false);
        }
    }
    
    public void Toggle(bool toggle)
    {
        gameObject.SetActive(toggle);
    }

    // [PunRPC]
    public void ResetShield()
    {
        transform.localScale = new Vector3(1f, 1f, 1f);
        Toggle(true);
    }
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(transform.localPosition);
                stream.SendNext(transform.localRotation);
                stream.SendNext(transform.localScale);
                stream.SendNext(gameObject.activeSelf);
            
        }
        else
        {
            transform.localPosition = (Vector3)stream.ReceiveNext();
            transform.localRotation = (Quaternion)stream.ReceiveNext();
            transform.localScale = (Vector3)stream.ReceiveNext();
            gameObject.SetActive((bool)stream.ReceiveNext());
        }
    }

}
