using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;

public enum Endpoints : byte
{
    playersInLobby,
    startGame,
    getCardsInHand,
    moveCardToDiscardPile,
}

public class API : MonoBehaviour
{
    [SerializeField, RequiredMember] private TextMeshProUGUI _playersNumber;

    protected void OnMessage(byte[] data)
    {
        if (data == null) return;
        var endpoint = (Endpoints)data[0];
        print($"endpoint = {endpoint}");
        if (endpoint == Endpoints.playersInLobby) OnPlayersInLobby(data);
        else if (endpoint == Endpoints.startGame) OnStartGame();
        else if (endpoint == Endpoints.getCardsInHand) StartCoroutine(OnGetCardsInHand(data));
        else if (endpoint == Endpoints.moveCardToDiscardPile) StartCoroutine(OnMoveCardToDiscardPile(data));
    }

    void OnPlayersInLobby(byte[] data)
    {
        var playersInLobby = data[1];
        _playersNumber.text = $"Players: {playersInLobby}";
    }

    void OnStartGame()
    {
        if (Scene.IsMenu()) SceneManager.LoadScene(Scene.MULTIPLAYER);
    }

    IEnumerator OnGetCardsInHand(byte[] data)
    {
        var length = data[1];
        var items = data[2..];
        while (!MultiplayerGame.Instance) yield return null;
        // var iz = 0;
        // foreach (var item in items)
        // {
        //     // print(item);
        //     // if ((iz + 1) % 5 == 0) print("next card");
        //     iz++;
        // }
        var offset = 0;
        for (var i = 0; i < length; i++)
        {
            var type = (CardType)items[offset++];
            var color = (SuitColor)items[offset++];
            var value = (SuitValue)items[offset++];
            var other = (OtherCards)items[offset++];
            var state = (CardState)items[offset++];
            // if (type == CardType.suit) print($"suit card: {color}, {value}, {state}");
            // else if (type == CardType.other) print($"other card: {other}, {state}");
            MultiplayerGame.Instance.CardManager.CreateCardAndAddToPlayer(type,
            (byte)color == 255 ? null : color,
            (byte)value == 255 ? null : value,
            (byte)other == 255 ? null : other,
            state);
        }
    }

    IEnumerator OnMoveCardToDiscardPile(byte[] data)
    {
        while (!MultiplayerGame.Instance) yield return null;
        var offset = 1;
        var type = (CardType)data[offset++];
        var color = (SuitColor)data[offset++];
        var value = (SuitValue)data[offset++];
        var other = (OtherCards)data[offset++];
        var state = (CardState)data[offset++];
        MultiplayerGame.Instance.CardManager.CreateCardAndAddToDiscardPile(type,
            (byte)color == 255 ? null : color,
            (byte)value == 255 ? null : value,
            (byte)other == 255 ? null : other,
            state
        );
    }
}