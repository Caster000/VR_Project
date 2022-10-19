using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Photon.Pun;
using UnityEngine;

public class BulletBehaviour : MonoBehaviourPunCallbacks,IPunObservable
{
    public float life = 5f;
    private GameConfig gameConfig;
    private Rigidbody _rigidbody;

    void Awake()
    {
        gameConfig = GameConfigLoader.Instance.gameConfig;
        Destroy(gameObject, life);
        _rigidbody = GetComponent<Rigidbody>();
    }
    
    
    private void OnCollisionEnter(Collision collision)
    {

            GameObject hit = collision.gameObject;
            IPlayer player = hit.GetComponent<IPlayer>();
            if(player != null)
            {
                //TODO uncomment
                // if (hit.layer == gameObject.layer && !gameConfig.friendlyFire)
                //     return;
                player.TakeDamage();
            
            }
            ResizeShield _resizeShield = hit.GetComponent<ResizeShield>();
            if (_resizeShield)
            {
                Debug.Log("Resize shield");
                if (gameObject.layer == 7) //todo 7
                {
                    Debug.Log("Resize");
                    // _resizeShield.Resize(new Vector3(0.1f, 0.1f, 0.1f));
                    photonView.RPC("Resize", RpcTarget.AllViaServer,new Vector3(0.1f, 0.1f, 0.1f),_resizeShield);
                }
            }
            Destroy(gameObject);
        
    }
    // [PunRPC]
    // public void Resize(Vector3 size,ResizeShield _resizeShield)
    // {
    //     _resizeShield.Resize(size);
    // }
    //
    //
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }
}