using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;
using UnityEngine.UI;

public enum MenuState { main, won }

public class Menu : MonoBehaviour
{
    public static MenuState state = MenuState.main;

    [SerializeField, RequiredMember] private Color _mainColor = new(0.1f, 0.1f, 0.1f);
    [SerializeField, RequiredMember] private Color _winColor = new(0, 0.5f, 0);
    [SerializeField, RequiredMember] private string _winText = "You won";
    [SerializeField, RequiredMember] private TextMeshProUGUI _status;
    [SerializeField, RequiredMember] private Button _singleplayer;
    [SerializeField, RequiredMember] private Button _multiplayer;

    private Image _image;

    void Start()
    {
        _image = GetComponent<Image>();
        _singleplayer.onClick.AddListener(Singleplayer);
        _multiplayer.onClick.AddListener(Multiplayer);
        if (state == MenuState.main) OnMainMenu();
        else if (state == MenuState.won) OnWin();
    }

    private void OnMainMenu()
    {
        _image.color = _mainColor;
        _status.text = "";
    }

    public void OnWin()
    {
        _image.color = _winColor;
        _status.text = _winText;
    }

    public void Singleplayer()
    {
        SceneManager.LoadScene(Scene.SINGLEPLAYER);
    }

    public void Multiplayer()
    {
        SceneManager.LoadScene(Scene.MULTIPLAYER);
    }
}
