using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TeleportGun : MonoBehaviour
{
   public GameObject gunspawnPoint;
   private int collisionlayer;
   private Rigidbody gunRigid;
   private XRSocketInteractor _xrSocketInteractor;
   

   private void Start()
   {
      gunRigid = GetComponent<Rigidbody>();
      _xrSocketInteractor = gunspawnPoint.GetComponent<XRSocketInteractor>();
   }

   private void OnCollisionEnter(Collision collision)
   {
      Debug.Log("In collision");
      collisionlayer = collision.gameObject.layer;
      Debug.Log(collision.gameObject.layer);
      //Layer 6 is ground
      if (collisionlayer == 6 || collisionlayer == 0 )
      {
         Debug.Log("In layer collision");
         transform.position = gunspawnPoint.transform.position;
         gunRigid.isKinematic = true;

      }
   }
}
