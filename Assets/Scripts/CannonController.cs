using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonController : MonoBehaviour
{
    [SerializeField]
    private Transform _aimingTransform = null;
    [SerializeField]
    private Transform _firePoint = null;
    [SerializeField]
    private GameObject _cannonballPrefab = null;
    [SerializeField]
    private float _ballVelocity = 20.0f;
    [SerializeField]
    private float _reloadTime = 1.0f;
    private float _reloadTimer = 1.0f;

    void Update()
    {
        if (_reloadTimer < _reloadTime)
        {
            _reloadTimer += Time.deltaTime;
        }
    }

    public void Aim(float pitch, float yaw)
    {
        if (_aimingTransform != null)
        {
            _aimingTransform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
        }
    }

    public void Fire()
    {
        if (_firePoint != null && _cannonballPrefab != null && _reloadTimer >= _reloadTime)
        {
            GameObject ball = Instantiate(_cannonballPrefab, _firePoint.position, _firePoint.rotation);
            Rigidbody rb = ball.GetComponent<Rigidbody>();
            rb.velocity = _firePoint.forward.normalized * _ballVelocity;

            _reloadTime = 0.0f;
        }
    }

    public float GetPitch()
    {
        return _aimingTransform.eulerAngles.x;
    }

    public float GetYaw()
    {
        return _aimingTransform.eulerAngles.y;
    }
}
