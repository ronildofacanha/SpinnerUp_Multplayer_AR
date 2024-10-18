using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    public Transform target;  // O alvo que o objeto deve olhar

    void Update()
    {
        // Verifica se o alvo está definido
        if (target != null)
        {
            // Faz o objeto olhar na direção do alvo
            transform.LookAt(target);
        }
    }
}
