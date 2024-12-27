using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NativeWebSocket;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;

public enum Endpoints : byte
{
    playersInLobby,
    startGame,
    getTurn,
    playCardToDiscardPile,
    currentColor,
    chooseColor,
    drawCardsFromDeck,
    otherDrawsCards, // Other player draws a card without info about what is a card exactly, for secure reasons, client does need to know
    uno,
    winnerOrLooser,
}

enum PlayerFinalCondition { WIN, DEFEAT };


public class API : MonoBehaviour
{
    // just need to be aware when to use, do not mess with data
    public static readonly byte BYTE_NULL = 255;
    public static readonly byte BYTE_SEPARATOR = 254;

    [SerializeField, RequiredMember] private TextMeshProUGUI _playersNumber;

    private bool _isGameStarted = false;

    protected WebSocket websocket;

    //\\ Parsing/Maping Data Structues //\\

    // public static byte ParseEnumToByte<T>(T value) where T : Enum {
    //     return value == null ? BYTE_NULL : (byte)Convert.ChangeType(value, typeof(T));
    // }

    // public static T? ParseByteToEnum<T>(byte value) where T : Enum {
    //     return value == BYTE_NULL ? null : (T)Convert.ChangeType(value, typeof(byte));
    // }

    // players ids, local players comes first
    static (byte Number, string Id)[] ParsePlayersInfo(byte[] data)
    {
        return data.Split(BYTE_SEPARATOR).Select(_ =>
        (
            _[0], // player order number
            Encoding.ASCII.GetString(_[1..])) // player id
        ).ToArray();
    }

#nullable enable
    static string? ParsePlayerId(byte[] data)
#nullable disable
    {
        var chunks = Encoding.ASCII.GetString(data).Split(":");
        var id = chunks[0];
        if (!id.StartsWith("player")) return null;
        return id;
    }

    static CardData[] ParseCards(byte[] data)
    {
        var chunks = data.Split(BYTE_SEPARATOR);
        // print(BitConverter.ToString(data));
        var cards = new List<CardData>();
        foreach (var card in chunks)
        {
            cards.Add(Card.MapFromBytes(card));
        }
        return cards.ToArray();
    }

    static (string, char, int)[] ParseOtherDrawsCards(byte[] data)
    {
        var chunks = data.Split(BYTE_SEPARATOR).Select(_ => Encoding.ASCII.GetString(_)).ToArray();
        var tuples = new List<(string, char, int)>();
        foreach (var chunk in chunks)
        {
            var splited = chunk.Split(":");
            var id = splited[0];
            var sign = splited[1][0]; // + or -
            var cardsNumber = int.Parse(splited[1][1..]);
            // print($"{id} {sign} {cardsNumber}");
            tuples.Add((id, sign, cardsNumber));
        }
        return tuples.ToArray();
    }

    //\\ Sending Data //\\

#nullable enable
    protected void Send(Endpoints endpoint, byte[]? data = null)
#nullable disable
    {
        var bytes = new List<byte>
        {
            (byte)endpoint
        };
        if (data != null) bytes.AddRange(data);
        websocket.Send(bytes.ToArray());
    }

    public void SendPlayCard(Card card, SuitColor? color)
    {
        var colorByte = color == null ? BYTE_NULL : (byte)color;
        var list = new List<byte> { colorByte };
        list.AddRange(card.ToBytes());
        Send(Endpoints.playCardToDiscardPile, list.ToArray());
    }

    public void SendDrawCardsFromDeck()
    {
        Send(Endpoints.drawCardsFromDeck);
    }

    public void SendChosenColor(SuitColor color)
    {
        MultiplayerGame.Instance.CardManager.ColorPicker.SetActive(false);
        Send(Endpoints.chooseColor, new byte[] { (byte)color });
    }

    public void SendUno()
    {
        Send(Endpoints.uno);
    }

    // Receiving messages //\\

    protected void OnMessage(byte[] data)
    {
        if (data == null) return;
        var endpoint = (Endpoints)data[0];
        print($"endpoint = {endpoint}");
        if (endpoint == Endpoints.playersInLobby) OnPlayersInLobby(data);
        else if (endpoint == Endpoints.startGame) StartCoroutine(OnStartGame(data));
        else if (endpoint == Endpoints.getTurn) StartCoroutine(OnGetTurn(data));
        else if (endpoint == Endpoints.playCardToDiscardPile) StartCoroutine(OnPlayedCard(data));
        else if (endpoint == Endpoints.drawCardsFromDeck) StartCoroutine(OnDrawedCards(data));
        else if (endpoint == Endpoints.otherDrawsCards) StartCoroutine(OtherDrawsCards(data));
        else if (endpoint == Endpoints.winnerOrLooser) StartCoroutine(OnWinnerOrLooser(data));
        else if (endpoint == Endpoints.chooseColor) StartCoroutine(OnChooseColor(data));
        else if (endpoint == Endpoints.currentColor) StartCoroutine(OnCurrentColor(data));
    }

    void OnPlayersInLobby(byte[] data)
    {
        var playersInLobby = data[1];
        // _playersNumber.text = $"Players: {playersInLobby}";
    }

