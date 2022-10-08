using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField]
    private float _speed;
    [SerializeField]
    private Vector3 _direction;
    [SerializeField]
    private GameObject _collectPowerUpVisualFeedback;
    [SerializeField]
    protected GameObject _model;

    [SerializeField]
    private AudioClip _ColPUp;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(_direction * _speed * Time.deltaTime);
        if (transform.position.x >= 28)
        {
            Destroy(gameObject);
        }
    }

    protected void PowerUpCollectedFeedback(Collider other)
    {
        GameObject visualFeedback = Instantiate(_collectPowerUpVisualFeedback, other.ClosestPoint(transform.position), Quaternion.identity);
        AudioSource sourceA = SpawnManager.Instance.AudioMethod(_ColPUp, false, .7f, false);
        sourceA.volume /= 1.2f;
        Destroy(visualFeedback, 1.0f);
    }
}
