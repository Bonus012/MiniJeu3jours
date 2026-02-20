using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    public TMP_Text scoreText;       // Assigne ton TMP_Text dans l’inspecteur
    public float ScoreByObstacle = 10f;  // Points gagnés par obstacle, modulable

    private int score = 0;

    void Start()
    {
        UpdateScoreText();
    }

    // Ajoute le score en fonction du combo
    public void AddScoreWithCombo(int combo)
    {
        int pointsToAdd = Mathf.RoundToInt(ScoreByObstacle * combo);
        score += pointsToAdd;
        UpdateScoreText();
    }

    public void AddScore()
    {
        int pointsToAdd = Mathf.RoundToInt(ScoreByObstacle);
        score += pointsToAdd;
        UpdateScoreText();
    }

    public void AddThisScore(float thescore)
    {
        int pointsToAdd = Mathf.RoundToInt(thescore);
        score += pointsToAdd;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
            scoreText.text = "" + score;
    }
}
