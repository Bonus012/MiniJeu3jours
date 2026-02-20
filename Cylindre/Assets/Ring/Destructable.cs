using UnityEngine;

public class Destructable : MonoBehaviour
{
    public Transform target;             // Le joueur
    public Score scoreSystem;            // Score manager à lier ou auto-recherche

    private bool hasBeenDestroyed = false;

    void Start()
    {
        // Cherche automatiquement le Score si non assigné
        if (scoreSystem == null)
        {
            scoreSystem = FindObjectOfType<Score>();
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    void Update()
    {
        if (hasBeenDestroyed || target == null) return;

        // Si le joueur est passé en dessous de cet objet
        if (target.position.y < transform.position.y)
        {
            HandleComboAndScore();
            HandleDestruction();
        }
    }

    void HandleComboAndScore()
    {
        Ball ball = target.GetComponent<Ball>();
        if (ball != null)
        {
            ball.AddCombo(1);

            if (scoreSystem != null)
            {
                scoreSystem.AddScoreWithCombo(ball.GetCombo());
            }
        }
    }

    void HandleDestruction()
    {
        hasBeenDestroyed = true;

        PartialHollowCylinderShatterer shatterer = GetComponent<PartialHollowCylinderShatterer>();
        if (shatterer != null)
        {
            shatterer.Shatter(); // Explosion 🎆
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
