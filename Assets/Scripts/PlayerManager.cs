using System.Linq;
using UnityEngine;
using UnityEngine.Scripting;

enum Direction { clockwise, counterClockwise }

public class PlayerManager : MonoBehaviour
{
    [SerializeField, RequiredMember] private PlayerData[] _players;
    int _currentPlayer = -1;

    public PlayerData[] Players => _players;

    private PlayerData _realPlayer;

    public PlayerData RealPlayer => _realPlayer;

    void Start()
    {
        _realPlayer = GetComponentInChildren<RealPlayer>().GetComponent<PlayerData>();
    }

    public void StartGame()
    {
        NextTurn();
    }

    public void FinishTurn()
    {
        GetCurrentPlayer().Player.OnEndTurn();
        NextTurn();
    }

    void NextTurn()
    {
        HighlightCurrentPlayer(false);
        NextPlayer();
        HighlightCurrentPlayer(true);
        GetCurrentPlayer().Player.GetTurn();
    }

    public PlayerData GetPlayerByHolder(GameObject cardsHolder)
    {
        return _players.First(el => el.CardsHolder == cardsHolder);
    }

    void HighlightCurrentPlayer(bool highlight)
    {
        var info = GetCurrentPlayer();
        if (info == null) return;
        info.Avatar.color = highlight ? Color.yellow : Color.white;
    }

    PlayerData GetCurrentPlayer()
    {
        if (_currentPlayer < 0) return null;
        return _players[_currentPlayer];
    }

    void NextPlayer()
    {
        _currentPlayer++;
        if (_currentPlayer == _players.Length) _currentPlayer = 0;
    }
}