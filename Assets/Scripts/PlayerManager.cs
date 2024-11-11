using System;
using UnityEngine;
using UnityEngine.Scripting;

public enum Direction { clockwise, counterClockwise }

interface IPlayerActions
{
    // Three basic actions, player can do, each of them finish turn
    void PlayCard(Card card, SuitColor? color); // color is used for wild card
    void PullCards(int amount, bool initiator);
    void Uno();
}

public class PlayerManager : MonoBehaviour, IPlayerActions
{
    static readonly string SKIP_TEXT = "Skip";
    static readonly string DRAW_TEXT = "Draw";
    static readonly string REVERSE_TEXT = "Reverse";

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

    ///// public actions

    public void StartGame()
    {
        NextTurn();
    }

    public void PlayCard(Card card, SuitColor? color = null)
    {
        var res = GameMaster.Instance.CardManager.TryMoveCardToDrop(CurrentPlayer.Cards, card, color);
        if (!res) throw new Exception("The card is not match last drop");
        PlayCardRule(card);
        FinishTurn();
    }

    public void PullCards(int amount, bool initiator = true)
    {
        GameMaster.Instance.CardManager.TakeNewCards(CurrentPlayer, amount,
            CurrentPlayer.PlayerType == PlayerType.Player ? CardState.opened : CardState.closed);
        CurrentPlayer.StatusText.AddPlay($"{DRAW_TEXT} {amount}");
        if (initiator) FinishTurn();
    }

    public void Uno()
    {

    }

    public void OnChooseCard(Card card)
    {
        CurrentPlayer.Player.OnChooseCard(card);
    }

    public void OnChooseColor(SuitColor color)
    {
        CurrentPlayer.Player.OnChooseColor(color);
    }

    /// inner methods

    void FinishTurn()
    {
        CurrentPlayer.Player.OnEndTurn();
        NextTurn();
    }

    void PlayCardRule(Card card)
    {
        AttemptChangeDirection(card);
        AttemptSkip(card); // order is important
        AttempDraw(card); // for theese two
    }

    void AttemptChangeDirection(Card card)
    {
        if (card.Type == CardType.suit && card.Value == SuitValue.reverse)
        {
            Direction = Direction == Direction.clockwise ? Direction.counterClockwise : Direction.clockwise;
            CurrentPlayer.StatusText.AddPlay(REVERSE_TEXT);
        }
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
        if (card.Type == CardType.suit && card.Value == SuitValue._draw)
            PullCards(CardManager.DRAW_CARDS_N, false);
        else if (card.Type == CardType.other && card.Other == OtherCards.wilddraw)
            PullCards(CardManager.WILDDRAW_CARDS_N, false);
    }

    void NextTurn(bool initiator = true)
    {
        HighlightCurrentPlayer(false);
        NextPlayer();
        HighlightCurrentPlayer(true);
        if (initiator) CurrentPlayer.Player.GetTurn();
        else CurrentPlayer.StatusText.AddPlay(SKIP_TEXT);
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