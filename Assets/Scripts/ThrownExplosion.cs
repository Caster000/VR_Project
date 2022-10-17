using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownExplosion : MonoBehaviour
{

    private bool isThrown;
    private int scientistLayer;
    private Rigidbody throwableRigidbody;

    [SerializeField] private float minimumThrownVelocity = 0.5f; //0.5 is chosen because it is considered sufficient to determine if a throwable has been thrown

    private void Awake()
    {
        scientistLayer = LayerMask.NameToLayer("Scientist");
        throwableRigidbody = GetComponent<Rigidbody>();
    }

    public void Throw()
    {
        isThrown = throwableRigidbody?.velocity.magnitude > minimumThrownVelocity; 
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isThrown) 
            Explode();
    }

    private void Explode()
    {
        Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, GameConfigLoader.Instance.gameConfig.RadiusExplosion);
        GameObject currentTreatedObject;
        foreach(Collider collider in hitColliders)
        {
            currentTreatedObject = collider.gameObject;

            if (currentTreatedObject.layer == scientistLayer)
                currentTreatedObject.GetComponent<UserManager>().TakeDamage();
        }
        Destroy(gameObject);
    }

}