    IEnumerator OnStartGame(byte[] data)
    {
        if (Scene.IsMenu) SceneManager.LoadScene(Scene.MULTIPLAYER);
        while (!MultiplayerGame.Instance) yield return null;
        var players = ParsePlayersInfo(data[1..]);
        if (players.Length < 2) throw new Exception("The're should be at least two players");
        MultiplayerGame.Instance.PlayerManager.Player.Id = players[0].Id;
        //MultiplayerGame.Instance.PlayerManager.Player.Avatar.SetSprite(MultiplayerGame.Instance.PlayerManager.PlayerAvatars[players[0].Number]);
        foreach (var player in players[1..]) MultiplayerGame.Instance.PlayerManager.AddPlayer(player.Id, player.Number);
        _isGameStarted = true;
    }

    IEnumerator OnGetTurn(byte[] data)
    {
        while (!_isGameStarted) yield return null;
        var id = ParsePlayerId(data[1..]);
        // foreach (var _ in MultiplayerGame.Instance.PlayerManager.GetAllPlayers()) _.Avatar.Highlight(false);
        var currentPlayer = MultiplayerGame.Instance.PlayerManager.GetPlayerById(id);
        // currentPlayer.Avatar.Highlight(true);
        MultiplayerGame.Instance.PlayerManager.CurrentPlayer = currentPlayer;
        MultiplayerGame.Instance.CardManager.Deck.Highlight(MultiplayerGame.Instance.PlayerManager.IsLocalPlayer(currentPlayer));
    }

    CardData GetCardByOffset(byte[] data, int offset)
    {
        var cards = ParseCards(data[offset..]);
        return cards[0];
    }

    void Cleanup()
    {
        MultiplayerGame.Instance.TimeSlider.Stop();
        var lastCard = MultiplayerGame.Instance.CardManager.LastTouchedCard;
        if (lastCard) lastCard.Glow.Stop();
    }

    // only for local player
    IEnumerator OnPlayedCard(byte[] data)
    {
        while (!_isGameStarted) yield return null;
        Cleanup();
        var res = data[1];
        if (res == 0) yield break;
        print("the card has been played successfully on the server");
        // print(BitConverter.ToString(data));
        var id = ParsePlayerId(data[2..]);
        if (id != null)
        {
            var card = GetCardByOffset(data, 2 + id.Length + 1);
            var player = MultiplayerGame.Instance.PlayerManager.GetPlayerById(id);
            MultiplayerGame.Instance.CardManager.MoveCardFromPlayerToDiscardPile(player, card);
        }
        else
        {
            var card = GetCardByOffset(data, 2);
            MultiplayerGame.Instance.CardManager.CreateCardAndAddToDiscardPile(card);
        }
    }

    // only for local player
    IEnumerator OnDrawedCards(byte[] data)
    {
        while (!_isGameStarted) yield return null;
        var timeToPlayS = data[1];
        print($"drawing new cards...");
        // print($"{BitConverter.ToString(data[2..])}");
        var cards = ParseCards(data[2..]);
        var player = MultiplayerGame.Instance.PlayerManager.Player;
        foreach (var card in cards)
        {
            MultiplayerGame.Instance.CardManager.CreateCardAndAddToPlayer(player, card, timeToPlayS);
        }
        if (timeToPlayS > 0) MultiplayerGame.Instance.TimeSlider.Play(timeToPlayS);
    }

    // getting info about other players
    IEnumerator OtherDrawsCards(byte[] data)
    {
        while (!_isGameStarted) yield return null;
        foreach (var (id, sign, cardsNumber) in ParseOtherDrawsCards(data[1..]))
        {
            var player = MultiplayerGame.Instance.PlayerManager.GetPlayerById(id);
            // print($"player is {player.Id}");
            if (sign == '+')
            {
                foreach (var _ in Enumerable.Range(1, cardsNumber)) MultiplayerGame.Instance.CardManager.CreateFakeCard(player);
            }
            // else if (sign == '-') {
            //     foreach (var _ in Enumerable.Range(1, cardsNumber)) MultiplayerGame.Instance.CardManager.RemoveFakeCard(player);
            // }
            else throw new Exception("Unknow sign operator");
        }
    }

    IEnumerator OnWinnerOrLooser(byte[] data)
    {
        while (!_isGameStarted) yield return null;
        var cond = (PlayerFinalCondition)data[1];
        var id = ParsePlayerId(data[2..]);
        var menuState = cond == PlayerFinalCondition.WIN ? MenuState.won : MenuState.lost;
        if (MultiplayerGame.Instance.PlayerManager.IsLocalPlayer(id))
        {
            Menu.state = menuState;
            Destroy(Connection.Instance.gameObject);
            Connection.Instance = null;
            SceneManager.LoadScene(Scene.MENU);
        }
        else
        {
            if (cond == PlayerFinalCondition.DEFEAT) throw new Exception("You shouldn't see other player's defeating");
            MultiplayerGame.Instance.PlayerManager.OnPlayerWin(id);
        }
    }

    IEnumerator OnChooseColor(byte[] data)
    {
        while (!_isGameStarted) yield return null;
        var colorPicker = MultiplayerGame.Instance.CardManager.ColorPicker;
        colorPicker.SetActive(true);
        colorPicker.GetComponent<RotateAround>().PlaceObjectsAround();
    }

    IEnumerator OnCurrentColor(byte[] data)
    {
        while (!_isGameStarted) yield return null;
        var color = (SuitColor)data[1];
        MultiplayerGame.Instance.CardManager.CurrentColor = color;
    }
}