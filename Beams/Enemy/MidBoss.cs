using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidBoss : Enemy
{
    [SerializeField]
    private Transform _secondPos;
    [SerializeField]
    private float _specialBlastCooldownTime;
    [SerializeField]
    private AudioClip _ablast2, _BPMExplo;

    protected override void Start()
    {
        base.Start();
        StartCoroutine(BasicBeamFireRoutine2());
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

    public void MiniExplosion()
    {
        AudioSource sourceA = SpawnManager.Instance.AudioMethod(_Player_E12_MidBossMiniExplo, false, 2.5f, false);
        sourceA.volume /= 1.5f;
    }

    public void FinalExplo()
    {
        AudioSource sourceA = SpawnManager.Instance.AudioMethod(_BPMExplo, false, 2.5f, false);
    }

    IEnumerator SpecialBlastRoutine()
    {
        while (_enemyCanFire)
        {
            yield return new WaitForSeconds(_specialBlastCooldownTime);
            SpawnManager.Instance.AudioMethod(_ablast2, false, .7f, false);
            _animator.SetTrigger("SpecialBlastMidBoss");
        }
    }
}
