using UnityEngine;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    public Transform[] spawnPoints; // Array de pontos de spawn
    public GameObject objectToSpawn; // Objeto a ser instanciado
    private List<Transform> spawnList; // Lista de pontos de spawn disponíveis

    void Start()
    {
        // Inicializa a lista de pontos de spawn disponíveis
        spawnList = new List<Transform>(spawnPoints);
    }

    void Update()
    {
        // Verifica se a tecla B foi pressionada
        if (Input.GetKeyDown(KeyCode.B))
        {
            SpawnObject();
        }
    }

    void SpawnObject()
    {
        if (spawnList.Count > 0)
        {
            // Seleciona um ponto de spawn aleatório
            int randomIndex = Random.Range(0, spawnList.Count);

            Transform spawnPoint = spawnList[randomIndex];

            // Instancia o objeto no ponto de spawn
            Instantiate(objectToSpawn, spawnPoint.position, Quaternion.identity);

            // Remove o ponto de spawn da lista de disponíveis
            spawnList.RemoveAt(randomIndex);
        }
        else
        {
            Debug.LogWarning("Nenhum ponto de spawn disponível.");
        }
    }
}
