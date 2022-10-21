using DefaultNamespace;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


namespace vr_vs_kms
{
    public class ContaminationArea : MonoBehaviour, IPunObservable
    {
        [System.Serializable]
        public struct BelongToProperties
        {
            public Color mainColor;
            public Color secondColor;

        }
        public BelongToProperties nobody;
        public BelongToProperties virus;
        public BelongToProperties scientist;
        private float faerieSpeed;
        private float radius = 1f;
        public float cullRadius = 5f;
        private float TimeToAreaContamination;
        private int playerNumber = 0;
        private float timer;
        public int isTaken = 0;
        public AudioSource audioSource;
        private ParticleSystem pSystem;
        ParticleSystem.ColorOverLifetimeModule colorModule;
        private WindZone windZone;
        private int remainingGrenades;
        private CullingGroup cullGroup;
        //Dictionary<int, int> layersList = new Dictionary<int, int>();
        private int numberOfScientist;
        private int numberOfVirus;
        private GameConfig gameConfig;
        private GameManager gameManager;

        void Start()
        {
            gameManager = GameManager.Instance;
            gameConfig = GameConfigLoader.Instance.gameConfig;
            TimeToAreaContamination = gameConfig.TimeToAreaContamination;
            populateParticleSystemCache();
            setupCullingGroup();
            TimeToAreaContamination = gameConfig.TimeToAreaContamination;
            BelongsToNobody();
            // audioSource = GetComponent<AudioSource>();
        }

        private void populateParticleSystemCache()
        {
            pSystem = GetComponentInChildren<ParticleSystem>();
            colorModule = pSystem.colorOverLifetime;
            // pSystem.startColor = Color.white;
        }


        /// <summary>
        /// This manage visibility of particle for the camera to optimize the rendering.
        /// </summary>
        private void setupCullingGroup()
        {
            // Debug.Log($"setupCullingGroup {Camera.main}");
            cullGroup = new CullingGroup();
            // cullGroup.targetCamera = Camera.main;
            cullGroup.SetBoundingSpheres(new BoundingSphere[] { new BoundingSphere(transform.position, cullRadius) });
            cullGroup.SetBoundingSphereCount(1);
            cullGroup.onStateChanged += OnStateChanged;
        }

        void OnStateChanged(CullingGroupEvent cullEvent)
        {
            // Debug.Log($"cullEvent {cullEvent.isVisible}");
            if (cullEvent.isVisible)
            {
                pSystem.Play(true);
            }
            else
            {
                pSystem.Pause();
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<IPlayer>() != null)
            {
                audioSource.Play();
                if (other.gameObject.layer == 7) numberOfScientist++;
                if (other.gameObject.layer == 8) numberOfVirus++;
            }
        }
        private void OnTriggerStay(Collider other)
        {
            //ifsame, stop audio()
            if (other.gameObject.GetComponent<IPlayer>() == null) return;
            if(isTaken != 7 && numberOfScientist > 0 && numberOfVirus == 0 )
            {
                contaminationProcess(other.gameObject.layer);
            }
            if (isTaken != 8 && numberOfVirus > 0 && numberOfScientist == 0)
            {
                contaminationProcess(other.gameObject.layer);
            }
           
            if((numberOfScientist >= 1 && isTaken == 7) || (numberOfVirus >=1 && isTaken == 8) || (numberOfVirus >=1 && numberOfScientist>=1))
            {
               audioSource.Stop();
            }
        }
        private void OnTriggerExit(Collider other)
        {
           
            if (other.gameObject.GetComponent<IPlayer>() != null)
            {
                if (other.gameObject.layer == 7) numberOfScientist--;
                if (other.gameObject.layer == 8) numberOfVirus--;
                if(numberOfScientist == 0 && numberOfVirus == 0) audioSource.Stop();
                timer = 0;
            }
        }
        private void ColorParticle(ParticleSystem pSys, Color mainColor, Color accentColor)
        {
            colorModule.color = new ParticleSystem.MinMaxGradient(mainColor, accentColor);


        }

        public void BelongsToNobody()
        {
            ColorParticle(pSystem, nobody.mainColor, nobody.secondColor);
        }

        public void BelongsToVirus()
        {
            ColorParticle(pSystem, virus.mainColor, virus.secondColor);
            if (isTaken == 0)
            {
                gameManager.IncreaseContaminationAreaVirusScore();
                gameManager.DecreaseContaminationAreaNeutralScore();
            }
            else if (isTaken == 7)
            {
                gameManager.IncreaseContaminationAreaVirusScore();
                gameManager.DecreaseContaminationAreaScientistScore();
            }
        }

        public void BelongsToScientists()
        {
            ColorParticle(pSystem, scientist.mainColor, scientist.secondColor);
            if (isTaken == 0)
            {
                gameManager.IncreaseContaminationAreaScientistScore();
                gameManager.DecreaseContaminationAreaNeutralScore();
            }
            else if (isTaken == 8)
            {
                gameManager.IncreaseContaminationAreaScientistScore();
                gameManager.DecreaseContaminationAreaVirusScore();
            }

        }
        public void contaminationProcess(int layer)
        {

            if (timer >= TimeToAreaContamination)
            {

                if (layer == 7)
                {
                    audioSource.Stop();
                    BelongsToScientists();
                    isTaken = 7;
                    Debug.Log("isTakenByScientist");
                }
                else if (layer == 8)
                {

                    audioSource.Stop();
                    BelongsToVirus();
                    isTaken = 8;
                    Debug.Log("isTakenByVirus");
                }
                timer = 0;
            }
            else
            {
                timer += Time.deltaTime;
            }
        }

        void OnDestroy()
        {
            if (cullGroup != null)
                cullGroup.Dispose();
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, cullRadius);
        }
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            Debug.Log(colorModule);
            if (colorModule.Equals(null))
                return;
            if (stream.IsWriting)
            {
                // stream.SendNext(colorModule.color);
                
                stream.SendNext(colorModule.color.colorMax.r);
                stream.SendNext(colorModule.color.colorMax.g);
                stream.SendNext(colorModule.color.colorMax.b);
                stream.SendNext(colorModule.color.colorMin.r);
                stream.SendNext(colorModule.color.colorMin.g);
                stream.SendNext(colorModule.color.colorMin.b);

            }
            else
            {
                // colorModule.color = (ParticleSystem.MinMaxGradient)stream.ReceiveNext();
                ColorParticle(null, 
                    new Color((float)stream.ReceiveNext(), (float)stream.ReceiveNext(), (float)stream.ReceiveNext()),
                    new Color((float)stream.ReceiveNext(), (float)stream.ReceiveNext(), (float)stream.ReceiveNext()));

            }
        }

    }
}