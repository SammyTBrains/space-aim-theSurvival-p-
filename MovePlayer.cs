using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    [SerializeField]
    private float _speed;
    [SerializeField]
    private Transform _finalBlowDestination, _player;
    [SerializeField]
    private GameObject _finalBlowTimeline;

    private GameObject _boss;
    public GameObject Boss
    {
        set => _boss = value;
    }

    // Update is called once per frame
    void Update()
    {
        _player.position = Vector3.MoveTowards(_player.position, _finalBlowDestination.position, _speed * Time.deltaTime);
        if(_player.position == _finalBlowDestination.position || Vector3.Distance(_player.position, _finalBlowDestination.position) <= 0.1)
        {
            _finalBlowTimeline.SetActive(true);
            UIManager.Instance.SetBestTime();
            UIManager.Instance.CalculateAndDisplayCoins();
            if (_boss != null)
            {
                _boss.GetComponent<Enemy>().DeleteAllHealthInScene();
                _boss.GetComponent<Enemy>().DeleteAllPowerUpsInScene();
            }
            Destroy(_boss, .37f);
        }
    }
}
