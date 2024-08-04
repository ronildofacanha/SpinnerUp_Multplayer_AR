using UnityEngine;

public class LookAtCameraSelf:MonoBehaviour
{
    void Update()
    {
        if (Camera.main != null)
        {
            // Faz o transform do Canvas olhar para a posição da câmera principal
            transform.LookAt(Camera.main.transform);
        }
    }
}
