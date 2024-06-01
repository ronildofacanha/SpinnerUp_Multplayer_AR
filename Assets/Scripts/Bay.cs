using UnityEngine;

public class Bay : MonoBehaviour
{
    private Bayblade bay;
    public string bayName;
    public float RPM, valMin, valMax, impulse;
    public Rigidbody rb, otherPlayer;
    public GameObject COM, vfx_hit;

    private Collider myCollider;

    void Start()
    {
        // Criando uma inst�ncia da classe Player
        bay = new Bayblade(rb, COM, bayName, RPM, valMin, valMax, impulse);
        myCollider = GetComponent<Collider>();
    }

    void Update()
    {
        bay.BlastAttack(otherPlayer);

        bay.Movement();

        // Obter a dire��o do vetor de velocidade normalizado
        Vector3 direction = rb.velocity.normalized;

        // Desenhar uma linha na cena para debugar a dire��o do vetor de velocidade
        Debug.DrawRay(transform.position, direction * 10f, Color.blue);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player")) 
        {
            // Log a message indicating a collision with the player
            Debug.Log("Player collision detected.");

            // Calcular a posição média entre os dois objetos colididos
            Vector3 collisionPoint = (transform.position + other.transform.position) / 2f;

            // Instanciar o objeto vfx_hit na posição média
            Instantiate(vfx_hit, collisionPoint, Quaternion.identity);
        }
    }
}
