using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

public class GazeGestureManager : MonoBehaviour
{
    public static GazeGestureManager Instance
    {
        get;
        private set;
    }

    [System.NonSerialized]
    public Vector3                      _headPosition                   = Vector3.zero;
    [System.NonSerialized]
    public Vector3                      _viewDirection                  = Vector3.zero;

    [Header("Cursor")]
    [SerializeField]
    private Transform                   _cursorTransform                = null;
    [SerializeField]
    private MeshRenderer                _cursorRenderer                 = null;
    private bool                        _gazing                         = true;

    private GameObject                  _focusedObject                  = null;

    [Header("Cannon")]
    public GameObject                   _cannonPrefab                   = null;
    private GameObject                  _cannon                         = null;
    private CannonController            _cannonController               = null;
    private GestureRecognizer           _fireCannonGestureHandler       = null;

	[Header("Target")]
	public GameObject 					_targetPrefab 					= null;
	public Transform					_targetContainer				= null;

    [Header("Move object parameters")]
    [SerializeField]
    private float                       _moveSpeed                      = 2.0f;
    private GestureRecognizer           _moveObjectGestureHandler       = null;
    private bool                        _moving                         = false;
    private Vector3                     _prevPos                        = Vector3.zero;
    private Vector3                     _moveDir                        = Vector3.zero;

    [Header("Rotate object parameters")]
    [SerializeField]
    private float                       _rotationSpeed                  = 20.0f;
    private GestureRecognizer           _rotateObjectGestureHandler     = null;
    private bool                        _rotating                       = false;
    private float                       _yawPace                        = 0.0f;
    private float                       _pitchPace                      = 0.0f;
    private float                       _yaw                            = 0.0f;
    private float                       _pitch                          = 0.0f;

    void Awake()
    {
        Instance = this;

        // Set up gesture recognizer for firing cannon
        _fireCannonGestureHandler = new GestureRecognizer();
        _fireCannonGestureHandler.SetRecognizableGestures(GestureSettings.Tap | GestureSettings.Hold);
        _fireCannonGestureHandler.TappedEvent += SingleFire;
        _fireCannonGestureHandler.HoldStartedEvent += FiringStarted;
        _fireCannonGestureHandler.HoldCompletedEvent += FiringCompleted;
        _fireCannonGestureHandler.HoldCanceledEvent += FiringCancelled;

        // Set up gesture recognizer for moving objects around
        _moveObjectGestureHandler = new GestureRecognizer();
        _moveObjectGestureHandler.SetRecognizableGestures(GestureSettings.ManipulationTranslate);
        _moveObjectGestureHandler.ManipulationStartedEvent += MoveObjectStarted;
        _moveObjectGestureHandler.ManipulationUpdatedEvent += MoveObjectUpdated;
        _moveObjectGestureHandler.ManipulationCompletedEvent += MoveObjectCompleted;
        _moveObjectGestureHandler.ManipulationCanceledEvent += MoveObjectCanceled;

        // Set up gesture handler for rotating objects
        _rotateObjectGestureHandler = new GestureRecognizer();
        _rotateObjectGestureHandler.SetRecognizableGestures(GestureSettings.NavigationX | GestureSettings.NavigationY);
        _rotateObjectGestureHandler.NavigationStartedEvent += RotateObjectStarted;
        _rotateObjectGestureHandler.NavigationUpdatedEvent += RotateObjectUpdated;
        _rotateObjectGestureHandler.NavigationCompletedEvent += RotateObjectCompleted;
        _rotateObjectGestureHandler.NavigationCanceledEvent += RotateObjectCanceled;
    }

    void Update()
    {
        // Update head position and view direction
        _headPosition = Camera.main.transform.position;
        _viewDirection = Camera.main.transform.forward;

        // If we're simply looking around, reposition cursor
        if (_gazing)
        {
            RaycastHit hitInfo;

            // Check if we hit any meshes in the view direction
            if (Physics.Raycast(_headPosition, _viewDirection, out hitInfo))
            {
                // Turn on renderer for cursor
                _cursorRenderer.enabled = true;

                // Set cursor at hit position and rotate it so that the up-axis points along normal
                _cursorTransform.position = hitInfo.point;
                _cursorTransform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);

                // Set the hit object as the currently focused object
                _focusedObject = hitInfo.collider.gameObject;
                // Special check for if the object is the cannon, since the colliders are on the base and the barrel and we want to target the cannon as a whole
                if (_focusedObject.CompareTag("CannonChild"))
                {
                    _focusedObject = GameObject.FindGameObjectWithTag("Cannon");
                }
            }
            else
            {
                // Don't render the cursor and set focused object to null
                _cursorRenderer.enabled = false;
                _focusedObject = null;
            }
        }
        else
        {
            // Don't render the cursor when we aren't trying to target something
            _cursorRenderer.enabled = false;
        }
        
        if (_moving)
        {
            _focusedObject.transform.position += _moveDir * _moveSpeed;
        }
        
