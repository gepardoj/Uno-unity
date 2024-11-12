using System;
using UnityEngine;
using UnityEngine.Scripting;

public enum Direction { clockwise, counterClockwise }

interface IPlayerActions
{
    // Three basic actions, player can do, each of them finish turn
    void PlayCard(Card card, SuitColor? color); // color is used for wild card
    void DrawCards(int amount, bool initiator);
    void Uno();
    // Additional
    void UnoPenalty(); // previous player draw 2 cards
}

public class PlayerManager : MonoBehaviour, IPlayerActions
{
    [SerializeField, RequiredMember] private PlayerData[] _players;
    // [SerializeField, RequiredMember] private LinkedList<PlayerData> _players2;
    private bool _isFirstTurn = true;
    private bool _isFirstColoredAction = false;
    private Card _firstCard;
    private int _currentPlayerIndex = -1;
    private PlayerData _currentPlayer;
    private PlayerData _prevPlayer;
    private PlayerData _playerController;
    private Direction _direction = Direction.clockwise;

    public PlayerData[] Players => _players;

    public PlayerData CurrentPlayer => _currentPlayer;
    public PlayerData PrevPlayer => _prevPlayer;
    public PlayerData PlayerController => _playerController;
    public Direction Direction
    {
        get { return _direction; }
        set { _direction = value; }
    }

    ///// public actions

    public void StartGame()
    {
        foreach (var player in _players)
        {
            if (player.PlayerType == PlayerType.Player)
            {
                _playerController = player;
            }
        }
        if (!_playerController)
        {
            throw new Exception("Real player not found");
        }
        ApplyFirstCardRule();
        NextTurn();
    }

    public void PlayCard(Card card, SuitColor? color = null)
    {
        var res = GameMaster.Instance.CardManager.TryMoveCardToDrop(CurrentPlayer.Cards, card, color);
        if (!res) throw new Exception("The card is not match last drop");
        PlayCardRule(card);
        FinishTurn();
    }

    public void DrawCards(int amount, bool initiator = true)
    {
        CurrentPlayer.DrawCards(amount);
        if (initiator) FinishTurn();
    }

    public void Uno()
    {
        print($"{_playerController.name} say uno");
        PlayerController.SaidUno = true;
        PlayerController.StatusText.AddPlay(Const.UNO_TEXT);
    }

    public void UnoPenalty()
    {
        if (PrevPlayer)
        {
            PrevPlayer.DrawCards(Const.UNO_PENALTY_N);
            CurrentPlayer.StatusText.AddPlay(Const.UNO_TEXT);
        }
    }

    public void OnChooseCard(Card card)
    {
        CurrentPlayer.Player.OnChosenCard(card);
    }

    public void OnChooseColor(SuitColor color)
    {
        CurrentPlayer.Player.OnChosenColor(color);
    }

    // inner methods

    // ATTENTION: CURRENT PLAYER IS NULL!
    void ApplyFirstCardRule()
    {
        _firstCard = GameMaster.Instance.CardManager.GetFirstCardInDrop();
        if (_firstCard.IsWild)
        {
            // the first player skips turn and the next player declares the color, and gets turn
            NextTurn(false);
        }
        else if (_firstCard.IsColoredAction)
        {
            // apply the effect to the next player, after the first player finishes turn
            _isFirstColoredAction = true;
        }
    }

    void FinishTurn()
    {
        CurrentPlayer.Player.OnEndTurn();
        if (_isFirstColoredAction)
        {
            _isFirstColoredAction = false;
            PlayCardRule(_firstCard);
        }
        _isFirstTurn = false;

        // WinCondition();
        NextTurn();
    }

    // void WinCondition()
    // {
    //     if (CurrentPlayer.Cards.Count == 0)
    //     {
    //         DetachPlayer();
    //         CurrentPlayer.StatusText.AddPlay(Const.WIN_TEXT);
    //         CurrentPlayer.Avatar.Play();
    //     }
    // }

    // void DetachPlayer()
    // {
    //     Players.Remove(CurrentPlayer);
    // }

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
            CurrentPlayer.StatusText.AddPlay(Const.REVERSE_TEXT);
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
            DrawCards(Const.SKIP_DRAW_CARDS_N, false);
        else if (card.Type == CardType.other && card.Other == OtherCards.wilddraw)
            DrawCards(Const.WILDDRAW_CARDS_N, false);
    }

    void NextTurn(bool initiator = true)
    {
        HighlightCurrentPlayer(false);
        NextPlayer();
        HighlightCurrentPlayer(true);
        if (initiator)
        {
            var shouldDeclareColor = _isFirstTurn && (_firstCard?.IsWild ?? false);
            _isFirstTurn = false;
            CurrentPlayer.Player.OnGetTurn(shouldDeclareColor, PrevPlayer?.SaidUno);
        }
        else CurrentPlayer.StatusText.AddPlay(Const.SKIP_TEXT);
    }

    void HighlightCurrentPlayer(bool highlight)
    {
        if (CurrentPlayer == null) return;
        CurrentPlayer.Avatar.Highlight(highlight);
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
        _prevPlayer = _currentPlayer;
        _currentPlayer = _players[_currentPlayerIndex];
    }
}