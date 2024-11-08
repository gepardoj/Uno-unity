using UnityEngine;
using UnityEngine.Scripting;

public enum Direction { clockwise, counterClockwise }

public class PlayerManager : MonoBehaviour
{
    [SerializeField, RequiredMember] private PlayerData[] _players;
    int _currentPlayerIndex = -1;
    private PlayerData _currentPlayer;
    private Direction _direction = Direction.clockwise;

    public PlayerData[] Players => _players;

    public PlayerData CurrentPlayer => _currentPlayer;
    public Direction Direction
    {
        get { return _direction; }
        set { _direction = value; }
    }

    // public actions

    public void UseCard(Card card)
    {
        CurrentPlayer.Player.UseCard(card);
    }

    public void StartGame()
    {
        NextTurn(true);
    }

    public void PullCards(int amount)
    {
        GameMaster.Instance.CardManager.TakeNewCards(CurrentPlayer, amount,
            CurrentPlayer.PlayerType == PlayerType.Player ? CardState.opened : CardState.closed);
    }

    public void OnChooseColor(SuitColor color)
    {
        CurrentPlayer.Player.OnChooseColor(color);
    }

    public void FinishTurn()
    {
        CurrentPlayer.Player.OnEndTurn();
        NextTurn(true);
    }

    public void PlayCardRule(Card card)
    {
        AttemptChangeDirection(card);
        AttemptSkip(card); // order is important
        AttempDraw(card); // for theese two
    }

    // inner methods

    void AttemptChangeDirection(Card card)
    {
        if (card.Type == CardType.suit && card.Value == SuitValue.reverse)
            Direction = Direction == Direction.clockwise ? Direction.counterClockwise : Direction.clockwise;
    }

    void AttemptSkip(Card card)
    {
        if ((card.Type == CardType.suit && (card.Value == SuitValue.cancel || card.Value == SuitValue._draw))
        || (card.Type == CardType.other && card.Other == OtherCards.wilddraw))
        {
            NextTurn(false);
        }
    }

    void AttempDraw(Card card)
    {
        if (card.Type == CardType.suit && card.Value == SuitValue._draw) PullCards(2);
        else if (card.Type == CardType.other && card.Other == OtherCards.wilddraw) PullCards(4);
    }

    void NextTurn(bool getTurn)
    {
        HighlightCurrentPlayer(false);
        NextPlayer();
        HighlightCurrentPlayer(true);
        if (getTurn) CurrentPlayer.Player.GetTurn();
    }

    void HighlightCurrentPlayer(bool highlight)
    {
        if (CurrentPlayer == null) return;
        CurrentPlayer.Avatar.color = highlight ? Color.yellow : Color.white;
    }

    void NextPlayer()
    {
        if (Direction == Direction.clockwise)
        {
            _currentPlayerIndex++;
            if (_currentPlayerIndex == _players.Length) _currentPlayerIndex = 0;
        }
        else if (Direction == Direction.counterClockwise)
        {
            _currentPlayerIndex--;
            if (_currentPlayerIndex == -1) _currentPlayerIndex = _players.Length - 1;
        }
        _currentPlayer = _players[_currentPlayerIndex];
    }
}