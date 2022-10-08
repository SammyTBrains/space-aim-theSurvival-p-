using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField]
    private Transform _camPos;
    [SerializeField]
    private float _waitSeconds;

    private void Start()
    {
        GetComponent<Animator>().enabled = false;
        transform.position = _camPos.position;
        transform.rotation = _camPos.rotation;
        transform.localScale = _camPos.localScale;
        StartCoroutine(EnableAnimatorRoutine());
    }

    IEnumerator EnableAnimatorRoutine()
    {
        yield return new WaitForSeconds(_waitSeconds);
        GetComponent<Animator>().enabled = true;
    }
}
