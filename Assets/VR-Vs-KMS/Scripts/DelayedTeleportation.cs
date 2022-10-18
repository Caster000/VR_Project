using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Random = System.Random;

namespace VR_Vs_KMS.Scripts
{
    /// <summary>
    /// An area is a teleportation destination which teleports the user to their pointed
    /// location on a surface.
    /// </summary>
    /// <seealso cref="TeleportationAnchor"/>
    [AddComponentMenu("XR/Teleportation Area", 11)]
    
    
    
    public class DelayedTeleportation : BaseTeleportationInteractable
    {
        
        
        private float timer;
        private bool canTp = true;
        
        private XRInteractorLineVisual _xrInteractorLineVisual;
        private GameObject manageParticle;
        private Vector3 startPosition;
        private Vector3 endPosition;
        private Vector3 direction;
        private bool inTravel;
        private GameConfig _gameConfig;

        public void Start()
        {
            _gameConfig = GameConfigLoader.Instance.gameConfig;
        }

        /// <inheritdoc />
        /// 
        protected override bool GenerateTeleportRequest(IXRInteractor interactor, RaycastHit raycastHit, ref TeleportRequest teleportRequest)
        {
            
            _xrInteractorLineVisual = interactor.transform.gameObject.GetComponent<XRInteractorLineVisual>();
            manageParticle = interactor.transform.gameObject.GetComponent<ManageParticle>().particleObjecT;

            if (!canTp) return false;

            if (raycastHit.collider == null)
                return false;
            teleportRequest.destinationPosition = raycastHit.point;
            teleportRequest.destinationRotation = transform.rotation;
            canTp = false;
            timer = _gameConfig.DelayTeleport;
            endPosition = 
            startPosition = teleportRequest.destinationPosition;
            
            
            return true;
        }

         void Update()
         {
             if (timer > 0 && !canTp)
             {
                 ChangeColorRaycast(Color.yellow);
                 
                 timer -= Time.deltaTime;
                 
                
                 
             }
             if (timer <= 0 && !canTp)
             {
               
                 canTp = true;
                 ChangeColorRaycast(Color.white);
             }
         }

         private void ChangeColorRaycast(Color color)
         {
             _xrInteractorLineVisual.validColorGradient = 
                     new Gradient
                     {
                         colorKeys = new[] { new GradientColorKey(color, 0f), new GradientColorKey(color, 1f) },
                         alphaKeys = new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) },
                     };
         }
       

       

        
    }
   


}
