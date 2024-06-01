using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMotionEffect : MonoBehaviour
{
    public KeyCode stopTime = KeyCode.Space; // Tecla para ativar/desativar o efeito
    public GameObject player; // Refer�ncia ao jogador
    public float slowDownFactor = 0.5f; // Fator de desacelera��o para os outros objetos

    private bool isSlowMotion = false; // Indica se a c�mera lenta est� ativada

    void Update()
    {
        // Verifica se a tecla "T" foi pressionada para alternar entre c�mera lenta e tempo normal
        if (Input.GetKeyDown(stopTime))
        {
            isSlowMotion = !isSlowMotion; // Inverte o estado da c�mera lenta

            // Se a c�mera lenta estiver ativada, define a escala de tempo para 0.5 (50% da velocidade normal)
            if (isSlowMotion)
            {
                Time.timeScale = slowDownFactor;
                Time.fixedDeltaTime = slowDownFactor * Time.deltaTime;

            }
            else // Se a c�mera lenta estiver desativada, restaura a escala de tempo para 1 (tempo normal)
            {
                Time.timeScale = 1f;
                Time.fixedDeltaTime = Time.deltaTime;
            }
        }
    }
}