using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
[RequireComponent(typeof(PhotonView))]
public class PhotonTransformChildView : MonoBehaviourPunCallbacks, IPunObservable
{
    public Camera cameraPlayer;
    public List<Transform> SynchronizedChildTransform;

    // Start is called before the first frame update
    void Awake()
    {
        // if (photonView.IsMine)
        // {
        //     for (int i = 0; i < SynchronizedChildTransform.Count; i++)
        //     {
        //         SynchronizedChildTransform[i].gameObject.SetActive(false);
        //         SynchronizedChildTransform[i].gameObject.SetActive(false);
        //         SynchronizedChildTransform[i].gameObject.SetActive(false);
        //     }
        // }
        if (NetworkManager.isMulti)
        {
            cameraPlayer.enabled = photonView.IsMine;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    [SerializeField] private float ratio = 0.8f;
    #region IPUnObservable
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
           for (int i = 0; i < SynchronizedChildTransform.Count; i++)
           {
               //Todo MAYBE COMPENSATE LAG
               
                stream.SendNext(SynchronizedChildTransform[i].localPosition);
                stream.SendNext(SynchronizedChildTransform[i].localRotation);
                stream.SendNext(SynchronizedChildTransform[i].localScale);
            }
        }
        else
        {
            for (int i = 0; i < SynchronizedChildTransform.Count; i++)
            {
                SynchronizedChildTransform[i].localPosition = (Vector3)stream.ReceiveNext();
                SynchronizedChildTransform[i].localRotation = (Quaternion)stream.ReceiveNext();
                SynchronizedChildTransform[i].localScale = (Vector3)stream.ReceiveNext();
            }
        }

    }
    #endregion
}
