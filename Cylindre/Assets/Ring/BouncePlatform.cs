using UnityEngine;
using System.Collections;

public class BouncePlatform : MonoBehaviour
{
    public float moveDownDistance = 0.5f;   // distance vers le bas lors du choc
    public float moveDownSpeed = 10f;       // vitesse déplacement vers le bas
    public float moveUpSpeed = 2f;          // vitesse remontée
    private Vector3 startPos;
    private bool isMoving = false;

    void Start()
    {
        startPos = transform.position;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isMoving)
        {
            StartCoroutine(ImpactRoutine());
        }
    }

    IEnumerator ImpactRoutine()
    {
        isMoving = true;
        Vector3 targetDown = startPos + Vector3.down * moveDownDistance;

        // Descente rapide
        while (Vector3.Distance(transform.position, targetDown) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetDown, moveDownSpeed * Time.deltaTime);
            yield return null;
        }

        // Remontée plus lente
        while (Vector3.Distance(transform.position, startPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPos, moveUpSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = startPos;
        isMoving = false;
    }
}
