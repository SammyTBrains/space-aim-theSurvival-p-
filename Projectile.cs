using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private float _damage, _speed;
    [SerializeField]
    private Vector2 _direction;
    [SerializeField]
    private GameObject _hitParticle, _explosion, _model;

    private GameObject _spawner;
    public GameObject Spawner
    {
        set => _spawner = value;
    }

    public float Damage
    {
        set => _damage = value;
    }

    [SerializeField]
    private AudioClip _MExplo, _hitS;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(_direction * _speed * Time.deltaTime);
        if (transform.position.x >= 28 || transform.position.y <= -15)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hit Something");
        if (other.TryGetComponent(out IDamageable ship))
        {
            Debug.Log("Spawner is " + _spawner + " hit Idamageable");
            if (other.gameObject != _spawner)
            {
                Debug.Log("Spawner is " + _spawner + " hit other");

                if(_spawner.GetComponent<Enemy>() && other.GetComponent<Enemy>())
                {
                    return;
                }

                if (!GetComponent<Missile>())
                {
                    Debug.Log("Hit from projectile script");
                    GameObject hit = Instantiate(_hitParticle, transform.position, Quaternion.identity);
                    hit.transform.SetParent(other.transform);
                    AudioSource sourceA = SpawnManager.Instance.AudioMethod(_hitS, false, .8f, false);
                    sourceA.volume /= 11.5f;
                    sourceA.pitch = .6f;
                    Destroy(hit, .3f);
                    GetComponent<SpriteRenderer>().enabled = false;
                }
                else
                {
                    GameObject explosion = Instantiate(_explosion, other.ClosestPoint(transform.position), Quaternion.identity);
                    explosion.transform.SetParent(other.transform);
                    AudioSource sourceA = SpawnManager.Instance.AudioMethod(_MExplo, false, .7f, false);
                    sourceA.volume /= 6.0f;
                    Destroy(explosion, .9f);
                    Destroy(_model);
                }
                if (ship != null)
                {
                    ship.Damage(_damage, gameObject);
                }
                if (!_spawner.GetComponent<Enemy>())
                {
                    SpecialAbilitiesManager.Instance.HitEnemiesCounter();
                }
                Destroy(gameObject, 2.5f);
            }
        }
    }
}
