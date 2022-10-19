
using UnityEngine;

public class TargetVR : MonoBehaviour
{
    public AudioClip die;
    public AudioClip Damage;
    public float health = 10f;
    public float bulletDamage = 1f;
    public int score;
   


    public void TakeDamage(float amount)
    {
        health -= amount;
        AudioSource.PlayClipAtPoint(Damage, transform.position);
        Debug.Log("Took " + amount + " damage");

        if (health <= 0f)
        {

            Die();
        }
    }

    void Die()
    {
        AudioSource.PlayClipAtPoint(die, transform.position);
        
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("bullet"))
        {
            Destroy(other.gameObject);
            TakeDamage(bulletDamage);
        }

    }

}
