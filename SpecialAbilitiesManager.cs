using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpecialAbilitiesManager : MonoBehaviour
{
    private static SpecialAbilitiesManager _instance;
    public static SpecialAbilitiesManager Instance
    {
        get => _instance;
    }

    [SerializeField]
    private Animator _playerAnim, _cameraAnim;
    [SerializeField]
    private GameObject _bodySlamShield, _pUpBFillIns, _pSpinFillIns, _sRecoilFillIns, _bSlamFillIns;
    [SerializeField]
    private Transform _player, _playerModel;
    [SerializeField]
    private float _bodySlamAcc, _bodySlamDist;
    [SerializeField]
    private TrailRenderer _playerTrail;
    [SerializeField]
    private float _slamSpeed = 10, _playerSpinSpeed = 800.0f;
    [SerializeField]
    private Image _pUpBFill, _pSpinFill, _sRecoilFill, _bSlamFill;
    [SerializeField]
    private float _enemyHitValue, _enemiesHitMaxValue, _timeElapsedMaxVlue;

    private Vector3 _slamPos;
    private bool _spinPlayer = false;
    private bool _xPressed = false;
    public bool XPressed
    {
        get => _xPressed;
    }

    [SerializeField]
    private float _enemiesHitValue, _timeElapsedValue;

    [SerializeField]
    private AudioClip _ablast1, _ablast2, _pUp3, _pUp2, _bSlamP;

    private bool _hitEnoughEnemies;
    private bool _timeElapsed;

    private void Awake()
    {
        _instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TimeAbilitiesCounter());
    }

    // Update is called once per frame
    void Update()
    {
        HitEnoughEnemiesManager();
        TimeElapsedManager();
        AbilitiesActivationManager();
    }

    private void AbilitiesActivationManager()
    {
        if (Input.GetKeyDown(KeyCode.Z) && _hitEnoughEnemies)
        {
            _enemiesHitValue = 0;
            _hitEnoughEnemies = false;
            _playerAnim.SetBool("PowerUpBlast", true);
            StartCoroutine(PowerUpBlast());
        }
        else if (Input.GetKeyDown(KeyCode.X) && _timeElapsed)
        {
            _timeElapsedValue = 0;
            _timeElapsed = false;
            StartCoroutine(TimeAbilitiesCounter());
            _bodySlamShield.SetActive(true);
            _playerTrail.emitting = true;
            SpawnManager.Instance.AudioMethod(_bSlamP, false, .5f, false);
            if (_player.transform.position.x != 24)
            {
                _cameraAnim.SetTrigger("CameraShake");
            }
            _slamPos = _player.transform.position;
            _slamPos.x += _bodySlamDist;
            _xPressed = true;
        }
        else if (Input.GetKeyDown(KeyCode.C) && _timeElapsed)
        {
            _timeElapsedValue = 0;
            _timeElapsed = false;
            StartCoroutine(TimeAbilitiesCounter());
            _playerAnim.SetBool("SingleRecoilBlast", true);
            CameraShakeTrigger();
            StartCoroutine(SingleRecoilBlastRoutine());
        }
        else if (Input.GetKeyDown(KeyCode.V) && _hitEnoughEnemies)
        {
            _enemiesHitValue = 0;
            _hitEnoughEnemies = false;
            _spinPlayer = true;
            _playerAnim.enabled = false;
            StartCoroutine(PlayerSpinRoutine());
        }

        if (_spinPlayer)
        {
            Debug.Log("Spinning Player");
            _playerModel.Rotate(Vector3.right * _playerSpinSpeed * Time.deltaTime);
        }

        if (_xPressed)
        {
            _player.transform.position = Vector3.Lerp(_player.transform.position, _slamPos, _slamSpeed * Time.deltaTime);
            _player.transform.position = new Vector3(Mathf.Clamp(_player.transform.position.x, -24, 24), _player.transform.position.y, _player.transform.position.z);
        }

        Debug.Log("Player pos " + _player.transform.position);
        Debug.Log("Slam pos " + _slamPos);
        if (_player.transform.position == _slamPos || _player.transform.position.x == 24 || Vector3.Distance(_player.transform.position,
            _slamPos) <= .1)
        {
            Debug.Log("At slam pos");
            _bodySlamShield.SetActive(false);
            _playerTrail.emitting = false;
            _xPressed = false;
        }
    }

    IEnumerator PlayerSpinRoutine()
    {
        AudioSource sourceA = SpawnManager.Instance.AudioMethod(_pUp2, false, .67f, false);
        sourceA.volume *= 1.25f;
        yield return new WaitForSeconds(1.0f);
        Debug.Log("Stop Spin");
        _spinPlayer = false;
        _playerAnim.enabled = true;
        _playerAnim.SetTrigger("PlayerSpinBlast");
        _cameraAnim.SetTrigger("CameraShake");
        SpawnManager.Instance.AudioMethod(_ablast2, false, .7f, false);
        _playerModel.localEulerAngles = Vector3.zero;
    }

    public void BlastAudio()
    {
        SpawnManager.Instance.AudioMethod(_ablast2, false, .7f, false);
    }

    IEnumerator PowerUpBlast()
    {
        SpawnManager.Instance.AudioMethod(_pUp3, false, .57f, false);
        yield return new WaitForSeconds(1.5f);
        _playerAnim.SetBool("PowerUpBlast", false);
    }

    IEnumerator SingleRecoilBlastRoutine()
    {
        AudioSource sourceA = SpawnManager.Instance.AudioMethod(_ablast1, false, 1.0f, false);
        sourceA.volume *= 1.4f;
        yield return new WaitForSeconds(0.7f);
        _playerAnim.SetBool("SingleRecoilBlast", false);
    }

    public void CameraShakeTrigger()
    {
        _cameraAnim.SetTrigger("CameraShake");
    }

    private void HitEnoughEnemiesManager()
    {
        AbilityActivationManager(_pUpBFill, _pSpinFill, _enemiesHitValue, _enemiesHitMaxValue, _pUpBFillIns, _pSpinFillIns);
    }

    private void TimeElapsedManager()
    {
        AbilityActivationManager(_sRecoilFill, _bSlamFill, _timeElapsedValue, _timeElapsedMaxVlue, _sRecoilFillIns, _bSlamFillIns);
    }

    private void AbilityActivationManager(Image firstFill, Image secondFill, float activationValue, float activationMaxValue,
        GameObject firstFillIns, GameObject secondFillIns)
    {
        firstFill.fillAmount = activationValue / activationMaxValue;
        secondFill.fillAmount = activationValue / activationMaxValue;

        if (activationValue >= activationMaxValue)
        {
            firstFillIns.SetActive(true);
            secondFillIns.SetActive(true);
            if (firstFillIns == _pUpBFillIns)
            {
                _hitEnoughEnemies = true;
            }
            else
            {
                _timeElapsed = true;
            }
        }
        else
        {
            firstFillIns.SetActive(false);
            secondFillIns.SetActive(false);
        }
    }

    IEnumerator TimeAbilitiesCounter()
    {
        while (!_timeElapsed)
        {
            yield return new WaitForSeconds(1.0f);
            _timeElapsedValue++;
        }
    }

    public void HitEnemiesCounter()
    {
        _enemiesHitValue += _enemyHitValue;
    }
}
