using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    private static TimeManager _instance;
    public static TimeManager Instance
    {
        get => _instance;
    }

    private void Awake()
    {
        _instance = this;
    }

    private bool _countTime;
    public bool CountTime
    {
        set => _countTime = value;
    }

    private int _time;
    public int Time
    {
        get => _time;
    }

    // Start is called before the first frame update
    void Start()
    {
        _countTime = true;
        StartCoroutine(TimeRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        UIManager.Instance.TimeValue = _time;
    }

    IEnumerator TimeRoutine()
    {
        while (_countTime)
        {
            yield return new WaitForSeconds(1.0f);
            _time++;
            Debug.Log("Time in sec " + _time);
        }
    }
}
