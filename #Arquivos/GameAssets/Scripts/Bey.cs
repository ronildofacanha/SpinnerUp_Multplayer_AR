using UnityEngine;
using TMPro; // Certifique-se de incluir a biblioteca TextMeshPro
using UnityEngine.UI; // Certifique-se de incluir o namespace UnityEngine.UI
using Photon.Pun;
using Photon.Realtime;
public class Bey : MonoBehaviour
{
    private Vector3 _initialCenterOfMass;
    private PhotonView _pv;
    private float rightMove;
    private float forwardMove;

    public Rigidbody rb;
    public GameObject COM, vfxHit;
    public TMP_Text txtNickName; // Referência ao componente TextMeshProUGUI
    public Image imgNickName; // Referência ao componente Image

    public Color PlayerColor = Color.white; // Cor a ser aplicada

    public string beyName;
    public float rpm = 1200;
    public float maxAgular = 25; // 0 à 25
   // public float maxEuler = 15f; // 0° à 90°
    public float mass;
    public bool isGrounded = false;
    public float moveForce = 5;
    public bool rootRight, movR, movL, movF, movB;

    void Start()
    {
        _pv = GetComponent<PhotonView>(); // Pegar o componente PhotonView do jogador

        if (rb == null)
        {
            Debug.LogError("O Rigidbody não está atribuído ao objeto.");
        }
        else
        {
            rb = GetComponent<Rigidbody>();
            _initialCenterOfMass = rb.centerOfMass;
            rb.centerOfMass = COM.transform.localPosition; // para de rodar so desativar
            InstantiateTrail();
        }

        if (_pv.IsMine)
        {
            imgNickName.color = PlayerColor;
            txtNickName.text = _pv.Owner.NickName;
        }
    }

    void Update()
    {
        if (_pv.IsMine)
        {
            if (rpm <= 0)
            {
                rb.centerOfMass = _initialCenterOfMass; // para de rodar so desativar
            }
        }
    }

   
    private void FixedUpdate()
    {
        if (_pv.IsMine)
        {
            AllMoveForce();
        }
    }

    public float ConvertRPMToRPS(float rpm)
    {
        return rpm / 60f;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Collider"))
        {
            Vector3 collisionPoint = (transform.position + collision.transform.position) / 2f;
            Instantiate(vfxHit, collisionPoint, Quaternion.identity);
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (_pv.IsMine)
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

    void OnCollisionExit(Collision collision)
    {
        if (_pv.IsMine)
        {
            // Quando o objeto sai da colisão, ele não está mais no chão
            if (collision.collider.CompareTag("Stadium"))
            {
                isGrounded = false;
            }
        }
    }

    void AllMoveForce()
    {
        BeyRotate();
        
        if (isGrounded)
        {
            BeyMove();
        }
    }

    public void BeyRotate()
    {
        // Converte o RPM para radianos por segundo
        float radiansPerSecond = (rpm * 2 * Mathf.PI) / 60f;
        float rotationStep = Mathf.Min(radiansPerSecond * Time.fixedDeltaTime, maxAgular);

        // Define a direção de rotação
        Vector3 rotationDirection = rootRight ? Vector3.up : Vector3.down;

        // Calcula a nova rotação alvo
        Quaternion targetRotation = rb.transform.rotation * Quaternion.Euler(rotationDirection * rotationStep * Mathf.Rad2Deg);

        // Aplica a rotação suavemente usando Slerp (interpolação esférica)
        rb.transform.rotation = Quaternion.Slerp(rb.transform.rotation, targetRotation, Time.fixedDeltaTime * 10f);

        // Limita a velocidade angular da Rigidbody para evitar bugs
        rb.angularVelocity = Vector3.ClampMagnitude(rb.angularVelocity, maxAgular);
    }

    public void BeyMove()
    {

        if (movR && !movL) // right
        {
            rightMove = moveForce;
            beyDirectionMove(transform.right, rightMove);
            //Debug.Log("right");
        }
        else
        if (!movR && movL) // left
        {
            rightMove = moveForce * (-1);
            beyDirectionMove(transform.right, rightMove);
            // Debug.Log("left");
        }
        else
        if (movR && movL) // right+left
        {
            rightMove = Random.Range(-moveForce, moveForce);
            beyDirectionMove(transform.right, rightMove);
            //Debug.Log("right && left");
        }
        else
        {
            beyDirectionMove(transform.right, 0);
        }

        if (movF && !movB) // forward
        {
            forwardMove = moveForce;
            beyDirectionMove(transform.forward, forwardMove);
            //Debug.Log("forward");
        }
        else
         if (!movF && movB) // backward
        {
            forwardMove = moveForce * (-1); ;
            beyDirectionMove(transform.forward, forwardMove);
            //Debug.Log("backward");
        }
        else
        if (movF && movB) // FB
        {
            forwardMove = Random.Range(-moveForce, moveForce);
            beyDirectionMove(transform.forward, forwardMove);
            //Debug.Log("forward && backward");
        }
        else
        {
            beyDirectionMove(transform.forward, 0);
        }
    }
    public void beyDirectionMove(Vector3 direction, float move)
    {
        rb.AddForce(direction * move,ForceMode.Force);
    }
    /*
    public void BeyLimitAngle()
    {
        // Obtém a rotação atual do objeto
        Quaternion currentRotation = transform.rotation;
        Vector3 currentEuler = currentRotation.eulerAngles;

        // Ajusta os ângulos para estarem no intervalo [-180, 180]
        currentEuler.x = Mathf.DeltaAngle(0, currentEuler.x);
        currentEuler.z = Mathf.DeltaAngle(0, currentEuler.z);

        // Limita os ângulos de Euler ao máximo permitido nos eixos X e Z
        currentEuler.x = Mathf.Clamp(currentEuler.x, -maxEuler, maxEuler);
        currentEuler.z = Mathf.Clamp(currentEuler.z, -maxEuler, maxEuler);

        // Converte de volta para uma rotação quaternion, mantendo o ângulo Y inalterado
        Quaternion limitedRotation = Quaternion.Euler(currentEuler.x, currentEuler.y, currentEuler.z);

        // Aplica a rotação limitada ao transform
        transform.rotation = limitedRotation;
    }
    */



    public GameObject trailPrefab; // Prefab do objeto com Trail Renderer
    private GameObject currentTrail;

    // Função para instanciar o trail
    public void InstantiateTrail()
    {
        if (trailPrefab != null && currentTrail == null)
        {
            // Instancia o trail como um filho do player
            currentTrail = Instantiate(trailPrefab, COM.transform.position, transform.rotation);
            currentTrail.transform.SetParent(COM.transform);
        }
    }

    // Função para remover o trail
    public void RemoveTrail()
    {
        if (currentTrail != null)
        {
            Destroy(currentTrail);
        }
    }
}
