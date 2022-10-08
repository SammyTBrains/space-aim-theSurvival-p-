using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayerHolder : MonoBehaviour
{
    private static MovePlayerHolder _instance;
    public static MovePlayerHolder Instance
    {
        get => _instance;
    }

    private void Awake()
    {
        _instance = this;
    }

    [SerializeField]
    private GameObject _movePlayer, _player;

    public void SetMovePlayerActive(GameObject bossObject)
    {
        Quaternion target = Quaternion.Euler(0, 0, 0);
        _player.transform.rotation = Quaternion.Slerp(_player.transform.rotation, target, 1);
        _movePlayer.SetActive(true);
        bossObject.GetComponent<Enemy>().DeleteAllHealthInScene();
        bossObject.GetComponent<Enemy>().DeleteAllPowerUpsInScene();
        _movePlayer.GetComponent<MovePlayer>().Boss = bossObject;
    }
}
