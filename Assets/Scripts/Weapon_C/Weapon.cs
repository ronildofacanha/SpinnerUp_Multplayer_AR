using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Settings")]
    public float fireRate = 0.2f; // Tempo entre os disparos
    public int maxAmmo = 30; // Máximo de balas no carregador
    public Transform firePoint; // Ponto de origem do disparo
    public GameObject bulletPrefab; // Prefab da bala
    public float bulletForce = 20f; // Força aplicada à bala

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
            Debug.LogError("Prefab não foi atribuído.");
            return;
        }
        // Carregue o prefab antecipadamente
        GameObject bullet = Instantiate(bulletPrefab, new Vector3(-1000, -1000, -1000), Quaternion.identity);

        // Destrua imediatamente após a instância para liberar memória
        Destroy(bullet,60);
    }


    void Update()
    {
        // Checa se o jogador quer atirar e se pode atirar
        if (Input.GetButton("Fire1") && Time.time >= _timeToFire)
        {
            // Verifica se há munição
            if (_currentAmmo > 0)
            {
                // Atualiza o tempo para o próximo tiro
                _timeToFire = Time.time + fireRate;
                // Dispara a arma
                Shoot();
            }
            else
            {
                Debug.Log("Sem munição!");
                // Aqui você poderia implementar uma função para recarregar ou outro feedback
            }
        }
    }

    void Shoot()
    {
        // Instancia a bala no ponto de disparo com a rotação da arma
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Aplica uma força à bala na direção em que a arma está apontando
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(firePoint.forward * bulletForce, ForceMode.Impulse);
        }

        // Decrementa a quantidade de munição
        _currentAmmo--;
    }

    public void Reload()
    {
        // Recarrega a arma
        _currentAmmo = maxAmmo;
        Debug.Log("Arma recarregada!");
    }
}
