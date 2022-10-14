using System;
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
        float t = 0f;
        private bool canTp = true;

        // public GameObject LeftHandController;
        // public GameObject RightHandController;

        private XRInteractorLineVisual _xrInteractorLineVisual;
        private ManageParticle manageParticle;
        private Vector3 direction;

        private void Start()
        {
            // _xrInteractorLineVisualRight = RightHandController.GetComponent<XRInteractorLineVisual>();
            // _xrInteractorLineVisualLeft = LeftHandController.GetComponent<XRInteractorLineVisual>();

        }

        /// <inheritdoc />
        /// 
        protected override bool GenerateTeleportRequest(IXRInteractor interactor, RaycastHit raycastHit, ref TeleportRequest teleportRequest)
        {
            
            Debug.Log("can tp : "+canTp);
            _xrInteractorLineVisual = interactor.transform.gameObject.GetComponent<XRInteractorLineVisual>();
            manageParticle = interactor.transform.gameObject.GetComponent<ManageParticle>();

            if (!canTp) return false;

            //timer = teleportRequest.requestTime;
            Debug.Log(raycastHit.collider);
            if (raycastHit.collider == null)
                return false;
            Debug.Log("I TP");
            teleportRequest.destinationPosition = raycastHit.point;
            teleportRequest.destinationRotation = transform.rotation;
            canTp = false;
            timer = 5f;

            
            
            ParticleStart(manageParticle);
            ParticleFollow(interactor.transform.position, teleportRequest.destinationPosition);

            return true;
        }

         void Update()
         {
             if (timer > 0 && !canTp)
             {
                 
                 //Debug.Log("_xrInteractorLineVisualLeft");
                 // _xrInteractorLineVisualRight.validColorGradient.colorKeys[0].color =
                 //    _xrInteractorLineVisualLeft.validColorGradient.colorKeys[0].color = Color.magenta;
                 ChangeColorRaycast(Color.yellow);
                 
                 timer -= Time.deltaTime;
                manageParticle.gameObject.transform.position *= Time.deltaTime;
            }
             if (timer <= 0 && !canTp)
             {
                manageParticle.particleSystem.gameObject.SetActive(false);
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

        private void ParticleFollow(Vector3 startPosition, Vector3 endPosition)
        {
            direction = endPosition - startPosition;
            
           

        }

        private void ParticleStart(ManageParticle manageParticle)
        {
            manageParticle.particleSystem.gameObject.SetActive(true);
        }
    }
   


}
