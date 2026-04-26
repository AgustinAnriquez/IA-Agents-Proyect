using UnityEngine;

public class InterestObject : MonoBehaviour
{
    public float health = 100f;
    //private FSMAgent targetAgent;
    public void RecieveDamage(float amount) 
    {
        health -= amount;
        if (health <= 0) 
        {
            //targetAgent.PutCheese--;
            Destroy(gameObject); 
        }
    }
}
