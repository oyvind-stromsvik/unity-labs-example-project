using System;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class MenuManager : MonoBehaviour {

	public GameObject player;
    public Transform playerStartPosition;

    private Camera _camera;

    public Animator animator;

    public Transform flythroughCameraMount;
    public Transform playerCameraMount;
    public Transform playerCameraRigRoot;

	public GameObject UIParent;
	public CameraRotation camRot;
	public Animator HUDAnimator;
	public ObjectivesController objController;

    public float TimeBeforeInteractive = 4f;
    public float IdleTimeout = 60f;
    private float _lastInputAt;

    public enum GameModes
    {
        None,
        Flythrough,
        Interactive
    }

    private GameModes _currentMode;
    private GameModes _nextMode;
    public GameModes StartMode;

	void Awake () {
		if(!player)
    		player = GameObject.Find ("CharacterScientist");
        _camera = Camera.main;
	}

	void Start() {
        // Scene is usually set up for interactive mode so we exit it to ensure things get turned off
        _currentMode = GameModes.Interactive;
        _nextMode = StartMode;
        OnFadeoutComplete();
	}

    public void EnterFlythroughMode()
    {
        _currentMode = GameModes.Flythrough;

        flythroughCameraMount.gameObject.SetActive(true);
        _camera.transform.parent = flythroughCameraMount;
        _camera.transform.localPosition = Vector3.zero;
        _camera.transform.localRotation = Quaternion.identity;

        player.SetActive(false);

        animator.SetTrigger("ShowMenuUI");

        flythroughCameraMount.GetComponent<Animator>().Play("FlyThroughCamera", 0, 0);
    }

    public void ExitFlythroughMode()
    {
        flythroughCameraMount.gameObject.SetActive(false);
    }

    public void EnterInteractiveMode()
    {

	#if !UNITY_EDITOR
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	#endif

        _currentMode = GameModes.Interactive;

        playerCameraRigRoot.gameObject.SetActive(true);
        _camera.transform.parent = playerCameraMount;
        _camera.transform.localPosition = Vector3.zero;
        _camera.transform.localRotation = Quaternion.identity;

        player.SetActive(true);
        player.transform.position = playerStartPosition.position;
        player.transform.rotation = playerStartPosition.rotation;
        player.GetComponent<SetupAndUserInput>().InputEnabled = false;
        
        playerCameraRigRoot.GetComponent<CameraRotation>().ResetAngles();
        playerCameraRigRoot.GetComponent<FollowObject>().TeleportToTarget();

        foreach (var elevator in FindObjectsOfType<Elevator>())
            elevator.TeleportToFloor(1);

        UIParent.SetActive(true);

        StartCoroutine(BeginPlay());
    }

    public void ExitInteractiveMode()
    {
		#if !UNITY_EDITOR
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		#endif

        camRot.enabled = false;
        playerCameraRigRoot.gameObject.SetActive(false);
        player.GetComponent<SetupAndUserInput>().InputEnabled = false;
        player.SetActive(false);
        UIParent.SetActive(false);
    }

    public void Launch()
    {
        _nextMode = GameModes.Interactive;
        animator.SetTrigger("ChangeGameMode");
        animator.SetTrigger("HideMenuUI");
    }

    public void Exit()
    {
        _nextMode = GameModes.None;
        animator.SetTrigger("ChangeGameMode");
        animator.SetTrigger("HideMenuUI");
    }

    public void OnFadeoutComplete()
    {
        switch (_currentMode)
        {
            case GameModes.Flythrough:
                ExitFlythroughMode();
                break;
            case GameModes.Interactive:
                ExitInteractiveMode();
                break;
            case GameModes.None:
                break;
        }

        switch (_nextMode)
        {
            case GameModes.Flythrough:
                EnterFlythroughMode();
                break;
            case GameModes.Interactive:
                EnterInteractiveMode();
                break;
             case GameModes.None:
                Application.Quit();
                break;
        }

        _lastInputAt = Time.time;

        _nextMode = GameModes.None;
    }
	
	private IEnumerator BeginPlay()
	{
        yield return new WaitForSeconds(TimeBeforeInteractive);
		
		camRot.enabled = true;
		player.GetComponent<SetupAndUserInput>().InputEnabled = true;
	    //var hudAnimator = GameObject.FindGameObjectWithTag("HUD").GetComponent<Animator>();
        HUDAnimator.enabled = true;
        HUDAnimator.Play("HUDAnimateIn");
		//var objController = FindObjectOfType<ObjectivesController>();
		objController.BeginFirstObjective();
	}

    public void Update()
    {
        if(Input.anyKey)
            _lastInputAt = Time.time;
        _lastInputAt = Mathf.Max(_lastInputAt, player.GetComponent<SetupAndUserInput>().LastInputAt);
        _lastInputAt = Mathf.Max(_lastInputAt, camRot.LastInputAt);

        if (_currentMode == GameModes.Interactive && _nextMode == GameModes.None && (Input.GetButtonDown("ExitInteractiveMode") || Time.time > (_lastInputAt + IdleTimeout)))
        {
            HUDAnimator.Play("HUDAnimateOut");
            objController.ClearCurrentPOI();
            _nextMode = GameModes.Flythrough;
            animator.SetTrigger("ChangeGameMode");
        }

        
    }
}
