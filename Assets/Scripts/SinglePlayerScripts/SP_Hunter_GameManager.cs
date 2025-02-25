using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;
//This is the script that manages most things in the hunter trial.
//This is also where I first learned about UnityEvents so I tested them out here. - Love
public class SP_Hunter_GameManager : MonoBehaviour
{
    PlayerInput _playerInput;
    [SerializeField] SP_NPC_PreyManager _preyNPCManager;
    [SerializeField] GameObject _startPoint;
    [SerializeField] GameObject _player;
    [SerializeField] GameObject _timerHolder;
    [SerializeField] TMP_Text _timerDisplay;
    [SerializeField] string _timerFilePath;
    private GameObject _startPointStopper;
    IEnumerator _removeStartStopper;

    public UnityEvent startLevel;
    public UnityEvent resetLevel;

    private float? _timerTime;
    private float? _bestTime;
    

    void Awake()
    {
        _playerInput = new PlayerInput();
        _playerInput.HunterControls.ResetSPLevel.started += OnResetLevel;
        _playerInput.HunterControls.Escape.started += OnEscape;
        _startPointStopper = _startPoint.transform.GetChild(0).gameObject;
        _bestTime = JSON_Handler.Load(_timerFilePath);
        _timerDisplay.text = "Best Time: " + _bestTime?.ToString("0.##") ?? "No saved times";
        _timerDisplay.text += "\n Latest Time: ";
    }

    public void StartLevel()
    {
        //Just in case we somehow missed deleting the last timer due to something unexpected happening we delete it again if it exist - Love
        if (_timerHolder.GetComponent<SP_Timer>()) Destroy(_timerHolder.GetComponent<SP_Timer>());
        _timerHolder.AddComponent<SP_Timer>();
        startLevel.Invoke();
        Debug.Log("Level started");
    }


    public void FinishLevel()
    {
        //End timer, save time and update the display board
        if (_timerHolder.GetComponent<SP_Timer>())
        {
            _timerTime = _timerHolder.GetComponent<SP_Timer>().GetTime();
            Destroy(_timerHolder.GetComponent<SP_Timer>());
        }
        else _timerTime = null;
        WriteTimer(_timerTime);
        Debug.Log("Level finished");
        ResetLevel();
    }

    //Resets the level, including player position, timer, NPC position, and adding the "start stopper" - Love
    public void ResetLevel()
    {
        resetLevel.Invoke();
        _preyNPCManager.RespawnPrey();
        if (_timerHolder.GetComponent<SP_Timer>()) Destroy(_timerHolder.GetComponent<SP_Timer>());
        _startPoint.SetActive(true);
        _startPointStopper.SetActive(true);
        _player.transform.position = _startPoint.transform.position + new Vector3(0, 1.5f, 0);
        _removeStartStopper = RemoveStartStopper();
        StartCoroutine(_removeStartStopper);
    }

    //The "start stopper" is a barrier within the start bubble which exists for a second after restarting a level so you don't accidentally
    //just run straight out of the bubble if you were holding W as you won/reset the level. - Love
    IEnumerator RemoveStartStopper()
    {
        yield return new WaitForSeconds(1f);
        _startPointStopper.SetActive(false);
    }

    //Returns the player to the SP menu. Resets the level before that to avoid any lingering timers being loaded in the background. - Love
    //The coroutine being started but not given enough time to finish before we switch does not seem to create any issues after extensive testing - Love
    private void ReturnToMenu()
    {
        ResetLevel();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("SP_Menu");
    }

    void OnResetLevel(InputAction.CallbackContext context)
    {
        ResetLevel();
    }

    void OnEscape(InputAction.CallbackContext context)
    {
        ReturnToMenu();
    }

    public void WriteTimer(float? _inputTime)
    {
        if (_inputTime == null) return;
        _bestTime = JSON_Handler.CheckOverwrite(_inputTime.Value, _timerFilePath);
        _timerDisplay.text = "Best Time: " + _bestTime?.ToString("0.##") ?? "No saved times";
        _timerDisplay.text += "\n Latest Time: " + _inputTime?.ToString("0.##") ?? string.Empty;
    }

    void OnEnable()
    {
        _playerInput.HunterControls.Enable();
    }

    void OnDisable()
    {
        _playerInput.HunterControls.Disable();
    }
}
