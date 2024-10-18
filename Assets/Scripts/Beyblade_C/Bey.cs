using UnityEngine;
using TMPro; // Certifique-se de incluir a biblioteca TextMeshPro
using UnityEngine.UI; // Certifique-se de incluir o namespace UnityEngine.UI
using Photon.Pun;
using Photon.Realtime;

public class Bey : MonoBehaviour
{
    private Vector3 _initialCenterOfMass;
    private PhotonView _pv;
    private float _rightMove;
    private float _forwardMove;
    private GameObject _currentTrail;

    public Rigidbody rb;
    public GameObject COM, vfxHit, trailPrefab;
    public TMP_Text txtNickName; // Referência ao componente TextMeshProUGUI
    public Image imgNickName; // Referência ao componente Image
    public bool isGrounded = false;

    public Color playerColor = Color.white; // Cor a ser aplicada

    public string beyName;
    public float rpm = 1200;
    public float maxAgularSpeed = 25;
    public float moveForce = 5;
    public bool rootRight, movR, movL, movF, movB;

    public Color botColor = Color.green; // Cor a ser aplicada
    public bool isBot = false;


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

        if (_pv.IsMine || isBot) // Para jogadores e bots controlados localmente
        {
            imgNickName.color = isBot ? botColor : Color.blue;
            txtNickName.text = isBot ? "Bot" : _pv.Owner.NickName;
        }
    }

    void Update()
    {
        if (_pv.IsMine || isBot)
        {
            if (rpm <= 0)
            {
                rb.centerOfMass = _initialCenterOfMass; // para de rodar so desativar
            }
        }
    }

    private void FixedUpdate()
    {
        if (_pv.IsMine || isBot)
        {
            AllMoveForce();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (_pv.IsMine || isBot)
        {
            if (collision.collider.CompareTag("Collider"))
            {
                Vector3 collisionPoint = (transform.position + collision.transform.position) / 2f;
                Instantiate(vfxHit, collisionPoint, Quaternion.identity);
                //PhotonNetwork.Instantiate(vfxHit.name, collisionPoint,Quaternion.identity);
            }
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (_pv.IsMine || isBot)
        {
            if (collision.collider.CompareTag("Stadium"))
            {
                foreach (ContactPoint contact in collision.contacts)
                {
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
        if (_pv.IsMine || isBot)
        {
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
        float radiansPerSecond = (rpm * 2 * Mathf.PI) / 60f;
        float rotationStep = Mathf.Min(radiansPerSecond * Time.fixedDeltaTime, maxAgularSpeed);
        Vector3 rotationDirection = rootRight ? Vector3.up : Vector3.down;
        Quaternion targetRotation = rb.transform.rotation * Quaternion.Euler(rotationDirection * rotationStep * Mathf.Rad2Deg);
        rb.transform.rotation = Quaternion.Slerp(rb.transform.rotation, targetRotation, Time.fixedDeltaTime * 10f);
        rb.angularVelocity = Vector3.ClampMagnitude(rb.angularVelocity, maxAgularSpeed);
    }

    public void BeyMove()
    {
        if (movR && !movL)
        {
            _rightMove = moveForce;
            beyDirectionMove(transform.right, _rightMove);
        }
        else if (!movR && movL)
        {
            _rightMove = moveForce * (-1);
            beyDirectionMove(transform.right, _rightMove);
        }
        else if (movR && movL)
        {
            _rightMove = Random.Range(-moveForce, moveForce);
            beyDirectionMove(transform.right, _rightMove);
        }
        else
        {
            beyDirectionMove(transform.right, 0);
        }

        if (movF && !movB)
        {
            _forwardMove = moveForce;
            beyDirectionMove(transform.forward, _forwardMove);
        }
        else if (!movF && movB)
        {
            _forwardMove = moveForce * (-1);
            beyDirectionMove(transform.forward, _forwardMove);
        }
        else if (movF && movB)
        {
            _forwardMove = Random.Range(-moveForce, moveForce);
            beyDirectionMove(transform.forward, _forwardMove);
        }
        else
        {
            beyDirectionMove(transform.forward, 0);
        }
    }

    public void beyDirectionMove(Vector3 direction, float move)
    {
        rb.AddForce(direction * move, ForceMode.Force);
    }

    public void InstantiateTrail()
    {
        if (trailPrefab != null && _currentTrail == null)
        {
            _currentTrail = Instantiate(trailPrefab, COM.transform.position, transform.rotation);
            _currentTrail.transform.SetParent(COM.transform);
        }
    }

    public void RemoveTrail()
    {
        if (_currentTrail != null)
        {
            Destroy(_currentTrail);
        }
    }
}
