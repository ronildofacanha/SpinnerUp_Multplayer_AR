using UnityEngine;

public class Bayblade
{
    protected Rigidbody rb;
    protected string bayName;
    protected float RPM,valMin,valMax, impulse;
    protected GameObject COM;
    Vector3 direction; // Para obter a dire��o

    public Bayblade(Rigidbody rigidbody, GameObject centerOfMass, string name, float rpm, float valMin, float valMax, float impulse)
    {
        this.rb = rigidbody;
        this.COM = centerOfMass;
        this.bayName = name;
        this.RPM = rpm;
        this.valMin = valMin;
        this.valMax = valMax;
        this.impulse = impulse;
    }

    public void DisplayInfo()
    {
        Debug.Log("Nome: " + bayName + ", RPS: " + RPM);
    }
    public void blastOff()
    {
        direction = rb.velocity.normalized;
        rb.AddForce(direction * impulse, ForceMode.Impulse);
    }

    public void BlastAttack(Rigidbody otherPlayerRigidbody)
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            // Calcula a dire��o para o outro jogador
            Vector3 direction = (otherPlayerRigidbody.position - rb.position).normalized;

            // Aplica o impulso na dire��o do outro jogador
            rb.AddForce(direction * impulse, ForceMode.Impulse);
        }
    }
    public void Movement()
    {
        GravitForce();

        float speedRotation = RPM * Time.deltaTime;
        rb.transform.Rotate(0, speedRotation, 0);

        direction = rb.velocity.normalized * Random.Range(-valMin, valMax);

        rb.AddForce(direction, ForceMode.Force);
    }

    public void GravitForce()
    {
        rb.centerOfMass = COM.transform.localPosition;
    }

    public void Hit(GameObject vfx_hit, Collider selfCollider, Collider otherCollider)
    {
        // Verifica se a colisão ocorreu com o jogador
        if (otherCollider.CompareTag("Player"))
        {
            // Calcula a posição média entre os dois objetos
            Vector3 collisionPoint = (selfCollider.transform.position + otherCollider.transform.position) / 2f;

            // Instancia o objeto vfx_hit nessa posição média
            GameObject hitEffect = GameObject.Instantiate(vfx_hit, collisionPoint, Quaternion.identity);

            // Destroi o objeto instanciado após 1 segundo
            GameObject.Destroy(hitEffect, 1f);
            Debug.Log("HIT "+otherCollider.tag);
        }
    }
}
