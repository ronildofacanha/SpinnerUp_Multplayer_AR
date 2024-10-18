using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    public Transform target;  // O alvo que o objeto deve olhar

    void Update()
    {
        // Verifica se o alvo est� definido
        if (target != null)
        {
            // Faz o objeto olhar na dire��o do alvo
            transform.LookAt(target);
        }
    }
}
