using UnityEngine;

public class CursorFollow : MonoBehaviour
{
    [SerializeField] private GameObject cursorSprite; 

    void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        cursorSprite.transform.position = worldPosition;
    }
}