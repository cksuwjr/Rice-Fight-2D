using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    private static GameLogic _singleton;
    public static GameLogic Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
                _singleton = value;
            else if (_singleton != value)
            {
                Debug.Log($"{nameof(GameLogic)} instance already exists, 파괴합니다.");
                Destroy(value);
            }
        }
    }

    public GameObject LocalPlayerPrefab => localplayerPrefab;
    public GameObject PlayerPrefab => playerPrefab;

    [Header("Prefabs")]
    [SerializeField] private GameObject localplayerPrefab;
    [SerializeField] private GameObject playerPrefab;

    private void Awake()
    {
        Singleton = this;
    }
}
