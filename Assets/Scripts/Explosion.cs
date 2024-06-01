using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float explosionForce = 1000f; // Força da explosão
    public float explosionRadius = 5f; // Raio da explosão
    public float upwardsModifier = 0.5f; // Modificador de direção vertical da explosão

    void Start()
    {
        Explode();
    }

    void Explode()
    {
        // Encontrar todos os colliders dentro do raio da explosão
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        // Aplicar força a todos os colliders encontrados
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
            {
                // Calcular a direção e intensidade da força
                Vector3 direction = (hit.transform.position - transform.position).normalized;
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                float force = 1 - (distance / explosionRadius); // Diminui a força com base na distância

                // Aplicar a força ao Rigidbody
                rb.AddForce(direction * explosionForce * force, ForceMode.Impulse);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // Desenhar uma esfera para representar o raio da explosão no editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
