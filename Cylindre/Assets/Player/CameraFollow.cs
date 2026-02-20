using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Le joueur

    [Header("Position relative (Offset)")]
    public Vector3 offset = new Vector3(0f, 30f, -40f); // bien éloignée et en hauteur

    void LateUpdate()
    {

        if (target == null) return;

        // Position directe sans interpolation
        transform.position = target.position + offset;
    }

    // Setter appelé depuis le spawner
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
