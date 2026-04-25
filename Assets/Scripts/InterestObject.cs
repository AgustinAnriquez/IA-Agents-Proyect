using UnityEngine;

public class InterestObject : MonoBehaviour
{
    public float health = 100f;
    public void RecieveDamage(float amount) 
    {
        health -= amount;
        if (health <= 0) 
        {
            Destroy(gameObject); 
        }
    }
}
