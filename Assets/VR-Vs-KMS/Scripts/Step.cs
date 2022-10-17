using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Step : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] clips;

    private AudioSource audioSource;
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    
   public void Steps()
    {
        audioSource.PlayOneShot(clips[Random.Range(0, clips.Length)]);
    }

    
}
