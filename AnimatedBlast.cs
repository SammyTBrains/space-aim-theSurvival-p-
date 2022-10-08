using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedBlast : MonoBehaviour
{
    [SerializeField]
    private float _damage;
    [SerializeField]
    private GameObject _hitParticle;

    [SerializeField]
    private AudioClip _hitS;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamageable ship))
        {
            if (other.gameObject != transform.parent.gameObject)
            {
                ship.Damage(_damage, gameObject);
                AudioSource sourceA = SpawnManager.Instance.AudioMethod(_hitS, false, .8f, false);
                sourceA.volume /= 11.5f;
                sourceA.pitch = .6f;
                SpecialAbilitiesManager.Instance.HitEnemiesCounter();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out IDamageable ship))
        {
            if (other.gameObject != transform.parent.gameObject)
            {
                GameObject hit = Instantiate(_hitParticle, other.ClosestPoint(transform.position), Quaternion.identity);
                hit.transform.SetParent(other.transform);
                Destroy(hit, .3f);
            }
        }
    }
}
