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

    [SerializeField] private MoveController moveController;
    [SerializeField] private AnimManager animManager;

    private void OnDestroy()
    {
        list.Remove(Id);    
    }

    private void FixedUpdate()
    {
        animManager.AnimateBasedOnSpeed();

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

    public void Move(int direction, bool jump)
    {
        if(direction != 0)
            gameObject.transform.GetChild(0).localScale = new Vector3(-direction, 1, 1);
        moveController.Move(direction, jump);
    }

    [MessageHandler((ushort)ServerToClientId.playerSpawned)]
    private static void SpawnPlayer(Message message)
    {
        Spawn(message.GetUShort(), message.GetString(), message.GetVector2());
    }

    [MessageHandler((ushort)ServerToClientId.Move)]
    private static void AcceptMove(Message message)
    {
        if(list.TryGetValue(NetworkManager.Singleton.Client.Id, out Player player))
        {
            player.Move(message.GetInt(), message.GetBool());
        }
    }
}
