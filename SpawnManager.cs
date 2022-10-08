using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private static SpawnManager _instance;
    public static SpawnManager Instance
    {
        get => _instance;
    }

    #region Player Fields
    [Header("Player")]
    [SerializeField]
    private Transform _player;
    [SerializeField]
    private Transform _beamSpawnPosition, _playerMissileSpawnPos, _helperOneSpawnPos, _helperTwoSpawnPos, _W4MissileSpawnPos, _helperMissileSpawnPos;
    [SerializeField]
    private GameObject _basicPlayerBeamPrefab, _missilePrefab, _beamW3Prefab;
    [SerializeField]
    private int _playerWeaponLevel, _missileDist;

    public int PlayerWeaponLevel
    {
        get => _playerWeaponLevel;
        set => _playerWeaponLevel = value;
    }
    [SerializeField]
    private float _playerBeamFireCooldown, _missileSpawnCooldown;

    private GameObject _basicPlayerBeam, _missile, _beamW3;
    private bool _w1, _w2, _w3, _w4;
    #endregion

    #region PowerUp Fields
    [Header("Power Ups")]
    [SerializeField]
    private PowerUp[] _powerUps;
    [SerializeField]
    private float _minSpawnTime, _maxSpawnTime;

    private bool _powerUpInstantiate;
    public bool PowerUpInstantiate
    {
        set => _powerUpInstantiate = value;
    }
    #endregion

    #region Wave Fields
    [Header("Waves")]
    [SerializeField]
    private GameObject[] _wavePrefabs;
    [SerializeField]
    private int _waveNumber;

    private GameObject _wave;
    #endregion

    [Header("The Audio")]
    [SerializeField]
    private GameObject _TheAudioSourcePrefab;
    [SerializeField]
    private AudioClip _bgSound, _BBBeam, _SBBeam, _SMi;

    private void Awake()
    {
        _instance = this;
        _powerUpInstantiate = true;
        _wave = Instantiate(_wavePrefabs[_waveNumber]);
        WaveChildren();
    }

    // Start is called before the first frame update
    void Start()
    {
        AudioMethod(_bgSound, true, 0, true);
        _playerWeaponLevel = 1;
        StartCoroutine(PowerUpRoutine());
    }

    public AudioSource AudioMethod(AudioClip clip, bool isBG, float destroyAfter, bool loop)
    {
        AudioSource source = Instantiate(_TheAudioSourcePrefab, Camera.main.transform.position, Quaternion.identity).GetComponent<AudioSource>();
        source.clip = clip;
        source.Play();
        if (loop)
        {
            source.loop = true;
        }
        if (!isBG)
        {
            Destroy(source.gameObject, destroyAfter);
        }

        return source;
    }

    public void ResetPlayerWeaponLevel()
    {
        _w1 = _w2 = _w3 = _w4 = false;
    }

    public void StopWeaponSpawnW1()
    {
        _w1 = _w2 = _w3 = _w4 = true;
        if (_playerWeaponLevel == 1)
        {
            _playerWeaponLevel = 2;
        }
        else
        {
            _playerWeaponLevel = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_wave.GetComponentInChildren<MidBoss>() && _wave.GetComponentInChildren<MidBoss>().MidBossDying)
        {
            _wave.GetComponent<Animator>().enabled = false;
            Debug.Log("Mid Boss Animator Disabled");
        }

        if (_wave.transform.childCount == 0)
        {
            Destroy(_wave);
            _waveNumber++;
            _wave = Instantiate(_wavePrefabs[_waveNumber]);
            WaveChildren();
        }

        #region Player
        if (_playerWeaponLevel == 1 && !_w1)
        {
            _w1 = true;
            StartCoroutine(SpawnBasicPlayerBeam());
        }
        else if (_playerWeaponLevel == 2 && !_w2)
        {
            _w2 = true;
            StartCoroutine(SpawnBasicPlayerBeamW2());
            StartCoroutine(SpawnPlayerMissile());
        }
        else if (_playerWeaponLevel == 3 && !_w3)
        {
            _w3 = true;
            StartCoroutine(BeamWeaponThreeRoutine());
            StartCoroutine(SpawnBasicHelperBeam());
            StartCoroutine(SpawnHelperMissile());
        }
        else if (_playerWeaponLevel == 4 && !_w4)
        {
            _w4 = true;
            StartCoroutine(SpawnHelperMissileW4());
            StartCoroutine(SpawnMissileW4());
        }
        #endregion

        UIManager.Instance.WeaponLevelValue = _playerWeaponLevel;
    }

    GameObject[] allChildren; 

    public void WaveChildren()
    {
        int i = 0;

        //Array to hold all child obj
      allChildren = new GameObject[_wave.transform.childCount];

        //Find all child obj and store to that array
        foreach (Transform child in _wave.transform)
        {
            allChildren[i] = child.gameObject;
            i += 1;
        }
    }

    public void AssignValue(float value, GameObject gameObjectChWave)
    {
        foreach (GameObject child in allChildren)
        {
            if (gameObjectChWave.GetComponent<Enemy1>() && child.GetComponent<Enemy1>())
            {
                child.GetComponent<Enemy>().maxHealth = value;
            }
            else if (gameObjectChWave.GetComponent<Enemy2>() && child.GetComponent<Enemy2>())
            {
                child.GetComponent<Enemy>().maxHealth = value;
            }
            else if (gameObjectChWave.GetComponent<Boss>() && child.GetComponent<Boss>())
            {
                child.GetComponent<Enemy>().maxHealth = value;
            }
            else if (gameObjectChWave.GetComponent<MidBoss>() && child.GetComponent<MidBoss>())
            {
                child.GetComponent<Enemy>().maxHealth = value;
            }
        }
    }

    #region Player
    IEnumerator SpawnBasicPlayerBeam()
    {
        while (_playerWeaponLevel == 1)
        {
            InstantiateBasicBeam(_beamSpawnPosition.position);
            yield return new WaitForSeconds(_playerBeamFireCooldown);
        }
    }

    IEnumerator SpawnBasicPlayerBeamW2()
    {
        while (_playerWeaponLevel == 2)
        {
            InstantiateBasicBeam(_beamSpawnPosition.position);
            yield return new WaitForSeconds(_playerBeamFireCooldown);
        }
    }

    IEnumerator SpawnBasicHelperBeam()
    {
        while (_playerWeaponLevel == 3)
        {
            InstantiateBasicBeam(_helperOneSpawnPos.position);
            yield return new WaitForSeconds(_playerBeamFireCooldown);
        }
    }

    IEnumerator SpawnPlayerMissile()
    {
        while (_playerWeaponLevel == 2)
        {
            InstantiateMissile(_playerMissileSpawnPos.position);
            yield return new WaitForSeconds(_missileSpawnCooldown);
        }
    }

    IEnumerator SpawnHelperMissile()
    {
        while (_playerWeaponLevel == 3)
        {
            InstantiateMissile(_helperMissileSpawnPos.position);
            yield return new WaitForSeconds(_missileSpawnCooldown);
        }
    }
    IEnumerator SpawnHelperMissileW4()
    {
        while (_playerWeaponLevel == 4)
        {
            InstantiateMissile(_helperMissileSpawnPos.position);
            yield return new WaitForSeconds(_missileSpawnCooldown);
        }
    }

    IEnumerator BeamWeaponThreeRoutine()
    {
        while (_playerWeaponLevel == 3)
        {
            _beamW3 = Instantiate(_beamW3Prefab, _beamSpawnPosition.position, Quaternion.identity);
            AudioSource sourceA = AudioMethod(_SBBeam, false, .7f, false);
            sourceA.volume /= 4.0f;
            _beamW3.GetComponent<Projectile>().Spawner = _player.gameObject;
            yield return new WaitForSeconds(_playerBeamFireCooldown);
        }
    }

    IEnumerator SpawnMissileW4()
    {
        while (_playerWeaponLevel == 4)
        {
            GameObject projectile = Instantiate(_missilePrefab, _W4MissileSpawnPos.position, Quaternion.identity);
            AudioSource sourceA = AudioMethod(_SMi, false, .6f, false);
            sourceA.volume /= 4.0f;
            sourceA.pitch = .65f;
            projectile.GetComponent<Projectile>().Spawner = _player.gameObject;
            yield return new WaitForSeconds(_missileSpawnCooldown);
        }
    }

    void InstantiateBasicBeam(Vector3 spawnPos)
    {
        _basicPlayerBeam = Instantiate(_basicPlayerBeamPrefab, spawnPos, Quaternion.identity);
        AudioSource sourceA = AudioMethod(_BBBeam, false, .7f, false);
        sourceA.volume /= 4.0f;
        _basicPlayerBeam.GetComponent<Projectile>().Spawner = _player.gameObject;
    }

    void InstantiateMissile(Vector3 spawnPos)
    {
        _missile = Instantiate(_missilePrefab, spawnPos, Quaternion.identity);
        AudioSource sourceA = AudioMethod(_SMi, false, .6f, false);
        sourceA.volume /= 4.0f;
        sourceA.pitch = .65f;
        _missile.GetComponent<Projectile>().Spawner = _player.gameObject;
        Vector3 rot = _missile.transform.localEulerAngles;
        rot.z = -15;
        _missile.transform.localEulerAngles = rot;
    }
    #endregion

    IEnumerator PowerUpRoutine()
    {
        while (_powerUpInstantiate)
        {
            yield return new WaitForSeconds(Random.Range(_minSpawnTime, _maxSpawnTime));
            int arr = Random.Range(0, 2);
            Vector3 spawnPos = new Vector3(23, Random.Range(-10, 13), -5.0f);
            Instantiate(_powerUps[arr].gameObject, spawnPos, Quaternion.identity);
        }
    }
}
