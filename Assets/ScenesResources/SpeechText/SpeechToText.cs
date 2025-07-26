using Oculus.Voice.Dictation;
using TMPro;
using UnityEngine;
using System.Collections;

public class SpeechToText : MonoBehaviour
{
    public AppDictationExperience dictationExperience;
    public TextMeshProUGUI transcriptionText;
    public float retryDelay = 2.0f;

    private bool _isDictationActive = false;
    private bool _isRestarting = false;
    private int maxLines = 3;
    void Start()
    {
        ConfigureDictation();
        StartDictation();
    }

    void OnDestroy()
    {
        CleanupDictation();
    }

    private void ConfigureDictation()
    {
        dictationExperience.DictationEvents.OnPartialTranscription.AddListener(OnPartial);
        dictationExperience.DictationEvents.OnFullTranscription.AddListener(OnFull);
        dictationExperience.DictationEvents.OnError.AddListener(OnDictationError);
        dictationExperience.DictationEvents.OnStoppedListening.AddListener(OnDictationStopped);
        dictationExperience.DictationEvents.OnStartListening.AddListener(OnDictationStarted);
    }

    private void CleanupDictation()
    {
        dictationExperience.DictationEvents.OnPartialTranscription.RemoveListener(OnPartial);
        dictationExperience.DictationEvents.OnFullTranscription.RemoveListener(OnFull);
        dictationExperience.DictationEvents.OnError.RemoveListener(OnDictationError);
        dictationExperience.DictationEvents.OnStoppedListening.RemoveListener(OnDictationStopped);
        dictationExperience.DictationEvents.OnStartListening.RemoveListener(OnDictationStarted);

        if (_isDictationActive)
        {
            dictationExperience.Deactivate();
            _isDictationActive = false;
        }
    }

    private void StartDictation()
    {
        if (!_isDictationActive && !_isRestarting)
        {
            _isDictationActive = true;
            dictationExperience.Activate();
            Debug.Log("Dictation started.");
        }
    }

    private void OnDictationStarted()
    {
        _isRestarting = false;
        Debug.Log("Dictation successfully started.");
    }

    private void OnPartial(string text)
    {
        transcriptionText.text = text;

        transcriptionText.ForceMeshUpdate();

        if (transcriptionText.textInfo.lineCount > maxLines)
        {
            transcriptionText.text = text;
        }
    }

    private void OnFull(string text)
    {
        transcriptionText.text = text;

        transcriptionText.ForceMeshUpdate();

        if (transcriptionText.textInfo.lineCount > maxLines)
        {
            transcriptionText.text = text; 
        }
    }

    private void OnDictationError(string errorType, string errorMessage)
    {
        Debug.LogError($"Dictation error: {errorType} - {errorMessage}");
        HandleDictationStop();
    }

    private void OnDictationStopped()
    {
        Debug.Log("Dictation stopped by system.");
        HandleDictationStop();
    }

    private void HandleDictationStop()
    {
        if (_isDictationActive || _isRestarting)
        {
            _isDictationActive = false;

            if (!_isRestarting)
            {
                _isRestarting = true;
                StartCoroutine(RestartDictationCoroutine());
            }
        }
    }

    private IEnumerator RestartDictationCoroutine()
    {
        yield return new WaitForSeconds(retryDelay);

        dictationExperience.Deactivate();
        yield return new WaitForEndOfFrame();

        Debug.Log("Attempting to restart dictation...");
        _isRestarting = false;
        StartDictation();
    }
}