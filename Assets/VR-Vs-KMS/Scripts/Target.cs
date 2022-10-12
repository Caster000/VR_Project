
using UnityEngine;

public class Target : MonoBehaviour
{
    public AudioClip die;
    public float health = 10f;
    public float bulletDamage = 1f;

   

   public void TakeDamage(float amount)
   {
        health -= amount;
        Debug.Log("Took " + amount + " damage");

        if(health <= 0f)
        {
            
            Die();
        }
   }

    void Die()
    {
        AudioSource.PlayClipAtPoint(die, transform.position);
        Destroy(gameObject);
    
   }

   private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("bullet"))
        {
            Destroy(other.gameObject);
            TakeDamage(bulletDamage);
        }
    
    }

}
