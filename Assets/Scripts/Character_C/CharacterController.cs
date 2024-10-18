using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public float moveSpeed = 3.5f;
    public float rotateSpeed = 100.0f;
    public float smoothTime = 0.1f; // Tempo para suavizar a transi��o

    private Rigidbody _rb;
    private Animator _anim;

    // Vari�veis para suaviza��o
    private float currentBlendH;
    private float currentBlendV;
    private float currentTurn;
    private float blendHVelocity;
    private float blendVVelocity;
    private float turnVelocity;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _anim = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        // Movimento
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;

        // Rota��o usando o mouse
        float mouseX = Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime;
        Vector3 rotateY = new Vector3(0, mouseX, 0);

        // Aplicar rota��o e movimento ao Rigidbody
        _rb.MoveRotation(_rb.rotation * Quaternion.Euler(rotateY));
        _rb.MovePosition(_rb.position + transform.forward * movement.z * moveSpeed * Time.deltaTime +
        transform.right * movement.x * moveSpeed * Time.deltaTime);

        // Suavizar os par�metros do Animator
        currentBlendH = Mathf.SmoothDamp(currentBlendH, movement.x, ref blendHVelocity, smoothTime);
        currentBlendV = Mathf.SmoothDamp(currentBlendV, movement.z, ref blendVVelocity, smoothTime);

        // Configura��o dos par�metros do Animator
        _anim.SetFloat("BlendH", currentBlendH);
        _anim.SetFloat("BlendV", currentBlendV);

        // Executar anima��o de rota��o apenas quando o player estiver parado
        if (movement.magnitude == 0)
        {
            currentTurn = Mathf.SmoothDamp(currentTurn, mouseX, ref turnVelocity, smoothTime);
            _anim.SetFloat("Turn", currentTurn); // Par�metro para a rota��o
        }
        else
        {
            currentTurn = Mathf.SmoothDamp(currentTurn, 0, ref turnVelocity, smoothTime);
            _anim.SetFloat("Turn", currentTurn); // Reseta a anima��o de rota��o quando o player estiver se movendo
        }
    }
}
