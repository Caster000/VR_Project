using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace VR_Vs_KMS.Scripts
{
    /// <summary>
    /// An area is a teleportation destination which teleports the user to their pointed
    /// location on a surface.
    /// </summary>
    /// <seealso cref="TeleportationAnchor"/>
    [AddComponentMenu("XR/e Area", 11)]

    
    public class DelayedTeleportation : BaseTeleportationInteractable
    {
        private float timer;
        private bool canTp = true;
        
        /// <inheritdoc />
        protected override bool GenerateTeleportRequest(IXRInteractor interactor, RaycastHit raycastHit, ref TeleportRequest teleportRequest)
        {
            if (!canTp) return false;

            //timer = teleportRequest.requestTime;
            if (raycastHit.collider == null)
                return false;

            teleportRequest.destinationPosition = raycastHit.point;
            teleportRequest.destinationRotation = transform.rotation;
            canTp = false;
            timer = 5f;
            return true;
        }

         void Update()
         {
             if (timer > 0 && !canTp)
             {
                 timer -= Time.deltaTime;
             }
             if (timer <= 0 && !canTp)
             {
                 canTp = true;
             }
             
         }
    }
   


}
