using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("BATTLE CONfIG")]
    public float angularForce = 10.0f; // Força angular
    public float forcaDeImpacto = 0.5f;
    public float ajusteDoMovimento = 0.1f;
    public float fricSpeed = 0.3f;
    public float minRPS = 0.5f;
    public float timeOff = 0.6f;
    public float RPM = 60f; // Rotação por minuto
    public float RPS = 3.6f;
    public float rotationSpeedMax = 50f;
    public float speedMovementMax = 50f;
    public float impulseMax = 50f;
    public float distanceOnGround = 0.2f;
    public float downForce = 1.0f;

}
