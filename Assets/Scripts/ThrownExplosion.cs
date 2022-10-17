using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownExplosion : MonoBehaviour
{

    private bool isThrown;
    private int scientistLayer;
    private Rigidbody throwableRigidbody;

    private float power = 10.0f;
    private float radius = 5.0f;
    private float upforce = 1.0f;

    [SerializeField] private float minimumThrownVelocity = 0.5f; //0.5 is chosen because it is considered sufficient to determine if a throwable has been thrown

    private void Awake()
    {
        scientistLayer = LayerMask.NameToLayer("Scientist");
        throwableRigidbody = GetComponent<Rigidbody>();

        power = GameConfigLoader.Instance.gameConfig.ExplosionPower > 0.0f ? GameConfigLoader.Instance.gameConfig.ExplosionPower : power;
        radius = GameConfigLoader.Instance.gameConfig.RadiusExplosion > 0.0f ? GameConfigLoader.Instance.gameConfig.RadiusExplosion : radius;
        upforce = GameConfigLoader.Instance.gameConfig.ExplosionUpforce > 0.0f ? GameConfigLoader.Instance.gameConfig.ExplosionUpforce : upforce;
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
        Vector3 position = gameObject.transform.position;
        Collider[] hitColliders = Physics.OverlapSphere(position, radius);
        GameObject currentTreatedObject;
        Rigidbody currentRigidbody;
        foreach(Collider collider in hitColliders)
        {
            currentTreatedObject = collider.gameObject;
            currentRigidbody = currentTreatedObject.GetComponent<Rigidbody>();

            if(currentRigidbody)
                currentRigidbody.AddExplosionForce(power, position, radius, upforce, ForceMode.Impulse);

            if (currentTreatedObject.layer == scientistLayer)
                currentTreatedObject.GetComponent<UserManager>().TakeDamage();
        }
        
        Destroy(gameObject);
    }

}
