using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public GameObject p_Login, p_Lobby, p_Status;
    public InputField playerNameInput;
    public Button loginButton;
    public Button quickMatchButton;
    public Text statusText;
    public int numberOfPlayers = 1;
    [SerializeField] private GameObject playerPrefab;
    public Transform[] spawnPoints; // Array de pontos de spawn
    private List<Transform> spawnList; // Lista de pontos de spawn dispon�veis

    private bool[] occupiedPositions;

    void Start()
    {
        // Configurar o bot�o de login
        loginButton.onClick.AddListener(OnLoginButtonClicked);
        quickMatchButton.onClick.AddListener(OnQuickMatchButtonClicked);

        // Inicializa a lista de pontos de spawn dispon�veis
        spawnList = new List<Transform>(spawnPoints);

        p_Login.SetActive(true);
        p_Lobby.SetActive(false);
        p_Status.SetActive(false);
    }

    void OnLoginButtonClicked()
    {
        p_Login.SetActive(false);
        p_Status.SetActive(true);
        string playerName = playerNameInput.text;

        if (string.IsNullOrEmpty(playerName))
        {
            statusText.text = "Por favor, insira um nome de jogador.";
            return;
        }

        PhotonNetwork.NickName = playerName; // Definir o nome do jogador para Photon
        PhotonNetwork.ConnectUsingSettings(); // Conectar ao servidor Photon
    }

    void OnQuickMatchButtonClicked()
    {
        // Tentar entrar em uma sala aleat�ria
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Conectado ao servidor Photon");
        statusText.text = "Conectado ao servidor Photon.";

        // Entrar no lobby ap�s a conex�o
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Entrou no lobby");
        statusText.text = "Entrou no lobby.";
        p_Login.SetActive(false);
        p_Status.SetActive(false);
        p_Lobby.SetActive(true);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogWarning("Falha ao entrar em uma sala aleat�ria, criando uma nova sala");
        statusText.text = "Falha ao entrar em uma sala aleat�ria, criando uma nova sala.";

        // Criar op��es da sala com o n�mero m�ximo de jogadores
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = numberOfPlayers // N�mero m�ximo de jogadores na sala (jogadores + bots)
        };

        string roomName = "Room_" + Random.Range(00001, 99999); // Gera um nome de sala aleat�rio
        PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Entrou na sala: " + PhotonNetwork.CurrentRoom.Name);
        statusText.text = "Entrou na sala: " + PhotonNetwork.CurrentRoom.Name;

        p_Lobby.SetActive(false);

        int totalPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        Debug.Log("N�mero total de jogadores na sala: " + totalPlayers);

        if (spawnList.Count > 0)
        {
            int playerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;

            int randomIndex = Random.Range(0, spawnList.Count);

            Transform spawnPoint = spawnList[randomIndex];

            GameObject playerObject = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, Quaternion.identity);

            // Opcional: Ajustar o nome do jogador ou qualquer configura��o adicional
            playerObject.name = $"Player_{playerIndex}";

            // Se voc� quiser remover o ponto de spawn da lista para evitar conflitos
            spawnList.RemoveAt(randomIndex);
        }
        else
        {
            Debug.LogWarning("Nenhum ponto de spawn dispon�vel.");
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("Desconectado do servidor Photon por causa: {0}", cause);
        statusText.text = "Desconectado do servidor Photon.";

        // Tentar reconectar automaticamente
        PhotonNetwork.ConnectUsingSettings();
    }

    void Update()
    {
        // Verifica se o jogador est� na sala e se a tecla 'B' foi pressionada
        if (PhotonNetwork.InRoom && Input.GetKeyDown(KeyCode.B))
        {
            CreateBots();
        }
    }

    public GameObject botPrefab;
    public void CreateBots()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (spawnList.Count > 0)
            {
                int randomIndex = Random.Range(0, spawnList.Count);

                Transform spawnPoint = spawnList[randomIndex];

                PhotonNetwork.Instantiate(botPrefab.name, spawnPoint.position, Quaternion.identity);

                // Se voc� quiser remover o ponto de spawn da lista para evitar conflitos
                spawnList.RemoveAt(randomIndex);
            }
            else
            {
                Debug.LogWarning("Nenhum ponto de spawn dispon�vel.");
            }
          
        }
    }
}
