using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManagerMM : MonoBehaviour
{
    private static UIManagerMM _instance;
    public static UIManagerMM Instance
    {
        get => _instance;
    }

    [SerializeField]
    private GameObject _optionsMenu, _loadingPopUp, _TheAudioSourcePrefab;
    [SerializeField]
    private AudioClip _bgClipMM, _button;
    [SerializeField]
    private TMP_Dropdown _difficulty, _sound, _brightness;

    private AudioSource _bgSource;
    private bool sourceSet;


    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        AudioMethod(_bgClipMM, true, 0, true);

        CheckAndDisplayOptionsValues("Difficulty");
        CheckAndDisplayOptionsValues("Sound");
        CheckAndDisplayOptionsValues("Brightness");
    }

    void AudioMethod(AudioClip clip, bool isBG, float destroyAfter, bool loop)
    {
        AudioSource source = Instantiate(_TheAudioSourcePrefab, Camera.main.transform.position, Quaternion.identity).GetComponent<AudioSource>();
        source.clip = clip;
        source.Play();
        if (loop)
        {
            source.loop = true;
        }
        if (!sourceSet)
        {
            sourceSet = true;
            _bgSource = source;
        }
        if (!isBG)
        {
            Destroy(source.gameObject, destroyAfter);
        }
    }

    private void Update()
    {
        _bgSource.volume = _TheAudioSourcePrefab.GetComponent<AudioSource>().volume;
    }

    void CheckAndDisplayOptionsValues(string property)
    {
        if (PlayerPrefs.HasKey(property))
        {
            int val = PlayerPrefs.GetInt(property);
            CheckPropertyStringAndDisplay(property, val);
        }
        else
        {
            switch (property)
            {
                case "Difficulty":
                    CheckPropertyStringAndDisplay(property, 0);
                    StoreLevelInPlayerpref(0, property);
                    break;
                case "Sound":
                    CheckPropertyStringAndDisplay(property, 1);
                    StoreLevelInPlayerpref(1, property);
                    break;
                case "Brightness":
                    CheckPropertyStringAndDisplay(property, 2);
                    StoreLevelInPlayerpref(2, property);
                    break;
            }
        }
    }

    void CheckPropertyStringAndDisplay(string property, int value)
    {
        switch (property)
        {
            case "Difficulty":
                _difficulty.value = value;
                break;
            case "Sound":
                _sound.value = value;
                break;
            case "Brightness":
                _brightness.value = value;
                break;
        }
    }

    public void StartGame()
    {
        AudioMethod(_button, false, .6f, false);
        _loadingPopUp.SetActive(true);
        StartCoroutine(LoadYourAsyncScene());
    }

    IEnumerator LoadYourAsyncScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            Debug.Log("SceneLoading");
            yield return null;
        }

        Debug.Log("Scene Loaded");
    }

    public void OpenOptions()
    {
        AudioMethod(_button, false, .6f, false);
        _optionsMenu.SetActive(true);
    }

    public void CloseOptions()
    {
        AudioMethod(_button, false, .6f, false);
        _optionsMenu.SetActive(false);
    }

    public void QuitApplication()
    {
        AudioMethod(_button, false, .6f, false);
        Application.Quit();
    }

    public void Difficulty(int level)
    {
        AudioMethod(_button, false, .6f, false);
        Debug.Log("Difficulty " + level);
        StoreLevelInPlayerpref(level, "Difficulty");
    }

    public void Sound(int level)
    {
        AudioMethod(_button, false, .6f, false);
        Debug.Log("Sound " + level);
        StoreLevelInPlayerpref(level, "Sound");
    }

    public void Brightness(int level)
    {
        AudioMethod(_button, false, .6f, false);
        Debug.Log("Brightness " + level);
        StoreLevelInPlayerpref(level, "Brightness");
    }

    void StoreLevelInPlayerpref(int level, string property)
    {
        switch (level)
        {
            case 0:
                PlayerPrefs.SetInt(property, 0);
                break;
            case 1:
                PlayerPrefs.SetInt(property, 1);
                break;
            case 2:
                PlayerPrefs.SetInt(property, 2);
                break;
        }
    }
}
