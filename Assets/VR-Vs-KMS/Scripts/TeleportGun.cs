using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TeleportGun : MonoBehaviour
{
   public GameObject gunspawnPoint;
   private int collisionlayer;
   private Rigidbody gunRigid;
   private XRSocketInteractor _xrSocketInteractor;
   

   private void Awake()
   {
      gunRigid = GetComponent<Rigidbody>();
      _xrSocketInteractor = gunspawnPoint.GetComponent<XRSocketInteractor>();
      
   }

   private void OnCollisionEnter(Collision collision)
   {
      Debug.Log("In collision");
      collisionlayer = collision.gameObject.layer;
      Debug.Log(collision.gameObject.layer);
      //Layer 6 is ground & 0 Default
      if ((collisionlayer == 6 || collisionlayer == 0) && _xrSocketInteractor != null )
      {
         _xrSocketInteractor.StartManualInteraction(GetComponent<XRGrabInteractable>());
         Debug.Log("In layer collision");
      }
   }
}
