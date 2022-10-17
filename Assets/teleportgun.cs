using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class teleportgun : MonoBehaviour
{
   public GameObject gunspawnPoint;
   private int collisionlayer;
   private Rigidbody gunRigid;
   


   private void Start()
   {
      gunspawnPoint = Camera.main.transform.GetChild(0).gameObject;
      gunRigid = GetComponent<Rigidbody>();
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
