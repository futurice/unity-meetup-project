using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasOrientator : MonoBehaviour
{

    [SerializeField]
    private Transform _headPosition = null;

    private Vector3 _canvasToHead = Vector3.zero;

    void Start()
    {
        _headPosition = Camera.main.transform;
        _canvasToHead = (_headPosition.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(_canvasToHead, Vector3.up);
    }
    
	void Update ()
    {
        _canvasToHead = (_headPosition.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(_canvasToHead, Vector3.up);
    }
}
