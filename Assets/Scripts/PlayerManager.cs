using UnityEngine;
using UnityEngine.Scripting;

enum Direction { clockwise, counterClockwise }

public class PlayerManager : MonoBehaviour
{
    [SerializeField, RequiredMember] private PlayerData[] _players;
    int _currentPlayerIndex = -1;
    private PlayerData _currentPlayer;

    public PlayerData[] Players => _players;

    public PlayerData CurrentPlayer => _currentPlayer;

    // public actions

    public void UseCard(Card card)
    {
        CurrentPlayer.Player.UseCard(card);
    }

    public void StartGame()
    {
        NextTurn();
    }

    public void OnPullCards()
    {
        CurrentPlayer.Player.OnPullCards();
    }

    public void OnChooseColor(SuitColor color)
    {
        CurrentPlayer.Player.OnChooseColor(color);
    }

    public void FinishTurn()
    {
        CurrentPlayer.Player.OnEndTurn();
        NextTurn();
    }

    // inner methods

    void NextTurn()
    {
        HighlightCurrentPlayer(false);
        NextPlayer();
        HighlightCurrentPlayer(true);
        CurrentPlayer.Player.GetTurn();
    }

    void HighlightCurrentPlayer(bool highlight)
    {
        if (CurrentPlayer == null) return;
        CurrentPlayer.Avatar.color = highlight ? Color.yellow : Color.white;
    }

    void NextPlayer()
    {
        _currentPlayerIndex++;
        if (_currentPlayerIndex == _players.Length) _currentPlayerIndex = 0;
        _currentPlayer = _players[_currentPlayerIndex];
    }
}