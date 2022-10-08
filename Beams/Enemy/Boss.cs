using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    [SerializeField]
    private Transform _secondPos, _thirdPos;
    [SerializeField]
    private float _specialBlastCooldownTime;
    [SerializeField]
    private AudioClip _ablast2;

    protected override void Start()
    {
        base.Start();
        StartCoroutine(BasicBeamFireRoutine2());
        StartCoroutine(BasicBeamFireRoutine3());
        StartCoroutine(SpecialBlastRoutine());
    }

    IEnumerator BasicBeamFireRoutine2()
    {
        while (_enemyCanFire)
        {
            GameObject projectile = Instantiate(_basicEnemyBeamPrefab, _secondPos.position, _basicEnemyBeamPrefab.transform.rotation);
            projectile.GetComponent<Projectile>().Spawner = gameObject;
            yield return new WaitForSeconds(_basicFireSpeedCooldown);
        }
    }

    IEnumerator BasicBeamFireRoutine3()
    {
        while (_enemyCanFire)
        {
            GameObject projectile = Instantiate(_basicEnemyBeamPrefab, _thirdPos.position, _basicEnemyBeamPrefab.transform.rotation);
            projectile.GetComponent<Projectile>().Spawner = gameObject;
            yield return new WaitForSeconds(_basicFireSpeedCooldown);
        }
    }

    public void SBlastA()
    {
        SpawnManager.Instance.AudioMethod(_ablast2, false, .7f, false);
    }

    IEnumerator SpecialBlastRoutine()
    {
        while (_enemyCanFire)
        {
            yield return new WaitForSeconds(_specialBlastCooldownTime);    
            _animator.SetTrigger("SpecialBlastBoss");
        }
    }
}
