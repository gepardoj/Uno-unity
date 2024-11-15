using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;
using UnityEngine.UI;

public enum MenuState { main, won, lobby }

public class Menu : MonoBehaviour
{
    public static MenuState state = MenuState.main;

    [SerializeField, RequiredMember] private Color _mainColor = new(0.1f, 0.1f, 0.1f);
    [SerializeField, RequiredMember] private Color _winColor = new(0, 0.5f, 0);
    [SerializeField, RequiredMember] private string _winText = "You won";
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
        else if (state == MenuState.lobby) OnLobby();
    }

    private void OnMainMenu()
    {
        _image.color = _mainColor;
        _status.text = "";
        _mainBlock.SetActive(true);
        _lobbyBlock.SetActive(false);
    }

    public void OnWin()
    {
        OnMainMenu();
        _image.color = _winColor;
        _status.text = _winText;
    }

    public void OnLobby()
    {
        _image.color = _mainColor;
        _status.text = _lobbyText;
        _mainBlock.SetActive(false);
        _lobbyBlock.SetActive(true);
        StartCoroutine(Connect());
    }

    IEnumerator Connect()
    {
        yield return new WaitForSeconds(0.5f);
        Connection.Instance.Init();
    }

    public void Singleplayer()
    {
        SceneManager.LoadScene(Scene.SINGLEPLAYER);
    }

    public void Multiplayer()
    {
        state = MenuState.lobby;
        OnLobby();
        //SceneManager.LoadScene(Scene.MULTIPLAYER);
    }

    public void BackFromLobby()
    {
        Connection.Instance.Close();
        state = MenuState.main;
        OnMainMenu();
    }
}
