using UnityEngine;

public class Goal : MonoBehaviour
{
    public Score scoreSystem; // Drag dans l'inspecteur ou set par script
    public float WinPoint = 1000f;

    void Start()
    {
        // Cherche automatiquement le Score dans la scène
        if (scoreSystem == null)
        {
            scoreSystem = FindObjectOfType<Score>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // On prévient le LevelGenerator
            FindObjectOfType<LevelGenerator>().GenerateNewLevel();
            scoreSystem.AddThisScore(WinPoint);
        }
    }
}
