using UnityEngine;

public class BayCreate
{
    protected Rigidbody rb;
    protected string name;
    protected float valMin, impulse;
    protected GameObject COM;
    protected float mass;
    Vector3 direction; // Para obter a dire��o

    public BayCreate(Rigidbody rb, GameObject COM, string name, float mass, float impulse)
    {
        this.rb = rb;
        this.COM = COM;
        this.name = name;
        this.mass = mass;
        this.impulse = impulse;
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

    public float smoothTime = 0.1f; // Tempo de suavização da rotação

    public float SpeedRegulation = 10; // 10X VEL

    public void MoveRotate(bool right, float rpm,float maxAngularVelocity)
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
    public void LimitAngle(Transform transform, float maxAngle)
    {
        // Obtém a rotação atual do objeto
        Quaternion currentRotation = transform.rotation;
        Vector3 currentEuler = currentRotation.eulerAngles;

        // Ajusta os ângulos para estarem no intervalo [-180, 180]
        currentEuler.x = Mathf.DeltaAngle(0, currentEuler.x);
        currentEuler.z = Mathf.DeltaAngle(0, currentEuler.z);

        // Limita os ângulos de Euler ao máximo permitido nos eixos X e Z
        currentEuler.x = Mathf.Clamp(currentEuler.x, -maxAngle, maxAngle);
        currentEuler.z = Mathf.Clamp(currentEuler.z, -maxAngle, maxAngle);

        // Converte de volta para uma rotação quaternion, mantendo o ângulo Y inalterado
        Quaternion limitedRotation = Quaternion.Euler(currentEuler.x, currentEuler.y, currentEuler.z);

        // Aplica a rotação limitada ao transform
        transform.rotation = limitedRotation;
    }


    public void MovePlayer(float moveSpeed)
    {
        // Gera um valor aleatório entre 0 e movForce
        float randomValue = Random.Range(-moveSpeed, moveSpeed);

        // Gera uma direção aleatória no plano XZ
        Vector3 randomDirection = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1)).normalized;

        // Aplica a força na direção aleatória
        rb.AddForce(randomDirection * randomValue);
    }

    private float rightMove;
    private float forwardMove;
    public void MoveSpeed(Transform transform, bool right, bool left, bool forward, bool backward, float moveSpeed)
    {
       
        if(right && !left) // right
        {
            rightMove = moveSpeed;
            addForceDir(transform.right, rightMove);
            //Debug.Log("right");
        }
        else 
        if (!right && left) // left
        {
            rightMove = moveSpeed * (-1);
            addForceDir(transform.right, rightMove);
           // Debug.Log("left");
        }
        else
        if (right && left) // right+left
        {
            rightMove = Random.Range(-moveSpeed, moveSpeed);
            addForceDir(transform.right, rightMove);
            //Debug.Log("right && left");
        }
        else
        {
            addForceDir(transform.right, 0);
        }

        if (forward && !backward) // forward
        {
            forwardMove = moveSpeed;
            addForceDir(transform.forward, forwardMove);
            //Debug.Log("forward");
        }
        else
         if (!forward && backward) // backward
        {
            forwardMove = moveSpeed * (-1); ;
            addForceDir(transform.forward, forwardMove);
            //Debug.Log("backward");
        }
        else
        if (forward && backward) // FB
        {
            forwardMove = Random.Range(-moveSpeed, moveSpeed);
            addForceDir(transform.forward, forwardMove);
            //Debug.Log("forward && backward");
        }
        else
        {
            addForceDir(transform.forward, 0);
        }
    } 

    public void addForceDir(Vector3 direction, float move)
    {
        rb.AddForce(direction * move);
    }

    public float baseGravity = -9.81f; // Gravidade padrão
    public void GravitForce()
    {
        // Calcula a gravidade personalizada baseada na massa do objeto
        Vector3 customGravity = new Vector3(0, baseGravity * rb.mass, 0);

        // Aplica a força da gravidade personalizada
        rb.AddForce(customGravity, ForceMode.Acceleration);

    }
}
