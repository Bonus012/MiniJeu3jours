using UnityEngine;

public class HelixController : MonoBehaviour
{
    public float rotationSpeed = 5f;

    private Vector2 lastTouchPos;

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            float deltaX = Input.GetAxis("Mouse X");
            transform.Rotate(0, -deltaX * rotationSpeed, 0);
        }
#else
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                float deltaX = touch.deltaPosition.x;
                transform.Rotate(0, -deltaX * rotationSpeed * Time.deltaTime, 0);
            }
        }
#endif
    }
}
