﻿using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
        
        private int playerNumber = 0;
        private bool isSomeoneAlreadyIn = false;
        private bool isSameTeam = false;
        private bool isAlone = false;
        private float timer;
        public int layerCapture = 0;
        public List<int> layers = new List<int>();
        private AudioSource audioSource;
        private ParticleSystem pSystem;
        ParticleSystem.ColorOverLifetimeModule colorModule; 
        private WindZone windZone;
        private int remainingGrenades;
        private CullingGroup cullGroup;
        
        private GameConfig gameConfig;


        void Start()
        {
            gameConfig = GameConfigLoader.Instance.gameConfig;
            populateParticleSystemCache();
            setupCullingGroup();

            BelongsToNobody();
            audioSource = GetComponent<AudioSource>();
        }

        private void populateParticleSystemCache()
        {
            pSystem = GetComponentInChildren<ParticleSystem>();
            colorModule = pSystem.colorOverLifetime;
        }


        /// <summary>
        /// This manage visibility of particle for the camera to optimize the rendering.
        /// </summary>
        private void setupCullingGroup()
        {
            // Debug.Log($"setupCullingGroup {Camera.main}");
            cullGroup = new CullingGroup();
            cullGroup.targetCamera = Camera.main;
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
            if (!isSomeoneAlreadyIn)
            {
                layers.Add(other.gameObject.layer);
                isSomeoneAlreadyIn = true;
                playerNumber++;
                isAlone = true;
                checkLayerCapture(other.gameObject.layer);
            }
            else
            {
                foreach (var layer in layers)
                {

                    if (layer != other.gameObject.layer)
                    {
                        isSameTeam = false;
                    }
                    else
                    {
                        isSameTeam = true;
                        checkLayerCapture(other.gameObject.layer);

                    }
                }
                layers.Add(other.gameObject.layer);
                playerNumber++;
                isAlone = false;
            }
        }
        private void OnTriggerStay(Collider other)
        {
            if (isSameTeam && !isAlone)
            {
                contaminationProcess(other.gameObject.layer);
            }
            if(isAlone)
            {

                contaminationProcess(other.gameObject.layer);
            }
            if (!isSameTeam && !isAlone)
            {
                audioSource.Stop();

            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (playerNumber > 0)
            {
                playerNumber--;
                isAlone = false;
            }
            if(playerNumber == 0)
            {
                isSomeoneAlreadyIn = false;
                isAlone = false;
            }
            if(playerNumber == 1)
            {
                isAlone = true;

            }
            timer = 0;
            audioSource.Stop();


        }
        void Update()
        {

        }

        private void checkLayerCapture(int playerLayer)
        {
            if (layerCapture != playerLayer)
            {
                audioSource.Play();

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
            layerCapture = 8;
            
        }

        public void BelongsToScientists()
        {

            ColorParticle(pSystem, scientist.mainColor, scientist.secondColor);
            layerCapture = 7;
        }
        public void contaminationProcess(int layer)
        {

            if (timer >= gameConfig.TimeToAreaContamination)
            {

                if (layer == 7)
                {
                    BelongsToScientists();
                    audioSource.Stop();

                }
                else if (layer == 8)
                {
                    BelongsToVirus();
                    audioSource.Stop();

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
            if (stream.IsWriting)
            {
                stream.SendNext(colorModule.color) ;
                
            }
            else
            {
                colorModule.color = (ParticleSystem.MinMaxGradient)stream.ReceiveNext();
               
            }
        }

    }
}