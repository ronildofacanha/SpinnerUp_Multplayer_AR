using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WeaponChange : MonoBehaviour
{
    public GameObject[] weaponPrefabs; // Lista de prefabs das armas
    private GameObject currentWeapon;
    private int currentWeaponIndex = -1; // Armazena o índice da arma atual

    [Header("RigBuilder Settings")]
    public TwoBoneIKConstraint rightHand;
    public TwoBoneIKConstraint leftHand;
    public RigBuilder rig;

    [Header("Hand Target")]
    public Transform rightHandTarget;
    public Transform leftHandTarget;

    void Start()
    {

        if (weaponPrefabs.Length > 0 && weaponPrefabs[0] != null)
        {
            SelectWeapon(0);
        }
        else
        {
            Debug.LogError("Nenhuma arma ou arma do índice 0 não atribuída!");
        }

        preLoad();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            HandTesting();
        }
    }

    void HandTesting()
    {

        rightHand.data.target = rightHandTarget;
        leftHand.data.target = leftHandTarget;
        rig.Build(); // Atualiza o rig
    }

    void Switch() // fução para troca de arma
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            SelectWeapon(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            SelectWeapon(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            SelectWeapon(2);
        }
    }

    void SelectWeapon(int index)
    {
        if (index < 0 || index >= weaponPrefabs.Length)
        {
            Debug.LogError("Índice da arma fora do intervalo!");
            return;
        }

        // Verifica se a arma selecionada já está equipada
        if (currentWeaponIndex == index)
        {
            Debug.Log("A arma já está equipada.");
            return;
        }

        // Destrói a arma atual, se existir
        if (currentWeapon != null)
        {
            Destroy(currentWeapon);
        }

        // Instancia e configura a nova arma como filha do objeto que contém este script
        currentWeapon = Instantiate(weaponPrefabs[index], transform);

        // Obtém o componente Weapon da arma instanciada
        Weapon WeaponScript = currentWeapon.GetComponent<Weapon>();

        rightHand.data.target = WeaponScript.rightHandTarget;
        leftHand.data.target = WeaponScript.leftHandTarget;
        rig.Build(); // Atualiza o rig

        // Atualiza o índice da arma atual
        currentWeaponIndex = index;
    }

    void preLoad()
    {
        if (weaponPrefabs == null)
        {
            Debug.LogError("O vetor de prefabs não foi atribuído ou está vazio.");
            return;
        }

        for (int i = 0; i < weaponPrefabs.Length; i++)
        {
            // Carregue o prefab antecipadamente
            GameObject weapon = Instantiate(weaponPrefabs[i], new Vector3(-1000, -1000, -1000), Quaternion.identity);

            // Destrua imediatamente após a instância para liberar memória
            Destroy(weapon,60);
        }
    }
}
