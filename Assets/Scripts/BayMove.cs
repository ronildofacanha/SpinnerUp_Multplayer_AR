using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class BayMove : MonoBehaviour
{
    [Header("PLAYER")]
    public float spinLife = 3600;
    public float RPS = 3.6f; // Giro por segundo
    public float velMov = 10.0f; // Quanto se move
    public float variateMov = 10.0f; // Quanto varia de movimento
    public float damageAttack = 10;
    public float bayDrag = 2.0f; // Defesa
    public float sentido = 1f;

    [Header("VIEW")]
    public float _speedMovement;
    public float _speedRotation;
    public float _danoTotal;
    public float impulseTotal;
    public bool inFloor;
    public bool isBattle;
    public bool bayBreak;

    [Header("BAY CONFIG")]
    public float bayMass = 1.0f;
    public GameObject COMUP;
    public GameObject COMDOWN;


    [Header("BATTLE CONfIG")]
    public GameManager BattleSystem;


    // [private]
    private Rigidbody rb;

    void StartBattleSystem()
    {
        BattleSystem = FindObjectOfType<GameManager>();
    }
    void Start()
    {
        // PLAYER
        rb = GetComponent<Rigidbody>();
        rb.mass = bayMass;
        spinLife = RPS * 1000;
        StartBattleSystem();
    }

    void Update() // mudar para fixed
    {
        if (!bayBreak)
        {
            BayMovement();
            BayOnFloor();
            SpinLifeUpDate();
            GravitForce();
        }
    }
    Vector3 _reverseDirection;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isBattle = true;
            
            _reverseDirection = (transform.position - collision.transform.position).normalized;

            // float _damageAttack = collision.gameObject.GetComponent<BayMove>().damageAttack;
            float _sMov = collision.gameObject.GetComponent<BayMove>()._speedMovement;
            float _sRotation = collision.gameObject.GetComponent<BayMove>()._speedRotation;

            impulseTotal = (_sMov * _sRotation * BattleSystem.forcaDeImpacto);

            rb.AddForce(_reverseDirection * ImpulsoMax(impulseTotal), ForceMode.Impulse);

            // Verificar se vão ficar grudadas
            BattleTimeMax();
        }
        else
        {
            isBattle = false;
        }
    }
    float ImpulsoMax(float valor)
    {
        if (valor < 0)
        {
            valor = valor * -1;
        }
        if (valor >= BattleSystem.impulseMax)
        {
            valor = BattleSystem.impulseMax;
        }
        if (valor <= 1)
        {
            valor = Random.Range(1f, 5f);
        }
        return valor;
    }

    void BattleTimeMax()
    {
        if (isBattle)
        {
            StartCoroutine(Contar());
        }
    }
    IEnumerator Contar()
    {
        float tempoDecorrido = 0f;
        float duracaoContagem = 1f;

        while (tempoDecorrido <= duracaoContagem)
        {
            // Aguarda o próximo quadro
            yield return null;

            // Atualiza o tempo decorrido
            tempoDecorrido += Time.deltaTime;
        }

        if (isBattle)
        {
            rb.AddForce(_reverseDirection * ImpulsoMax(impulseTotal), ForceMode.Impulse);
            Debug.Log("Contagem concluída!");
        }

        // Quando a contagem chegar a 2 segundos
    }
    private void OnCollisionExit(Collision collision)
    {
        // Check if the collided object has the tag "Player"
        if (collision.gameObject.CompareTag("Player"))
        {
            isBattle = false;
        }
    }
    void setDamage(float dano)
    {
        if (spinLife >= 0)
        {
            spinLife -= dano;
        }
    }
    void SpinLifeUpDate()
    {
        RPS = spinLife / 1000; // atualização

        float _movement = (rb.velocity.magnitude * BattleSystem.fricSpeed) * (60 * Time.deltaTime);

        setDamage(_movement); // Perdendo vida por segundo e movimento

        if (isBattle)
        {
            setDamage(_movement);
        }
    }

    void BayMovement()
    {
        _speedRotation = (RPS * BattleSystem.RPM * BattleSystem.angularForce * Time.deltaTime) * sentido;

        if (_speedRotation > BattleSystem.rotationSpeedMax)
        {
            _speedRotation = BattleSystem.rotationSpeedMax;
            rb.transform.Rotate(0, _speedRotation, 0);
        }
        else
        {
            rb.transform.Rotate(0, _speedRotation, 0);
        }

        // ###FOÇA DE MOVIMENTO###
        _speedMovement = rb.velocity.magnitude;


        // Verifica se a velocidade excede o limite máximo
        if (_speedMovement > BattleSystem.speedMovementMax)
        {
            // Calcula a velocidade desejada para respeitar o limite
            float velocidadeDesejadaMS = BattleSystem.speedMovementMax;
            // Atualiza a velocidade do objeto para a velocidade desejada
            rb.velocity = rb.velocity.normalized * velocidadeDesejadaMS;
        }

        if (!isBattle && inFloor) // Está no chão? rotação por segundo maior que zero?
        {
            // Movimento livre
            Vector3 _movX = transform.right * Random.Range(-variateMov, velMov);
            // Vector3 _movZ = transform.forward * Random.Range(-variateMov, variateMov);

            float totalForceMov = Time.deltaTime * (BattleSystem.RPM * BattleSystem.RPS * BattleSystem.ajusteDoMovimento);

            // Aplica a força de direção ao objeto
            rb.AddForce(_movX * totalForceMov, ForceMode.Acceleration);
        }
    }
    void GravitForce()
    {
        if (RPS >= BattleSystem.minRPS)
        {
            if (inFloor)
            {
                // Centro de massa no chão
                rb.centerOfMass = COMDOWN.transform.localPosition;

                // DOWNFORCE
                rb.AddForce(-transform.up * BattleSystem.downForce * rb.velocity.magnitude);
            }
            else
            {
                // Centro de massa no ar
                rb.centerOfMass = COMUP.transform.localPosition;
            }
            bayBreak = false;
        }
        else if (RPS <= BattleSystem.minRPS)
        {
            rb.centerOfMass = COMUP.transform.localPosition;
            velMov = 0;
            variateMov = 0;
            timeBreak();
        }
    }

    private float tempoDeParada;

    void timeBreak()
    {
        // Incrementa o tempo decorrido desde o início
        tempoDeParada += Time.deltaTime;

        // Verifica se passou 1 segundo
        if (tempoDeParada >= BattleSystem.timeOff)
        {
            spinLife = 0;
            bayBreak = true;
        }
    }

    public void BayOnFloor()
    {
        // Lança um raycast para baixo
        if (Physics.Raycast(transform.position, Vector3.down, BattleSystem.distanceOnGround))
        {
            // O jogador está no chão
            //Debug.Log("Está no chão.");
            inFloor = true;
        }
        else
        {
            // O jogador não está no chão
            //Debug.Log("Não está no chão.");
            inFloor = false;
        }
    }
}