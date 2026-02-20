using UnityEngine;
using TMPro;

public class Ball : MonoBehaviour
{
    public float bounceForce = 5f;
    private Rigidbody rb;

    public TMP_Text comboText; // assigné depuis LevelGenerator
    private int combo = 0;

    private Color startColor = Color.yellow; // combo bas = jaune
    private Color endColor = Color.red;      // combo max = rouge
    private int maxComboForRed = 10;

    public Material normalMaterial;
    public Material aggressiveMaterial;

    private Renderer rend;

    private bool isAggressive = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;

        // Ne plus chercher comboText ici, il est assigné depuis l'extérieur !
        if (comboText == null)
        {
            Debug.LogWarning("comboText n'est pas assigné dans Ball. Assigne-le depuis le LevelGenerator.");
        }
        else
        {
            comboText.text = ""; // cache le texte au lancement
        }

        rend = GetComponentInChildren<Renderer>();

        if (rend != null && normalMaterial != null)
            rend.material = normalMaterial;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Vector3 vel = rb.velocity;
            vel.y = 0f;
            rb.velocity = vel;

            rb.AddForce(Vector3.up * bounceForce, ForceMode.VelocityChange);

            if (isAggressive)
            {
                Destroy(collision.gameObject);
            }

            ResetCombo();
            UpdateComboText();
        }
    }

    public void AddCombo(int amount = 1)
    {
        combo += amount;
        UpdateComboText();
    }

    public int GetCombo()
    {
        return combo;
    }
    public void ResetCombo()
    {
        combo = 0;
        UpdateComboText();
    }

    private void UpdateComboText()
    {
        if (combo >= 2)
        {
            if (comboText != null)
            {
                comboText.text = combo + "x";
                float t = Mathf.InverseLerp(2, maxComboForRed, combo);
                comboText.color = Color.Lerp(startColor, endColor, t);
            }

            if (combo >= 3)
            {
                isAggressive = true;
                if (rend != null && aggressiveMaterial != null)
                    rend.material = aggressiveMaterial;
            }
            else
            {
                isAggressive = false;
                if (rend != null && normalMaterial != null)
                    rend.material = normalMaterial;
            }
        }
        else
        {
            isAggressive = false;
            if (comboText != null)
                comboText.text = "";

            if (rend != null && normalMaterial != null)
                rend.material = normalMaterial;
        }
    }

    public void Die()
    {
        Debug.Log("Le joueur est mort !");
        // TODO: animation, respawn, écran de fin, etc.
    }

}
