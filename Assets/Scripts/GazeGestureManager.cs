using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

public class GazeGestureManager : MonoBehaviour {

    public GazeGestureManager Instance { get; private set; }

    public Vector3 _headPosition;
    public Vector3 _viewDirection;

    [SerializeField]
    private Transform _cursorTransform = null;
    [SerializeField]
    private MeshRenderer _cursorRenderer = null;
    private bool gazing = true;

    private GameObject _focusedObject;

    private GestureRecognizer _moveObjectGestureHandler;

    void Awake()
    {
        _moveObjectGestureHandler = new GestureRecognizer();
        _moveObjectGestureHandler.SetRecognizableGestures(GestureSettings.ManipulationTranslate);
        _moveObjectGestureHandler.ManipulationStartedEvent += MoveObjectStarted;
        _moveObjectGestureHandler.ManipulationUpdatedEvent += MoveObjectUpdated;
        _moveObjectGestureHandler.ManipulationCompletedEvent += MoveObjectCompleted;
        _moveObjectGestureHandler.ManipulationCanceledEvent += MoveObjectCanceled;
    }

    void Update()
    {
        // Update head position and view direction
        _headPosition = Camera.main.transform.position;
        _viewDirection = Camera.main.transform.forward;

        // If we're simply looking around, reposition cursor
        if (gazing)
        {
            RaycastHit hitInfo;

            if (Physics.Raycast(_headPosition, _viewDirection, out hitInfo))
            {
                // Turn on renderer for cursor
                _cursorRenderer.enabled = true;

                // Set cursor at hit position and rotate it so that the up-axis points along normal
                _cursorTransform.position = hitInfo.point;
                _cursorTransform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);

                // Set the hit object as the currently focused object
                _focusedObject = hitInfo.collider.gameObject;
            }
            else
            {
                // Don't render the cursor and set focused object to null
                _cursorRenderer.enabled = false;
                _focusedObject = null;
            }
        }
    }

    #region MoveObjectDelegates
    private void MoveObjectStarted(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headRay)
    { }

    private void MoveObjectUpdated(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headRay)
    { }

    private void MoveObjectCompleted(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headRay)
    { }

    private void MoveObjectCanceled(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headRay)
    { }
    #endregion

    private GazeGestureManager() { }
}
