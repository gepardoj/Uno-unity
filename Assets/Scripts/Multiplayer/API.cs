using System.Collections;
using System.Collections.Generic;
using System.Text;
using NativeWebSocket;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;

public enum Endpoints : byte
{
    playersInLobby,
    otherPlayersInGame,
    startGame,
    getCardsInHand,
    moveCardToDiscardPile,
    tryMoveCardToDiscardPile,
}

public class API : MonoBehaviour
{
    [SerializeField, RequiredMember] private TextMeshProUGUI _playersNumber;

    protected WebSocket websocket;


    public void SendTryChooseCard(Card card)
    {
        var bytes = new List<byte>
        {
            (byte)Endpoints.tryMoveCardToDiscardPile
        };
        bytes.AddRange(card.ToBytes());
        websocket.Send(bytes.ToArray());
    }

    protected void OnMessage(byte[] data)
    {
        if (data == null) return;
        var endpoint = (Endpoints)data[0];
        print($"endpoint = {endpoint}");
        if (endpoint == Endpoints.playersInLobby) OnPlayersInLobby(data);
        else if (endpoint == Endpoints.otherPlayersInGame) StartCoroutine(OnOtherPlayersInGame(data));
        else if (endpoint == Endpoints.startGame) StartCoroutine(OnStartGame(data));
        else if (endpoint == Endpoints.getCardsInHand) StartCoroutine(OnGetCardsInHand(data));
        else if (endpoint == Endpoints.moveCardToDiscardPile) StartCoroutine(OnMoveCardToDiscardPile(data));
        else if (endpoint == Endpoints.tryMoveCardToDiscardPile) StartCoroutine(IsCardMoved(data));
    }

    void OnPlayersInLobby(byte[] data)
    {
        var playersInLobby = data[1];
        // _playersNumber.text = $"Players: {playersInLobby}";
    }

    IEnumerator OnOtherPlayersInGame(byte[] data)
    {
        while (!MultiplayerGame.Instance) yield return null;
        // foreach (var el in data) print(el);
        var str = Encoding.ASCII.GetString(data[1..]);
        MultiplayerGame.Instance.debugText.text = str;
        // print(str);
        var playersData = str.Split("\n");
        foreach (var playerData in playersData)
        {
            var splited = playerData.Split(":");
            var id = splited[0];
            var cardsNumber = int.Parse(splited[1]);
            MultiplayerGame.Instance.AddPlayer(id, cardsNumber);

        }
    }

    IEnumerator OnStartGame(byte[] data)
    {
        if (Scene.IsMenu) SceneManager.LoadScene(Scene.MULTIPLAYER);
        while (!MultiplayerGame.Instance) yield return null;
        var id = Encoding.ASCII.GetString(data[1..]);
        MultiplayerGame.Instance.PlayerManager.Player.Id = id;
    }

    IEnumerator OnGetCardsInHand(byte[] data)
    {
        var length = data[1];
        var items = data[2..];
        while (!MultiplayerGame.Instance) yield return null;
        for (var i = 0; i < length; i++)
        {
            var (type, color, value, other, state) = Card.MapFromBytes(items[(i * 5)..]);
            var player = MultiplayerGame.Instance.PlayerManager.Player;
            MultiplayerGame.Instance.CardManager.CreateCardAndAddToPlayer(player, type, color, value, other, CardState.opened);
        }
    }

    IEnumerator OnMoveCardToDiscardPile(byte[] data)
    {
        while (!MultiplayerGame.Instance) yield return null;
        var (type, color, value, other, state) = Card.MapFromBytes(data[1..]);
        MultiplayerGame.Instance.CardManager.CreateCardAndAddToDiscardPile(type, color, value, other, state);
    }

    IEnumerator IsCardMoved(byte[] data)
    {
        while (!MultiplayerGame.Instance) yield return null;
        var res = data[1];
        // print($"res {res}");
        MultiplayerGame.Instance.CardManager.MoveCardFromPlayerToDiscardPile(res == 1);
    }
}