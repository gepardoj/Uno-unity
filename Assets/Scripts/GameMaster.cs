using System.Collections;
using UnityEngine;
using UnityEngine.Scripting;

public class GameMaster : MonoBehaviour
{
    private static GameMaster _instance;
    private CardManager _cardManager;
    private PlayerManager _playerManager;

    [SerializeField, RequiredMember] private GameObject _winScreen;

    public static GameMaster Instance => _instance;
    public CardManager CardManager => _cardManager;
    public PlayerManager PlayerManager => _playerManager;

    void Start()
    {
        if (_instance == null) _instance = this;
        else { if (_instance != this) Destroy(gameObject); }

        _winScreen.SetActive(false);
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
        _winScreen.SetActive(true);
    }
}
