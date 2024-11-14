using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour
{
    private static GameMaster _instance;
    private CardManager _cardManager;
    private PlayerManager _playerManager;


    public static GameMaster Instance => _instance;
    public CardManager CardManager => _cardManager;
    public PlayerManager PlayerManager => _playerManager;

    void Start()
    {
        if (_instance == null) _instance = this;
        else { if (_instance != this) Destroy(gameObject); }

        _cardManager = GetComponentInChildren<CardManager>();
        _playerManager = GetComponentInChildren<PlayerManager>();

        _cardManager.ManualInit();
        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(0.5f);
        _playerManager.StartGame();
    }

    public void OnWin()
    {
        Menu.state = MenuState.won;
        SceneManager.LoadScene(Scene.MENU);
    }
}
