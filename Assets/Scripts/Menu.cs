using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;
using UnityEngine.UI;

public enum MenuState { main, won, lost, lobby }

public class Menu : MonoBehaviour
{
    public static MenuState state = MenuState.main;

    [SerializeField, RequiredMember] private Color _mainColor = new(0.1f, 0.1f, 0.1f);
    [SerializeField, RequiredMember] private Color _winColor = new(0, 0.5f, 0);
    [SerializeField, RequiredMember] private string _winText = "You won";
    [SerializeField, RequiredMember] private Color _lostColor = new(0.5f, 0, 0);
    [SerializeField, RequiredMember] private string _lostText = "You lost";
    [SerializeField, RequiredMember] private string _lobbyText = "Lobby";
    [SerializeField, RequiredMember] private TextMeshProUGUI _status;
    [SerializeField, RequiredMember] private Button _singleplayer;
    [SerializeField, RequiredMember] private Button _multiplayer;
    [SerializeField, RequiredMember] private Button _backFromLobby;
    [SerializeField, RequiredMember] private GameObject _mainBlock;
    [SerializeField, RequiredMember] private GameObject _lobbyBlock;

    private Image _image;

    void Start()
    {
        _image = GetComponent<Image>();
        _singleplayer.onClick.AddListener(Singleplayer);
        _multiplayer.onClick.AddListener(Multiplayer);
        _backFromLobby.onClick.AddListener(BackFromLobby);
        if (state == MenuState.main) OnMainMenu();
        else if (state == MenuState.won) OnWin();
        else if (state == MenuState.lost) OnLost();
        else if (state == MenuState.lobby) OnLobby();
    }

    void OnMainMenu()
    {
        _image.color = _mainColor;
        _status.text = "";
        _mainBlock.SetActive(true);
        _lobbyBlock.SetActive(false);
    }

    void OnWin()
    {
        OnMainMenu();
        _image.color = _winColor;
        _status.text = _winText;
    }

    void OnLost()
    {
        OnMainMenu();
        _image.color = _lostColor;
        _status.text = _lostText;
    }

    void OnLobby()
    {
        _image.color = _mainColor;
        _status.text = _lobbyText;
        _mainBlock.SetActive(false);
        _lobbyBlock.SetActive(true);
        Connection.Instance.AttemptConnect();
    }

    void Singleplayer()
    {
        SceneManager.LoadScene(Scene.SINGLEPLAYER);
    }

    void Multiplayer()
    {
        state = MenuState.lobby;
        OnLobby();
    }

    void BackFromLobby()
    {
        Connection.Instance.AttemptClose();
        state = MenuState.main;
        OnMainMenu();
    }
}
