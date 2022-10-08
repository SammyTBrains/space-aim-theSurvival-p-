using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance
    {
        get => _instance;
    }

    private void Awake()
    {
        _instance = this;
    }

    [SerializeField]
    private GameObject _player;

    [SerializeField]
    private GameObject _pauseMenu, _loadingPopUp, _pauseButton;
    [SerializeField]
    private AudioClip _button;

    #region Logic UI
    [SerializeField]
    private Slider _playerHealth;
    [SerializeField]
    private TextMeshProUGUI _weaponLevelValue, _timeMinuteValue, _timeSecondsValue;
    [SerializeField]
    private TextMeshProUGUI _endTimeMinuteValue, _endTimeSecondsValue;
    [SerializeField]
    private TextMeshProUGUI _bestTimeMinuteValue, _bestTimeSecondsValue;
    [SerializeField]
    private TextMeshProUGUI _noOfDamageTakenValue, _noOfUpgradesCollectedValue, _noOfCoinsGainedValue;

    private int _time, _currentBestTime;

    private int _coins, _damageTaken, _upgrades;

    public int DamageTaken
    {
        set => _damageTaken = value;
    }

    public int UpgradesTaken
    {
        set => _upgrades = value;
    }

    public float PlayerHealthMaxValue
    {
        set => _playerHealth.maxValue = value;
    }

    public float PlayerHealthUI
    {
        set => _playerHealth.value = value;
    }

    public int WeaponLevelValue
    {
        set => _weaponLevelValue.text = value.ToString();
    }

    private bool _displayedCoins;

    public int TimeValue
    {
        set
        {
            _time = value;
            string minutesString;
            string secondsString;
            GetMinutesAndSeconds(out minutesString, out secondsString);

            _timeMinuteValue.text = minutesString;
            _timeSecondsValue.text = secondsString;

            //End Time
            _endTimeMinuteValue.text = minutesString;
            _endTimeSecondsValue.text = secondsString;
        }
    }

    private void Update()
    {
        if (_player != null)
        {
            _noOfDamageTakenValue.text = _player.GetComponent<Player>().NoOfDamageTaken.ToString("#,#");
            _noOfUpgradesCollectedValue.text = _player.GetComponent<Player>().NoOfUpgradesCollected.ToString("#,#");
        }
    }

    private void StoreBestTimeAndGetNew(string min, string sec)
    {
        PlayerPrefs.SetString("bestTimeMinutes", min);
        PlayerPrefs.SetString("bestTimeSeconds", sec);
        Debug.Log("Stored new values " + min + ":" + sec);
    }

    public void SetBestTime()
    {
        string min;
        string sec;
        if (PlayerPrefs.HasKey("bestTimeMinutes"))
        {
            min = PlayerPrefs.GetString("bestTimeMinutes");
            sec = PlayerPrefs.GetString("bestTimeSeconds");
        }
        else
        {
            min = _endTimeMinuteValue.text;
            sec = _endTimeSecondsValue.text;
        }

        _currentBestTime = (60 * int.Parse(min)) + int.Parse(sec);
        Debug.Log("Current Best Time " + _currentBestTime);
        Debug.Log("Time is " + _time);
        if (_time <= _currentBestTime)
        {
            Debug.Log("Time is less than!");
            GetMinutesAndSeconds(out min, out sec);
            StoreBestTimeAndGetNew(min, sec);
        }
        _bestTimeMinuteValue.text = PlayerPrefs.GetString("bestTimeMinutes");
        _bestTimeSecondsValue.text = PlayerPrefs.GetString("bestTimeSeconds");
    }

    void GetMinutesAndSeconds(out string min, out string sec)
    {
        int minutes = (int)Mathf.Floor(_time / 60);
        if (minutes.ToString().Length == 1)
        {
            min = "0" + minutes.ToString();
        }
        else
        {
            min = minutes.ToString();
        }

        int seconds = (_time - (minutes * 60));
        if (seconds.ToString().Length == 1)
        {
            sec = "0" + seconds.ToString();
        }
        else
        {
            sec = seconds.ToString();
        }
    }

    public void CalculateAndDisplayCoins()
    {
        if (!_displayedCoins)
        {
            _displayedCoins = true;
            if (_time <= 900 && _time > 840)
            {
                _coins += 10;
            }
            else if (_time <= 840 && _time > 780)
            {
                _coins += 20;
            }
            else if (_time <= 780 && _time > 720)
            {
                _coins += 30;
            }
            else if (_time <= 720 && _time > 660)
            {
                _coins += 40;
            }
            else if (_time <= 660)
            {
                _coins += 50;
            }

            if (_damageTaken <= 600 && _damageTaken > 500)
            {
                _coins += 10;
            }
            else if (_damageTaken <= 500 && _damageTaken > 400)
            {
                _coins += 30;
            }
            else if (_damageTaken <= 400 && _damageTaken > 300)
            {
                _coins += 40;
            }
            else if (_damageTaken <= 300)
            {
                _coins += 50;
            }

            if (_upgrades >= 30 && _upgrades < 40)
            {
                _coins += 10;
            }
            else if (_upgrades >= 40 && _upgrades < 50)
            {
                _coins += 27;
            }
            else if (_upgrades >= 50 && _upgrades < 60)
            {
                _coins += 40;
            }
            else if (_upgrades >= 60)
            {
                _coins += 50;
            }

            _noOfCoinsGainedValue.text = _coins.ToString();
        }
    }
    #endregion

    public void Pause()
    {
        SpawnManager.Instance.AudioMethod(_button, false, .6f, false);
        Debug.Log("Calling Pause Method");
        _pauseMenu.SetActive(true);
        _pauseButton.SetActive(false);
        Time.timeScale = 0;
    }

    public void Restart()
    {
        SpawnManager.Instance.AudioMethod(_button, false, .6f, false);
        Time.timeScale = 1;
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

    public void Menu()
    {
        SpawnManager.Instance.AudioMethod(_button, false, .6f, false);
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void Continue()
    {
        SpawnManager.Instance.AudioMethod(_button, false, .6f, false);
        Time.timeScale = 1;
        _pauseMenu.SetActive(false);
        _pauseButton.SetActive(true);
    }
}
