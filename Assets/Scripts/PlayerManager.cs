using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Scripting;

enum Direction { clockwise, counterClockwise }


public class PlayerManager : MonoBehaviour
{
    public delegate List<Card> GetCardsCB(GameObject holder);

    [SerializeField, RequiredMember] private PlayerData[] _players;
    int _currentPlayer = -1;

    public PlayerData[] Players => _players;

    public void ManualInit(GetCardsCB[] playersCallbacks)
    {
        for (var i = 0; i < _players.Length; i++)
        {
            var cards = playersCallbacks[i](_players[i].CardsHolder);
            _players[i].Cards = cards;
        }
    }

    public void NextTurn()
    {
        HighlightCurrentPlayer(false);
        NextPlayer();
        HighlightCurrentPlayer(true);
        GetCurrentPlayer().Player.GetTurn();
    }

    public void OnEndTurn()
    {
        GetCurrentPlayer().Player.OnEndTurn();
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