        if (_rotating)
        {
            _pitch = _cannonController.GetPitch() + (_pitchPace * _rotationSpeed * Time.deltaTime);
            _yaw = _cannonController.GetYaw() + (_yawPace * _rotationSpeed * Time.deltaTime);

            _cannonController.Aim(_pitch, _yaw);
        }
    }

    #region VoiceCommandActions
    #region StateSwitchingFunctions
    public void ActivateMoveObjectsMode()
    {
        _rotateObjectGestureHandler.StopCapturingGestures();
        _fireCannonGestureHandler.StopCapturingGestures();
        _moveObjectGestureHandler.StartCapturingGestures();

        if (_cannonController != null)
        {
            _cannonController.SetMoveMode();
        }
    }

    public void ActivateRotateObjectsMode()
    {
        _moveObjectGestureHandler.StopCapturingGestures();
        _fireCannonGestureHandler.StopCapturingGestures();
        _rotateObjectGestureHandler.StartCapturingGestures();

        if (_cannonController != null)
        {
            _cannonController.SetRotateMode();
        }
    }

    public void ActivateFiringMode()
    {
        _moveObjectGestureHandler.StopCapturingGestures();
        _rotateObjectGestureHandler.StopCapturingGestures();
        _fireCannonGestureHandler.StartCapturingGestures();

		if (_cannonController != null) {
            _cannonController.SetFiringMode();
		}
    }
    #endregion

    public void CreateCannon()
    {
        if (_cannon == null && _cannonPrefab != null)
        {
            _cannon = Instantiate(_cannonPrefab, _headPosition + (_viewDirection.normalized * 2), Quaternion.identity);
            _cannonController = _cannon.GetComponent<CannonController>();
        }
    }

	public void CreateTarget()
	{
		if (_targetContainer != null && _targetPrefab != null)
		{
			Instantiate(_targetPrefab, _headPosition + (_viewDirection.normalized * 2), Quaternion.identity, _targetContainer);
		}
	}

    public void Fire()
    {
        if (_cannonController != null)
        {
            _cannonController.Fire();
        }
    }

    public void ResetWorld()
    {
        GameObject temp = _cannon;

        _cannon = null;
        _cannonController = null;

		// Destroy cannon
        Destroy(temp);

		// Destroy targets
		int childCount = _targetContainer.childCount;

		for (int i = 0; i < childCount; ++i)
		{
			Destroy(_targetContainer.GetChild (0).gameObject);
		}
    }
    #endregion

    #region MoveObjectDelegates
    private void MoveObjectStarted(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headRay)
    {
        // At the moment we only want to move the cannon
		if (_gazing && _focusedObject != null && (_focusedObject.CompareTag("Cannon") || _focusedObject.CompareTag("Target")))
        {
            _gazing = false;
            _moving = true;
            _prevPos = Vector3.zero;

            _moveDir = cumulativeDelta - _prevPos;
            _prevPos += _moveDir;
        }
    }

    private void MoveObjectUpdated(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headRay)
    {
		if (_moving && _focusedObject != null && (_focusedObject.CompareTag("Cannon") || _focusedObject.CompareTag("Target")))
        {
            _moveDir = cumulativeDelta - _prevPos;
            _prevPos += _moveDir;
        }
    }

    private void MoveObjectCompleted(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headRay)
    {
        _moving = false;
        _gazing = true;

        _moveDir = Vector3.zero;
        _prevPos = Vector3.zero;
    }

    private void MoveObjectCanceled(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headRay)
    {
        _moving = false;
        _gazing = true;

        _moveDir = Vector3.zero;
        _prevPos = Vector3.zero;
    }
    #endregion

    #region RotateObjectDelegates

    private void RotateObjectStarted(InteractionSourceKind source, Vector3 normalizedOffset, Ray headRay)
    {
        // At the moment we only want to rotate the cannon
		if (_gazing && _focusedObject != null && (_focusedObject.CompareTag("Cannon") || _focusedObject.CompareTag("Target")))
        {
            _gazing = false;
            _rotating = true;

            _yawPace = -normalizedOffset.x;
            _pitchPace = normalizedOffset.y;
        }
    }

    private void RotateObjectUpdated(InteractionSourceKind source, Vector3 normalizedOffset, Ray headRay)
    {
		if (_rotating && _focusedObject != null && (_focusedObject.CompareTag("Cannon") || _focusedObject.CompareTag("Target")))
        {
            _moveDir = normalizedOffset - _prevPos;
            _prevPos += _moveDir;

            _yawPace = -normalizedOffset.x;
            _pitchPace = normalizedOffset.y;
        }
    }

    private void RotateObjectCompleted(InteractionSourceKind source, Vector3 normalizedOffset, Ray headRay)
    {
        _rotating = false;
        _gazing = true;

        _yawPace = 0.0f;
        _pitchPace = 0.0f;
    }

    private void RotateObjectCanceled(InteractionSourceKind source, Vector3 normalizedOffset, Ray headRay)
    {
        _rotating = false;
        _gazing = true;

        _yawPace = 0.0f;
        _pitchPace = 0.0f;
    }
    #endregion

    #region FireCannonDelegates
    private void SingleFire(InteractionSourceKind source, int tapCount, Ray headRay)
    {
        _cannonController.Fire();
    }

    private void FiringStarted(InteractionSourceKind source, Ray headRay)
    {
        if (_gazing)
        {
            _gazing = false;
            _cannonController.StartCharging();
        }
    }

    private void FiringCompleted(InteractionSourceKind source, Ray headRay)
    {
        _cannonController.Fire();
        _gazing = true;
    }

    private void FiringCancelled(InteractionSourceKind source, Ray headRay)
    {
        _cannonController.Fire();
        _gazing = true;
    }
    #endregion

    private GazeGestureManager() { }
}
