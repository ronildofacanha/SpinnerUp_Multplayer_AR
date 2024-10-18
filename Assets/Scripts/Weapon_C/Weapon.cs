using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Settings")]
    public float fireRate = 0.2f; // Tempo entre os disparos
    public int maxAmmo = 30; // M�ximo de balas no carregador
    public Transform firePoint; // Ponto de origem do disparo
    public GameObject bulletPrefab; // Prefab da bala
    public float bulletForce = 20f; // For�a aplicada � bala

    [Header("Hand Target")]
    public Transform rightHandTarget;
    public Transform leftHandTarget;

    private float _timeToFire = 0f;
    private int _currentAmmo;

    void Start()
    {
        // Iniciar com o carregador cheio
        _currentAmmo = maxAmmo;
        preLoad();
    }

    void preLoad()
    {
        if (bulletPrefab == null)
        {
            Debug.LogError("Prefab n�o foi atribu�do.");
            return;
        }
        // Carregue o prefab antecipadamente
        GameObject bullet = Instantiate(bulletPrefab, new Vector3(-1000, -1000, -1000), Quaternion.identity);

        // Destrua imediatamente ap�s a inst�ncia para liberar mem�ria
        Destroy(bullet,60);
    }


    void Update()
    {
        // Checa se o jogador quer atirar e se pode atirar
        if (Input.GetButton("Fire1") && Time.time >= _timeToFire)
        {
            // Verifica se h� muni��o
            if (_currentAmmo > 0)
            {
                // Atualiza o tempo para o pr�ximo tiro
                _timeToFire = Time.time + fireRate;
                // Dispara a arma
                Shoot();
            }
            else
            {
                Debug.Log("Sem muni��o!");
                // Aqui voc� poderia implementar uma fun��o para recarregar ou outro feedback
            }
        }
    }

    void Shoot()
    {
        // Instancia a bala no ponto de disparo com a rota��o da arma
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Aplica uma for�a � bala na dire��o em que a arma est� apontando
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(firePoint.forward * bulletForce, ForceMode.Impulse);
        }

        // Decrementa a quantidade de muni��o
        _currentAmmo--;
    }

    public void Reload()
    {
        // Recarrega a arma
        _currentAmmo = maxAmmo;
        Debug.Log("Arma recarregada!");
    }
}
