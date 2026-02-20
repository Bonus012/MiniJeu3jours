using UnityEngine;

public class KillingZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Ball ball = other.GetComponent<Ball>();
            if (ball != null)
            {
                ball.Die(); // Appelle une méthode de mort sur la balle
            }
        }
    }
}
