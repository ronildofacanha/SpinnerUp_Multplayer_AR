using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LoginManager : MonoBehaviourPunCallbacks
{
    public GameObject p_Login, p_Lobby, p_Status;
    public InputField playerNameInput;
    public Button loginButton;
    public Button quickMatchButton;
    public Text statusText;

    [SerializeField]
    private GameObject playerPrefab;

    void Start()
    {
        // Configurar o bot�o de login
        loginButton.onClick.AddListener(OnLoginButtonClicked);
        quickMatchButton.onClick.AddListener(OnQuickMatchButtonClicked);

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

        // Criar op��es da sala
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 4
        };
       
        PhotonNetwork.CreateRoom("4V4", roomOptions, TypedLobby.Default);  // Criar uma nova sala
    }


    public GameObject[] spawnPositions; // Array de posi��es de respawn
    public override void OnJoinedRoom()
    {
        Debug.Log("Entrou na sala: " + PhotonNetwork.CurrentRoom.Name);
        statusText.text = "Entrou na sala: " + PhotonNetwork.CurrentRoom.Name;

        p_Lobby.SetActive(false);

        // Obter o �ndice do jogador baseado no ActorNumber
        int playerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1; // �ndice do jogador na sala
        Vector3 spawnPosition;

        // Verificar se o �ndice est� dentro do vetor de posi��es de respawn
        if (playerIndex >= 0 && playerIndex < spawnPositions.Length)
        {
            spawnPosition = spawnPositions[playerIndex].transform.position;
        }
        else
        {
            // Se n�o houver posi��o definida, usar uma posi��o padr�o
            spawnPosition = new Vector3(0, 0.5f, 0); // 1 unidade acima do ponto de origem
            Debug.LogWarning("Nenhuma posi��o de respawn espec�fica dispon�vel, usando posi��o padr�o.");
        }

        // Instanciar o jogador na cena com posi��o inicial personalizada
        PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("Desconectado do servidor Photon por causa: {0}", cause);
        statusText.text = "Desconectado do servidor Photon.";

        // Tentar reconectar automaticamente
        PhotonNetwork.ConnectUsingSettings();
    }
}
