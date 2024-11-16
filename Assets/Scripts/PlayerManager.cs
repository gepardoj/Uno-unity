using System;
using System.Collections.Generic;
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
    private LinkedList<PlayerData> _linkedPlayers;
    private bool _isFirstTurn = true;
    private bool _isFirstColoredAction = false;
    private Card _firstCard;
    private LinkedListNode<PlayerData> _currentPlayerNode = null;
    private PlayerData _realPlayer;
    private Direction _direction = Direction.clockwise;

    public LinkedList<PlayerData> Players => _linkedPlayers;

    public PlayerData CurrentPlayer => _currentPlayerNode.Value;
    public PlayerData PrevPlayer => _currentPlayerNode.Previous?.Value ?? _linkedPlayers.Last.Value;
    public PlayerData NextPlayer => _currentPlayerNode.Next?.Value ?? _linkedPlayers.First.Value;
    public PlayerData RealPlayer => _realPlayer;
    public Direction Direction
    {
        get { return _direction; }
        set { _direction = value; }
    }

    ///// public actions

    public void StartGame()
    {
        _linkedPlayers = new LinkedList<PlayerData>(_players);
        // _currentPlayerNode = _linkedPlayers.First;
        foreach (var player in _players)
        {
            if (player.PlayerType == PlayerType.Player)
            {
                _realPlayer = player;
            }
        }
        if (!_realPlayer)
        {
            throw new Exception("Real player not found");
        }
        var res = ApplyFirstCardRule();
        // if (res)
        NextTurn();
        // {
        //     CurrentPlayer.Avatar.Highlight(true);
        //     CurrentPlayer.Player.OnGetTurn();
        // }
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
        RealPlayer.SaidUno = true;
        if (PrevPlayer.SaidUno != null && PrevPlayer.SaidUno == false)
        {
            UnoPenalty();
        }
    }

    public void UnoPenalty()
    {
        if (PrevPlayer)
        {
            PrevPlayer.SaidUno = null;
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
    bool ApplyFirstCardRule()
    {
        _firstCard = GameMaster.Instance.CardManager.GetFirstCardInDiscardPile();
        if (_firstCard.IsWild)
        {
            // the first player skips turn and the next player declares the color, and gets turn
            NextTurn(false);
            return false;
        }
        else if (_firstCard.IsColoredAction)
        {
            // apply the effect to the next player, after the first player finishes turn
            _isFirstColoredAction = true;
        }
        return true;
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

        WinCondition();
        NextTurn();
    }

    void WinCondition()
    {
        if (CurrentPlayer.Cards.Count == 0)
        {
            DetachPlayer();
            CurrentPlayer.StatusText.AddPlay(Const.WIN_TEXT);
            CurrentPlayer.Avatar.Play();
            if (CurrentPlayer == RealPlayer)
            {
                GameMaster.Instance.OnWin();
            }
        }
    }

    void DetachPlayer()
    {
        var res = Players.Remove(CurrentPlayer);
        if (!res) throw new Exception($"Failed to remove player {CurrentPlayer.name}");
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
        IterateNextPlayer();
        PrevPlayer.Avatar.Highlight(false);
        CurrentPlayer.Avatar.Highlight(true);
        if (initiator)
        {
            var shouldDeclareColor = _isFirstTurn && (_firstCard?.IsWild ?? false);
            _isFirstTurn = false;
            CurrentPlayer.Player.OnGetTurn(shouldDeclareColor, PrevPlayer?.SaidUno);
        }
        else CurrentPlayer.StatusText.AddPlay(Const.SKIP_TEXT);
    }

    void IterateNextPlayer()
    {
        if (Direction == Direction.clockwise)
        {
            _currentPlayerNode = _currentPlayerNode?.Next ?? _linkedPlayers.First;
        }
        else if (Direction == Direction.counterClockwise)
        {
            _currentPlayerNode = _currentPlayerNode?.Previous ?? _linkedPlayers.Last;
        }
        print($"current player is [{_currentPlayerNode.Value.name}]");
    }
}