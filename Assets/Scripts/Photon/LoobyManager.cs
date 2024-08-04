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
        // Configurar o botão de login
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
        // Tentar entrar em uma sala aleatória
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Conectado ao servidor Photon");
        statusText.text = "Conectado ao servidor Photon.";

        // Entrar no lobby após a conexão
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
        Debug.LogWarning("Falha ao entrar em uma sala aleatória, criando uma nova sala");
        statusText.text = "Falha ao entrar em uma sala aleatória, criando uma nova sala.";

        // Criar opções da sala
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 4
        };
       
        PhotonNetwork.CreateRoom("4V4", roomOptions, TypedLobby.Default);  // Criar uma nova sala
    }


    public GameObject[] spawnPositions; // Array de posições de respawn
    public override void OnJoinedRoom()
    {
        Debug.Log("Entrou na sala: " + PhotonNetwork.CurrentRoom.Name);
        statusText.text = "Entrou na sala: " + PhotonNetwork.CurrentRoom.Name;

        p_Lobby.SetActive(false);

        // Obter o índice do jogador baseado no ActorNumber
        int playerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1; // Índice do jogador na sala
        Vector3 spawnPosition;

        // Verificar se o índice está dentro do vetor de posições de respawn
        if (playerIndex >= 0 && playerIndex < spawnPositions.Length)
        {
            spawnPosition = spawnPositions[playerIndex].transform.position;
        }
        else
        {
            // Se não houver posição definida, usar uma posição padrão
            spawnPosition = new Vector3(0, 0.5f, 0); // 1 unidade acima do ponto de origem
            Debug.LogWarning("Nenhuma posição de respawn específica disponível, usando posição padrão.");
        }

        // Instanciar o jogador na cena com posição inicial personalizada
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
