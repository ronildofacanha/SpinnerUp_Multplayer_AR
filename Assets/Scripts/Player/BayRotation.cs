using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BayRotation : MonoBehaviour
{
    public Rigidbody rb;
    public bool right = true;
    public float rpm = 3600;
    public float mass, maxAngularVelocity;
    public GameObject COM;
    void Start()
    {
        if (rb == null)
        {
            Debug.LogError("O Rigidbody não está atribuído ao objeto.");
        }
        else
        {
            rb = GetComponent<Rigidbody>();
            rb.centerOfMass = COM.transform.localPosition; // para de rodar so desativar
        }
    }

    void FixedUpdate()
    {
         MoveRotate();
        // GravitForce();
        
        if (isGrounded)
        {

        }
    }

    public void MoveRotate()
    {
        if (right)
        {
            // Aplica a rotação ao Transform
            rb.transform.Rotate(Vector3.up, rpm * Time.fixedDeltaTime);
        }
        else
        {
            // Aplica a rotação ao Transform
            rb.transform.Rotate(Vector3.down, (rpm * Time.fixedDeltaTime));
        }

        if (rb.angularVelocity.magnitude > maxAngularVelocity)
        {
            rb.angularVelocity = rb.angularVelocity.normalized * maxAngularVelocity;
        }
    }

    public float baseGravity = -9.81f; // Gravidade padrão
    public void GravitForce()
    {
        // Calcula a gravidade personalizada baseada na massa do objeto
        Vector3 customGravity = new Vector3(0, baseGravity * rb.mass, 0);

        // Aplica a força da gravidade personalizada
        rb.AddForce(customGravity, ForceMode.Acceleration);

    }

    public bool isGrounded = false;
    void OnCollisionStay(Collision collision)
    {
            // Verifica se a colisão é com o chão
            if (collision.collider.CompareTag("Stadium"))
            {
                foreach (ContactPoint contact in collision.contacts)
                {
                    // Verifica se o ponto de contato está perto do centro de massa
                    if (contact.thisCollider.CompareTag("COM") && contact.normal.y > 0.5f)
                    {
                        isGrounded = true;
                    }
                }
            }
    }
}
