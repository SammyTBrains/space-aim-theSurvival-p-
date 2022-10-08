using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsManager : MonoBehaviour
{
    #region Difficulty Fields
    [Header("Difficulty")]
    [SerializeField]
    private GameObject _enemBasicBeamPrefab;
    [SerializeField]
    private GameObject _bossPrefab, _enemy1Prefab, _enemy2Prefab, _midBossPrefab;
    #endregion

    #region Sound Fields
    [Header("Sound")]
    [SerializeField]
    private AudioSource _theAudioSourcePrefab;
    #endregion

    // Update is called once per frame
    void Update()
    {
        Difficulty();
        Sound();
        Quality();
    }

    #region Difficulty
    void Difficulty()
    {
        int diff = PlayerPrefs.GetInt("Difficulty");

        switch (diff)
        {
            case 0:
                SetDifficultyValues(6, 4000, 500, 650, 2000);
                break;
            case 1:
                SetDifficultyValues(9, 5000, 600, 750, 3000);
                break;
            case 2:
                SetDifficultyValues(12, 6000, 800, 950, 4000);
                break;
        }
    }

    void SetDifficultyValues(int beamDamage, int bossMHealth, int enemy1MHealth, int enemy2MHealth, int midBossMHealth)
    {
        _enemBasicBeamPrefab.GetComponent<Projectile>().Damage = beamDamage;
        if (SpawnManager.Instance != null)
        {
            SpawnManager.Instance.AssignValue(bossMHealth, _bossPrefab);
            SpawnManager.Instance.AssignValue(enemy1MHealth, _enemy1Prefab);
            SpawnManager.Instance.AssignValue(enemy2MHealth, _enemy2Prefab);
            SpawnManager.Instance.AssignValue(midBossMHealth, _midBossPrefab);
        }
    }
    #endregion

    #region Sound
    void Sound()
    {
        int sound = PlayerPrefs.GetInt("Sound");

        switch (sound)
        {
            case 0:
                _theAudioSourcePrefab.volume = .3f;
                Debug.Log("Volume set 1");
                break;
            case 1:
                _theAudioSourcePrefab.volume = .67f;
                Debug.Log("Volume set 2");
                break;
            case 2:
                _theAudioSourcePrefab.volume = 1.0f;
                Debug.Log("Volume set 3");
                break;
        }

    }
    #endregion

    #region Quality
    void Quality()
    {
        int qual = PlayerPrefs.GetInt("Brightness");

        switch (qual)
        {
            case 0:
                QualitySettings.SetQualityLevel(0);
                break;
            case 1:
                QualitySettings.SetQualityLevel(1);
                break;
            case 2:
                QualitySettings.SetQualityLevel(2);
                break;
        }
    }
    #endregion
}
