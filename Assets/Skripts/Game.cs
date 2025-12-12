using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine;
using YG;
using System.Collections;

public class Game : MonoBehaviour
{
    [Header("Player Reference")]
    [SerializeField] private Health _playerHealth;
    [SerializeField] private InputHandler _inputHandler;

    [Header("Wave Manager")]
    [SerializeField] private WaveEnemy _waveManager;

    [Header("UI Panels")]
    [SerializeField] private GameObject _gamePanel;
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _victoryPanel;
    [SerializeField] private GameObject _defeatPanel;
    [SerializeField] private GameObject _helpPanel;

    [Header("Victory Panel")]
    [SerializeField] private Button _victoryRestartButton;

    [Header("Defeat Panel")]
    [SerializeField] private TextMeshProUGUI _defeatWaveText;
    [SerializeField] private Button _defeatRestartButton;
    [SerializeField] private string _defeatFormat = "You survived {0} waves";

    [Header("Help Panel Settings")]
    [SerializeField] private float _helpPanelDuration = 5f;

    private Coroutine _helpPanelCoroutine;

    private bool _gameEnded = false;
    private bool _isPaused = false;

    private void Start()
    {
        SubscribeToEvents();

        SetPanelState(_victoryPanel, false);
        SetPanelState(_defeatPanel, false);
        SetPanelState(_pausePanel, false);
        SetPanelState(_gamePanel, true);
        SetPanelState(_helpPanel, true);

        SetupButtons();

        StartHelpPanelTimer();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        YG2.InterstitialAdvShow();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void StartHelpPanelTimer()
    {
        if (_helpPanelCoroutine != null)
            StopCoroutine(_helpPanelCoroutine);

        _helpPanelCoroutine = StartCoroutine(HideHelpPanelAfterDelay());
    }

    private void SubscribeToEvents()
    {
        _playerHealth.Death += OnPlayerDeath;
        _waveManager.AllWavesCompleted += OnAllWavesCompleted;
        _inputHandler.EscapePressed += OnEscapePressed;
    }

    private void SetupButtons()
    {
        _victoryRestartButton.onClick.AddListener(RestartGame);
        _defeatRestartButton.onClick.AddListener(RestartGame);
    }

    private void OnPlayerDeath()
    {
        if (_gameEnded) 
            return;

        _gameEnded = true;

        if (_helpPanelCoroutine != null)
        {
            StopCoroutine(_helpPanelCoroutine);
            _helpPanelCoroutine = null;
        }

        ShowDefeatPanel();
    }

    private void OnAllWavesCompleted()
    {
        if (_gameEnded) 
            return;

        _gameEnded = true;

        if (_helpPanelCoroutine != null)
        {
            StopCoroutine(_helpPanelCoroutine);
            _helpPanelCoroutine = null;
        }

        ShowVictoryPanel();
    }

    private void OnEscapePressed()
    {
        if (_gameEnded == false)
            TogglePause();
    }

    private void TogglePause()
    {
        _isPaused = !_isPaused;

        if (_isPaused)
        {
            //SetPanelState(_gamePanel, false);
            SetPanelState(_helpPanel, true);
            SetPanelState(_pausePanel, true);

            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            SetPanelState(_pausePanel, false);
            SetPanelState(_helpPanel, false);
            //SetPanelState(_gamePanel, true);

            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void ShowVictoryPanel()
    {
        SetPanelState(_gamePanel, false);
        SetPanelState(_helpPanel, false);
        SetPanelState(_victoryPanel, true);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
    }

    private void ShowDefeatPanel()
    {
        SetPanelState(_gamePanel, false);
        SetPanelState(_helpPanel, false);
        SetPanelState(_defeatPanel, true);

        if (_defeatWaveText != null && _waveManager != null)
        {
            int wavesCompleted = Mathf.Max(0, _waveManager.CurrentWave - 1);
            _defeatWaveText.text = string.Format(_defeatFormat, wavesCompleted);
        }

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
    }

    private void SetPanelState(GameObject panel, bool state)
    {
        if (panel != null)
            panel.SetActive(state);
    }

    private IEnumerator HideHelpPanelAfterDelay()
    {
        yield return new WaitForSeconds(_helpPanelDuration);

        SetPanelState(_helpPanel, false);
    }

    private void OnDestroy()
    {
        if (_playerHealth != null)
            _playerHealth.Death -= OnPlayerDeath;

        if (_waveManager != null)
            _waveManager.AllWavesCompleted -= OnAllWavesCompleted;

        if (_victoryRestartButton != null)
            _victoryRestartButton.onClick.RemoveAllListeners();

        if (_defeatRestartButton != null)
            _defeatRestartButton.onClick.RemoveAllListeners();

        if (_helpPanelCoroutine != null)
        {
            StopCoroutine(_helpPanelCoroutine);
            _helpPanelCoroutine = null;
        }
    }
}