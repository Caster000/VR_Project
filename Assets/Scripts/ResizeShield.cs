using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ResizeShield : MonoBehaviour, IPunObservable
{
    // private void OnCollisionEnter(Collision collision)
    // {
    //     if (collision.gameObject.layer == 7 && collision.gameObject.GetComponent<BulletBehaviour>())
    //     {
    //         Debug.Log("Collision");
    //         photonView.RPC("Resize", RpcTarget.AllViaServer,new Vector3(0.1f, 0.1f, 0.1f));
    //
    //     }
    //
    //     if (gameObject.transform.localScale.x < 0.5)
    //     {
    //         photonView.RPC("Toggle", RpcTarget.AllViaServer,false);
    //
    //     }
    //     
    // }
    
    [PunRPC]
    public void Resize(Vector3 size)
    {
        gameObject.transform.localScale -= size;
        if (gameObject.transform.localScale.x < 0.5)
        {
            Toggle(false);
        }
    }
    
    // [PunRPC]
    public void Toggle(bool toggle)
    {
        gameObject.SetActive(toggle);
    }

    // [PunRPC]
    public void ResetShield()
    {
        transform.localScale = new Vector3(1f, 1f, 1f);
        gameObject.SetActive(true);
    }
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // if(stream.IsWriting)
        // {
        //
        //         stream.SendNext(transform.localPosition);
        //         stream.SendNext(transform.localRotation);
        //         stream.SendNext(transform.localScale);
        //         stream.SendNext(gameObject.activeSelf);
        //     
        // }
        // else
        // {
        //     transform.localPosition = (Vector3)stream.ReceiveNext();
        //     transform.localRotation = (Quaternion)stream.ReceiveNext();
        //     transform.localScale = (Vector3)stream.ReceiveNext();
        //     gameObject.SetActive((bool)stream.ReceiveNext());
        // }
    }

}
