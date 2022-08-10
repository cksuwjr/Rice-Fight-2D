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

    Rigidbody2D rb;
    private void OnDestroy()
    {
        list.Remove(Id);    
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {

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

    public static void Move(ushort id, int direction, bool jump)
    {
        if (list.TryGetValue(id, out Player player))
        {
            if (direction != 0)
                player.transform.GetChild(1).localScale = new Vector3(-direction, 1, 1);

            if(NetworkManager.Singleton.Client.Id == id)
                player.moveController.Move(direction, jump);
        }
    }
    public void PositionUpdated(Vector3 position)
    {
        rb.MovePosition(position); 
        // 1.  transform.position = position;     오브젝트 위치 즉시 순간이동,  모든 콜리더가 리지드바디 위치 재계산, 성능저하
        // 2.  rb.position = position;            다음 물리 시뮬레이션 이후 오브젝트 위치 순간이동, 10배 정도 tranfrom보다 빠름
        // 3.  rb.MovePosition(position);         자연스러운 움직임
    }
    [MessageHandler((ushort)ServerToClientId.playerSpawned)]
    private static void SpawnPlayer(Message message)
    {
        Spawn(message.GetUShort(), message.GetString(), message.GetVector2());
    }

    [MessageHandler((ushort)ServerToClientId.Move)]
    private static void SendMoveAccept(Message message)
    {//NetworkManager.Singleton.Client.Id
        Move(message.GetUShort(), message.GetInt(), message.GetBool());
    }

    [MessageHandler((ushort)ServerToClientId.PoseUpdate)]
    private static void PoseUpdateAccept(Message message)
    {
        ushort id = message.GetUShort();
        if (list.TryGetValue(id, out Player player))
        {
            if(id != NetworkManager.Singleton.Client.Id)
                player.transform.position = message.GetVector2();
        }
    }
    [MessageHandler((ushort)ServerToClientId.AnimUpdate)]
    private static void AnimUpdateAccept(Message message)
    {
        ushort id = message.GetUShort();
        bool[] animParameter = message.GetBools(3);
        if (list.TryGetValue(id, out Player player))
        {
            if (id != NetworkManager.Singleton.Client.Id)
            {
                player.animManager.AnimSet("isIdle", animParameter[0]);
                player.animManager.AnimSet("isWalk", animParameter[1]);
                player.animManager.AnimSet("isJump", animParameter[2]);
            }
                
        }
    }
}
