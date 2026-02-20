using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelGenerator : MonoBehaviour
{
    [System.Serializable]
    public struct OpeningRange
    {
        public float min;
        public float max;
    }

    [Header("Player")]
    public GameObject playerPrefab;
    public TMP_Text comboText;

    [Header("Level")]
    public GameObject cylinderPrefab;
    public float minHeight = 100f;
    public float maxHeight = 150f;

    [Header("Platform")]
    public OpeningRange Opening;
    public GameObject obstaclePrefab;
    public float obstacleSpacing = 10f;
    [Range(0f, 1f)] public float separateSpawnChance = 0.3f; // ✅ Nouveau paramètre exposé

    [Header("Goal")]
    public GameObject goalPrefab;

    [Header("UI")]
    public Slider progressSlider;

    private Transform playerTransform;
    private float topY;
    private float bottomY;

    private GameObject currentCylinder;
    private GameObject currentPlayer;
    private GameObject currentGoal;

    private Color startColor;
    private Color endColor;

    void Start()
    {
        GenerateNewLevel();
    }

    void Update()
    {
        if (playerTransform != null && progressSlider != null)
        {
            float totalDistance = topY - bottomY;
            float distanceParcourue = topY - playerTransform.position.y;
            progressSlider.value = Mathf.Clamp(distanceParcourue, 0f, totalDistance);
        }
    }

    public void GenerateNewLevel()
    {
        // Clean previous level
        if (currentPlayer != null) Destroy(currentPlayer);
        if (currentGoal != null) Destroy(currentGoal);
        if (currentCylinder != null) Destroy(currentCylinder);

        // Random colors
        startColor = Random.ColorHSV(0f, 1f, 0.7f, 1f, 0.8f, 1f);
        endColor = Random.ColorHSV(0f, 1f, 0.7f, 1f, 0.8f, 1f);

        // Cylinder height
        float height = Random.Range(minHeight, maxHeight);

        currentCylinder = Instantiate(cylinderPrefab, Vector3.zero, Quaternion.identity);
        currentCylinder.transform.localScale = new Vector3(1f, height / 2f, 1f);

        topY = currentCylinder.transform.position.y + height / 2f;
        bottomY = currentCylinder.transform.position.y - height / 2f;

        if (progressSlider != null)
        {
            progressSlider.minValue = 0f;
            progressSlider.maxValue = topY - bottomY;
            progressSlider.value = 0f;
        }

        // Spawn player
        Vector3 spawnPosition = new Vector3(0, topY + 1f, -0.9f);
        currentPlayer = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
        playerTransform = currentPlayer.transform;

        Ball ballScript = currentPlayer.GetComponent<Ball>();
        if (ballScript != null && comboText != null)
        {
            ballScript.comboText = comboText;
        }

        Camera.main.GetComponent<CameraFollow>().SetTarget(playerTransform);

        // Spawn goal
        Vector3 goalPosition = new Vector3(0, bottomY, 0);
        currentGoal = Instantiate(goalPrefab, goalPosition, Quaternion.identity);
        currentGoal.transform.parent = currentCylinder.transform;

        // Spawn obstacles
        for (float y = topY - obstacleSpacing; y > bottomY + 1f; y -= obstacleSpacing)
        {
            float angle = Random.Range(0f, 360f);
            Vector3 obstaclePos = new Vector3(0, y, 0);

            GameObject obstacle = Instantiate(obstaclePrefab, obstaclePos, Quaternion.Euler(0, angle, 0));
            obstacle.transform.parent = currentCylinder.transform;

            float t = Mathf.InverseLerp(bottomY, topY, y);
            Renderer rend = obstacle.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material.color = Color.Lerp(startColor, endColor, t);
            }

            Destructable destructible = obstacle.GetComponent<Destructable>();
            if (destructible != null)
            {
                destructible.SetTarget(playerTransform);
            }

            PartialHollowCylinderMesh meshScript = obstacle.GetComponent<PartialHollowCylinderMesh>();
            if (meshScript != null)
            {
                bool makeSeparate = Random.value < separateSpawnChance; // 30% chance

                if (makeSeparate)
                {
                    meshScript.isSeparate = true;
                    meshScript.OpeningAngle = 360f;
                    meshScript.numberOfCuts = Random.Range(2, 5); // 2 to 4
                    meshScript.cutAngleSize = Random.Range(30f, 60f);
                }
                else
                {
                    meshScript.isSeparate = false;
                    meshScript.OpeningAngle = Random.Range(Opening.min, Opening.max);
                }

                meshScript.GenerateMesh();
            }
        }
    }
}
