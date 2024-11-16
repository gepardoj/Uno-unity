using UnityEngine;
using UnityEngine.Scripting;

public class ClientPlayerManager : MonoBehaviour
{
    [SerializeField, RequiredMember] private PlayerData _player;

    public PlayerData Player => _player;
}
