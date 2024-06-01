using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float explosionForce = 1000f; // For�a da explos�o
    public float explosionRadius = 5f; // Raio da explos�o
    public float upwardsModifier = 0.5f; // Modificador de dire��o vertical da explos�o

    void Start()
    {
        Explode();
    }

    void Explode()
    {
        // Encontrar todos os colliders dentro do raio da explos�o
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        // Aplicar for�a a todos os colliders encontrados
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
            {
                // Calcular a dire��o e intensidade da for�a
                Vector3 direction = (hit.transform.position - transform.position).normalized;
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                float force = 1 - (distance / explosionRadius); // Diminui a for�a com base na dist�ncia

                // Aplicar a for�a ao Rigidbody
                rb.AddForce(direction * explosionForce * force, ForceMode.Impulse);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // Desenhar uma esfera para representar o raio da explos�o no editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
