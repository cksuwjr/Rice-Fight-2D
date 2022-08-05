using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;
public class Player : MonoBehaviour
{
    public static Dictionary<ushort, Player> list = new Dictionary<ushort, Player>();   //  < id, Player.cs > 담는 자료구조생성
    public ushort Id { get; private set; }  // 개인 ID
    public bool IsLocal { get; private set; }  //  내 플레이어인가
    private string username;

    [SerializeField] private AnimManager animManager;       // AnimManager.cs

    private void OnDestroy()
    {
        list.Remove(Id);    
    }
    private void Move(Vector2 newPosition)   //  서버로부터 ServerToClientId.playerMovement 받으면 Player.cs 하단의 단에서 해당움직임을 표출한 Player.cs 호출.
    {
        transform.position = newPosition;                     // 이동 좌표 받아와서 이동시키기

        if (IsLocal)                                         // 내 Player.cs 면
        {
            animManager.AnimateBasedOnSpeed();
        }
    }

    public static void Spawn(ushort id, string username, Vector2 position)   //  플레이어 소환
    {
        Player player;
        if (id == NetworkManager.Singleton.Client.Id)  // 본인이면
        {
            player = Instantiate(GameLogic.Singleton.LocalPlayerPrefab, position, Quaternion.identity).GetComponent<Player>();
            player.IsLocal = true;
        }
        else  // 타인이면
        {
            player = Instantiate(GameLogic.Singleton.PlayerPrefab, position, Quaternion.identity).GetComponent<Player>();
            player.IsLocal = false;
        }

        player.name = $"Player {id} ({(string.IsNullOrEmpty(username) ? "Guest" : username)})";
        player.Id = id;
        player.username = username;

        list.Add(id, player);

    }

    [MessageHandler((ushort)ServerToClientId.playerSpawned)]
    private static void SpawnPlayer(Message message)
    {
        Spawn(message.GetUShort(), message.GetString(), message.GetVector2());
    }
    [MessageHandler((ushort)ServerToClientId.playerMovement)]
    private static void PlayerMovement(Message message)
    {
        if (list.TryGetValue(message.GetUShort(), out Player player))
            player.Move(message.GetVector2());
    }
}
