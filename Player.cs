using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    [SerializeField]
    private float _speed, _tiltValue = 10.0f, _WLDTime;
    [SerializeField]
    private GameObject _helperOne, _helperTwo, _smokeEffect, _WLD;
    [SerializeField]
    private Transform _holderPosOne, _holderPosTwo, _helperTwoHolder;
    [SerializeField]
    private Animator _helperAnimator;
    [SerializeField]
    private GameObject _explosion, _gameOverTimeline;

    public GameObject _shield;
    public bool _isSheilded;

    private Animator _animator;
    private int _pWLevel;
    private bool _calledRecoil;

    [SerializeField]
    private float _maxHealth;
    [SerializeField]
    private int _NoOfEnemyHits, _noOfDamageTaken, _noOfUpgradesCollected;
    public int NoOfEnemyHits
    {
        set => _NoOfEnemyHits = value;
    }
    public int NoOfDamageTaken
    {
        get => _noOfDamageTaken;
    }
    public int NoOfUpgradesCollected
    {
        get => _noOfUpgradesCollected;
        set => _noOfUpgradesCollected = value;
    }

    [SerializeField]
    private AudioClip _Player_E12_MidBossMiniExplo, _MExplo4;

    private MidBoss _midBoss;

    public float maxHealth { get; set; }
    public float health { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        maxHealth = _maxHealth;
        UIManager.Instance.PlayerHealthMaxValue = _maxHealth;
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        _midBoss = FindObjectOfType<MidBoss>();
        if (_midBoss != null && _midBoss.gameObject.activeInHierarchy && _midBoss.MidBossDying)
        {
            return;
        }

        if (health > maxHealth)
        {
            health = maxHealth;
        }
        UIManager.Instance.PlayerHealthUI = health;
        float percent = (health / maxHealth) * 100;
        if (percent <= 40)
        {
            _smokeEffect.SetActive(true);
        }
        else
        {
            _smokeEffect.SetActive(false);
        }

        _pWLevel = SpawnManager.Instance.PlayerWeaponLevel;

        if (_pWLevel == 3)
        {
            _helperOne.SetActive(true);
            _helperOne.transform.position = _holderPosTwo.position;
            _calledRecoil = false;
        }
        else if (_pWLevel == 4)
        {
            _helperOne.SetActive(true);
            _helperTwo.SetActive(true);
            _helperOne.transform.position = _holderPosOne.position;
            if (!_calledRecoil)
            {
                _calledRecoil = true;
                StartCoroutine(HelperRecoilRoutine());
            }
            if (transform.position.y <= -10)
            {
                _helperTwo.transform.position = _helperTwoHolder.position;
                _helperOne.transform.position = _holderPosTwo.position;
            }
            else
            {
                _helperOne.transform.position = _holderPosOne.position;
                _helperTwo.transform.position = _holderPosTwo.position;
            }
        }
        else
        {
            _calledRecoil = false;
            _helperOne.SetActive(false);
            _helperTwo.SetActive(false);
        }
    }

    IEnumerator HelperRecoilRoutine()
    {
        while (_pWLevel == 4)
        {
            yield return new WaitForSeconds(0.5f);
            _helperAnimator.SetInteger("Recoil", 1);
            AudioSource sourceA = SpawnManager.Instance.AudioMethod(_MExplo4, false, .5f, false);
            sourceA.volume /= 2.0f;
            sourceA.pitch = .5f;
            yield return new WaitForSeconds(0.7f);
            _helperAnimator.SetInteger("Recoil", 0);
        }
    }

    void Movement()
    {
        if (SpecialAbilitiesManager.Instance.XPressed)
        {
            return;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        transform.Translate(new Vector3(horizontal, vertical) * _speed * Time.deltaTime);
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -24, 24), Mathf.Clamp(transform.position.y, -11, 11), transform.position.z);

        if (horizontal > 0)
        {
            _animator.SetBool("Backward", false);
            _animator.SetBool("Forward", true);
        }
        else if (horizontal < 0)
        {
            _animator.SetBool("Forward", false);
            _animator.SetBool("Backward", true);
        }
        else
        {
            _animator.SetBool("Forward", false);
            _animator.SetBool("Backward", false);
        }

        float tiltAroundZ = vertical * _tiltValue;

        Quaternion target = Quaternion.Euler(0, 0, tiltAroundZ);

        transform.rotation = Quaternion.Slerp(transform.rotation, target, _speed * Time.deltaTime);
    }

    public void TriggerCameraShake()
    {
        SpecialAbilitiesManager.Instance.CameraShakeTrigger();
    }

    public void BlastAudio()
    {
        SpecialAbilitiesManager.Instance.BlastAudio();
    }

    public void Damage(float damage, GameObject damager)
    {
        if (_isSheilded)
        {
            return;
        }

        Debug.Log("Damaging Player");
        health = health - damage;
        _noOfDamageTaken++;
        _NoOfEnemyHits++;
        if (health <= 0)
        {
            _gameOverTimeline.SetActive(true);
            Instantiate(_explosion, transform.position, Quaternion.identity);
            AudioSource sourceA = SpawnManager.Instance.AudioMethod(_Player_E12_MidBossMiniExplo, false, 2.5f, false);
            sourceA.volume /= 1.5f;
            Destroy(gameObject);
        }

        if (_NoOfEnemyHits >= 20 && _pWLevel > 1)
        {
            Debug.Log("Hit by enemy 20 times since last weapon upgrade");
            _WLD.SetActive(true);
            StartCoroutine(WLDRoutine());
            SpawnManager.Instance.PlayerWeaponLevel--;
            SpawnManager.Instance.ResetPlayerWeaponLevel();
            _NoOfEnemyHits = 0;
        }
    }

    IEnumerator WLDRoutine()
    {
        yield return new WaitForSeconds(_WLDTime);
        _WLD.SetActive(false);
    }

    float _shieldPowerUpTime;
    GameObject _sheildPowerUpObj;

    public void CallShieldRoutine(float shieldTime, GameObject shield)
    {
        StopCoroutine("ShieldRoutine");
        _shieldPowerUpTime = shieldTime;
        _sheildPowerUpObj = shield;
        StartCoroutine("ShieldRoutine");
    }

    private IEnumerator ShieldRoutine()
    {
        yield return new WaitForSeconds(_shieldPowerUpTime);
        _shield.SetActive(false);
        _isSheilded = false;
        Destroy(_sheildPowerUpObj);
    }
}
