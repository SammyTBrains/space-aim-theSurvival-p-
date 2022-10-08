using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineASource : MonoBehaviour
{
    [SerializeField]
    private AudioSource _aSource, _aSourcePrefab;

   void  Update()
    {
        _aSource.volume = _aSourcePrefab.volume;
    }
}
