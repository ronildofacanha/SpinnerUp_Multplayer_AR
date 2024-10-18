using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    public bool cursor = true;
    public RectTransform crosshair; // Refer�ncia para o RectTransform da imagem do crosshair

    //LookAt
    public GameObject aimWorld;
    public float rayDistance = 25;
    public float speed = 25;

    private Vector3 targetPos;


    private void Start()
    {
        mouseSetting();
        crosshairSetting();
    }

    void crosshairSetting()
    {
        if (crosshair == null)
        {
            Debug.LogError("Crosshair RectTransform n�o est� atribu�do.");
            return;
        }
        // Ajusta o crosshair para o centro da tela
        crosshair.anchoredPosition = Vector2.zero;
    }

    private void mouseSetting()
    {
        if (Cursor.visible && !cursor)
        {
            //Mant�m o cursor escondido e bloqueado
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void FixedUpdate()
    {
        RayPos();
    }

    private void LookAtPos()
    {
        // Calcular a posi��o central da tela
        Vector3 centerScreenPos = new Vector3(Screen.width / 2, Screen.height / 2, rayDistance);

        // Converter a posi��o da tela para a posi��o mundial
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(centerScreenPos);

        // Definir a posi��o alvo
        targetPos = worldPos;

        // Interpolar suavemente, mas de forma mais r�pida
        aimWorld.transform.position = Vector3.Lerp(aimWorld.transform.position, targetPos, speed);
    }

    private void RayPosBacup()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        targetPos = ray.origin + ray.direction * rayDistance;

        // Vetor da c�mera para o targetPos
        Vector3 cameraToTarget = targetPos - Camera.main.transform.position;

        // Calcule o �ngulo entre a dire��o da c�mera e a dire��o do targetPos
        float angle = Vector3.Angle(cameraToTarget, Camera.main.transform.forward);

        // Verifique se o �ngulo est� dentro do intervalo de -90 a 90 graus
        if (angle <= 90)
        {
            // Verifique a dire��o do vetor para garantir que est� dentro do intervalo de -90 a 90 graus
            if (Vector3.Dot(cameraToTarget, Camera.main.transform.forward) > 0)
            {
                aimWorld.transform.position = Vector3.Lerp(aimWorld.transform.position, targetPos, speed * Time.deltaTime);
            }
            else
            {
                // Se estiver fora do intervalo, mantenha o aimWorld na posi��o atual ou trate conforme necess�rio
                aimWorld.transform.position = Vector3.Lerp(aimWorld.transform.position, aimWorld.transform.position, speed * Time.deltaTime);
            }
        }
        else
        {
            // Se o �ngulo estiver fora do intervalo, mantenha o aimWorld na posi��o atual ou trate conforme necess�rio
            aimWorld.transform.position = Vector3.Lerp(aimWorld.transform.position, aimWorld.transform.position, speed * Time.deltaTime);
        }
    }

    private void RayPos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        targetPos = ray.origin + ray.direction * rayDistance;

        // Vetor da c�mera para o targetPos
        Vector3 cameraToTarget = targetPos - Camera.main.transform.position;

        // Obter o �ngulo horizontal e vertical
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
        Vector3 up = Camera.main.transform.up;

        float horizontalAngle = Vector3.SignedAngle(forward, Vector3.ProjectOnPlane(cameraToTarget, up), up);
        float verticalAngle = Vector3.SignedAngle(forward, Vector3.ProjectOnPlane(cameraToTarget, right), right);

        // Verifique se os �ngulos est�o dentro do intervalo de -90 a 90 graus
        if (Mathf.Abs(horizontalAngle) <= 90 && Mathf.Abs(verticalAngle) <= 90)
        {
            aimWorld.transform.position = Vector3.Lerp(aimWorld.transform.position, targetPos, speed * Time.deltaTime);
        }
        else
        {
            // Se os �ngulos estiverem fora do intervalo, mantenha o aimWorld na posi��o atual ou trate conforme necess�rio
            aimWorld.transform.position = Vector3.Lerp(aimWorld.transform.position, aimWorld.transform.position, speed * Time.deltaTime);
        }
    }
}
