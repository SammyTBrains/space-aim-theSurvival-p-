using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField]
    protected GameObject _basicEnemyBeamPrefab, _healthPrefab, _model;
    [SerializeField]
    private Transform _basicBeamSpawnPos;
    [SerializeField]
    protected float _basicFireSpeedCooldown = 0.2f;
    [SerializeField]
    private GameObject _smokeEffect, _sEffect2, _sEffect3, _e12Explosion;
    [SerializeField]
    private Image _healthUI;
    [SerializeField]
    protected int _maxHitValue;

    [SerializeField]
    protected AudioClip _Player_E12_MidBossMiniExplo, _BBBeam;

    protected Animator _animator;
    protected bool _enemyCanFire;
    protected bool _midBossDying;
    protected int _noOfHitsEnemy;

    public bool MidBossDying
    {
        get => _midBossDying;
    }

    public float health { get; set; }
    public float maxHealth { get; set; }

    private bool maxHealthSet;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        _enemyCanFire = true;
        _animator = GetComponent<Animator>();
        StartCoroutine(BasicBeamFireRoutine());
    }

    // Update is called once per frame
     void Update()
    {
        Debug.Log("Enemy Max Health " + maxHealth);
        if (!maxHealthSet)
        {
            maxHealthSet = true;
            health = maxHealth;
        }

        if (GetComponent<MidBoss>() && _midBossDying)
        {
            _enemyCanFire = false;
            return;
        }

        _healthUI.fillAmount = health / maxHealth;
        float percent = (health / maxHealth) * 100;
        if (percent <= 40 && percent > 30)
        {
            _smokeEffect.SetActive(true);
        }
        else if (percent <= 30 && percent > 20)
        {
            if (GetComponent<MidBoss>() || GetComponent<Boss>())
            {
                _sEffect2.SetActive(true);
            }
        }
        else if (percent <= 20)
        {
            if (GetComponent<Boss>())
            {
                _sEffect3.SetActive(true);
            }
        }

        if ((GetComponent<Boss>() || GetComponent<MidBoss>()) && _noOfHitsEnemy >= _maxHitValue)
        {
            Vector3 spawnPos = new Vector3(Random.Range(-20, 24), Random.Range(-11, 13), transform.position.z);
            Instantiate(_healthPrefab, spawnPos, _healthPrefab.transform.rotation);
            _noOfHitsEnemy = 0;
        }

    }

    IEnumerator BasicBeamFireRoutine()
    {
        while (_enemyCanFire)
        {
            GameObject projectile = Instantiate(_basicEnemyBeamPrefab, _basicBeamSpawnPos.position, _basicEnemyBeamPrefab.transform.rotation);
            AudioSource sourceA = SpawnManager.Instance.AudioMethod(_BBBeam, false, .7f, false);
            sourceA.volume /= 4.0f;
            projectile.GetComponent<Projectile>().Spawner = gameObject;
            yield return new WaitForSeconds(_basicFireSpeedCooldown);
        }
    }

    public void Damage(float damage, GameObject damager)
    {
        if(transform.position.x >= 26.0f)
        {
            return;
        }


        health = health - damage;
        if (GetComponent<Boss>() || GetComponent<MidBoss>())
        {
            if (!damager.GetComponent<BasicBeam>())
            {
                _noOfHitsEnemy++;
            }
        }
        if (health <= 0)
        {
            if (!GetComponent<Boss>())
            {
                GameObject health = Instantiate(_healthPrefab, transform.position, _healthPrefab.transform.rotation);
                if (health.transform.position.x >= 24.5f)
                {
                    health.transform.position = new Vector3(23, health.transform.position.y, health.transform.position.z);
                }
                if (health.transform.position.y <= -12)
                {
                    health.transform.position = new Vector3(health.transform.position.x, -11, health.transform.position.z);
                }
                else if (health.transform.position.y >= 14)
                {
                    health.transform.position = new Vector3(health.transform.position.x, 13, health.transform.position.z);
                }
            }

            if (GetComponent<Enemy1>() || GetComponent<Enemy2>())
            {
                GameObject e12Explosion = Instantiate(_e12Explosion, transform.position, Quaternion.identity);
                AudioSource sourceA = SpawnManager.Instance.AudioMethod(_Player_E12_MidBossMiniExplo, false, 2.5f, false);
                sourceA.volume /= 1.5f;
                Destroy(e12Explosion, 2.0f);
                ClearChildren();
            }
            else if (GetComponent<MidBoss>())
            {
                _animator.SetTrigger("Die");
                _midBossDying = true;
                StartCoroutine(MidBossDeadRoutine());
            }

            if (GetComponent<Boss>())
            {
                _enemyCanFire = false;
                TimeManager.Instance.CountTime = false;
                SpawnManager.Instance.PowerUpInstantiate = false;
                SpawnManager.Instance.StopWeaponSpawnW1();
                UIManager.Instance.DamageTaken = FindObjectOfType<Player>().NoOfDamageTaken;
                UIManager.Instance.UpgradesTaken = FindObjectOfType<Player>().NoOfUpgradesCollected;
                StartCoroutine(CancelObjects());
                DeleteAllHealthInScene();
                DeleteAllPowerUpsInScene();
                MovePlayerHolder.Instance.SetMovePlayerActive(gameObject);
            }

            GetComponent<Collider>().enabled = false;
            if (!GetComponent<Boss>())
            {
                Destroy(gameObject, 5.5f);
            }
        }
    }

    IEnumerator CancelObjects()
    {
        yield return new WaitForSeconds(.4f);
        SpawnManager.Instance.GetComponent<SpawnManager>().enabled = false;
        FindObjectOfType<Player>().enabled = false;
        SpecialAbilitiesManager.Instance.GetComponent<SpecialAbilitiesManager>().enabled = false;
    }

    public void ClearChildren()
    {
        Debug.Log(transform.childCount);
        int i = 0;

        //Array to hold all child obj
        GameObject[] allChildren = new GameObject[transform.childCount];

        //Find all child obj and store to that array
        foreach (Transform child in transform)
        {
            allChildren[i] = child.gameObject;
            i += 1;
        }

        //Now destroy them
        foreach (GameObject child in allChildren)
        {
            if (child.tag != "EEffectMidBoss")
            {
                Destroy(child.gameObject);
            }
        }

        Debug.Log(transform.childCount);
    }

    public void DeleteAllHealthInScene()
    {
        Debug.Log("All Healths before " + FindObjectsOfType<Health>().Length);
        int i = 0;

        GameObject[] allObjects = new GameObject[FindObjectsOfType<Health>().Length];

        foreach (Health objs in FindObjectsOfType<Health>())
        {
            allObjects[i] = objs.gameObject;
            i++;
        }

        foreach (GameObject objs in allObjects)
        {
            Destroy(objs);
        }

        Debug.Log("All Healths after " + FindObjectsOfType<Health>().Length);
    }

    public void DeleteAllPowerUpsInScene()
    {
        Debug.Log("All PowerUps before " + FindObjectsOfType<PowerUp>().Length);
        int i = 0;

        GameObject[] allObjects = new GameObject[FindObjectsOfType<PowerUp>().Length];

        foreach (PowerUp objs in FindObjectsOfType<PowerUp>())
        {
            allObjects[i] = objs.gameObject;
            i++;
        }

        foreach (GameObject objs in allObjects)
        {
            Destroy(objs);
        }

        Debug.Log("All PowerUps after " + FindObjectsOfType<PowerUp>().Length);
    }

    IEnumerator MidBossDeadRoutine()
    {
        yield return new WaitForSeconds(4.0f);
        _midBossDying = false;
    }
}
