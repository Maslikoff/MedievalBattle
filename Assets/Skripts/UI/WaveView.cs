using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaveView : MonoBehaviour
{
    [Header("Wave Display")]
    [SerializeField] private TextMeshProUGUI _waveText;
    [SerializeField] private string _waveFormat = "Wave: {0}/{1}";

    [Header("Boss Display")]
    [SerializeField] private GameObject _bossPanel;
    [SerializeField] private TextMeshProUGUI _bossText;
    [SerializeField] private string _bossMessage = "BOSS!";
    [SerializeField] private float _bossDisplayTime = 3f;

    [Header("Setting")]
    [SerializeField] private WaveEnemy _waveManager;

    private Coroutine _bossDisplayCoroutine;

    private void Start()
    {
        _waveManager.WaveStarted += OnWaveStarted;
        _waveManager.WaveCompleted += OnWaveCompleted;

        _bossPanel.SetActive(false);

        UpdateWaveDisplay(_waveManager.CurrentWave, _waveManager.TotalWaves);
    }

    public void OnBossSpawned()
    {
        ShowBossWarning();
    }

    private void OnWaveStarted(int waveNumber)
    {
        UpdateWaveDisplay(waveNumber, _waveManager.TotalWaves);
    }

    private void OnWaveCompleted(int waveNumber)
    {
        if (waveNumber < _waveManager.TotalWaves && IsBossWave(waveNumber + 1))
            ShowBossWarning();
    }

    private void UpdateWaveDisplay(int currentWave, int totalWaves)
    {
        _waveText.text = string.Format(_waveFormat, currentWave, totalWaves);
    }

    private bool IsBossWave(int waveNumber) => waveNumber == _waveManager.TotalWaves;

    private void ShowBossWarning()
    {
        if (_bossDisplayCoroutine != null)
            StopCoroutine(_bossDisplayCoroutine);

        _bossDisplayCoroutine = StartCoroutine(BossDisplayRoutine());
    }

    private IEnumerator BossDisplayRoutine()
    {
        _bossPanel.SetActive(true);
        _bossText.text = _bossMessage;

        yield return new WaitForSeconds(_bossDisplayTime);

        _bossPanel.SetActive(false);
        _bossDisplayCoroutine = null;
    }

    private void OnDestroy()
    {
        if (_waveManager != null)
        {
            _waveManager.WaveStarted -= OnWaveStarted;
            _waveManager.WaveCompleted -= OnWaveCompleted;
        }

        if (_bossDisplayCoroutine != null)
            StopCoroutine(_bossDisplayCoroutine);
    }
}