using UnityEngine;
using TMPro; // Certifique-se de incluir a biblioteca TextMeshPro
using UnityEngine.UI; // Certifique-se de incluir o namespace UnityEngine.UI
using Photon.Pun;
using Photon.Realtime;
public class Bay : MonoBehaviour
{
    private BayCreate _bay;
    private Vector3 _initialCenterOfMass;
    private PhotonView _pv;

    public Rigidbody rb;
    public GameObject COM, vfxHit;
    public TMP_Text txtNickName; // Referência ao componente TextMeshProUGUI
    public Image imgNickName; // Referência ao componente Image

    public Color PlayerColor = Color.white; // Cor a ser aplicada

    public string bayName;
    public float rpm, maxAngularVelocity, mass, impulse;

    void Start()
    {
        _pv = GetComponent<PhotonView>(); // Pegar o componente PhotonView do jogador
        txtNickName.text = _pv.Owner.NickName;
        // Criando uma inst�ncia da classe Player
        _bay = new BayCreate(rb, COM, bayName, mass, impulse);

        if (rb == null)
        {
            Debug.LogError("O Rigidbody não está atribuído ao objeto.");
        }
        else
        {
            rb = GetComponent<Rigidbody>();
            _initialCenterOfMass = rb.centerOfMass;
            rb.centerOfMass = COM.transform.localPosition; // para de rodar so desativar
        }

        if (_pv.IsMine)
        {
            imgNickName.color = PlayerColor;
        }
    }

    void Update()
    {
        if (_pv.IsMine)
        {
            //bay.BlastAttack(otherPlayer);
            if (rpm <= 0)
            {
                rb.centerOfMass = _initialCenterOfMass; // para de rodar so desativar
            }
        }
    }

    public float moveForce = 5;
    public bool rootRight, movR, movL, movF, movB;
    private void FixedUpdate()
    {
        if (_pv.IsMine)
        {
            _pv.RPC("Move", RpcTarget.All);
            _bay.LimitAngle(this.transform, 15f);

            if (isGrounded)
            {
                //_bay.MoveSpeed(this.transform, movR, movL, movF, movB, moveForce);
               // _bay.GravitForce();
            }
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

            // Chama o RPC para todos os jogadores
            _pv.RPC("SparkEmission", RpcTarget.All, collisionPoint);
        }
    }

    public bool isGrounded = false;
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

    [PunRPC]
    void SparkEmission(Vector3 collisionPoint)
    {
        Instantiate(vfxHit, collisionPoint, Quaternion.identity);

        // Log para depuração
        Debug.Log("HIT at " + collisionPoint);
    }

    [PunRPC]
    void Move()
    {
        _bay.MoveRotate(rootRight, rpm, maxAngularVelocity);
    }
}
