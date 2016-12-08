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
    private float _minimumVelocity = 1.0f;
    [SerializeField]
    private float _maxVelocity = 15.0f;
    [SerializeField]
    private float _chargeRate = 2.0f;
    [SerializeField]
    private float _defaultBallVelocity = 5.0f;
    private float _chargedVelocity = 0.0f;
    private bool _charging = false;
    [SerializeField]
    private float _reloadTime = 1.0f;
    private float _cooldown = 0.0f;
    private bool _loaded = true;

    void Update()
    {
        if (!_loaded)
        {
            _cooldown -= Time.deltaTime;
            _loaded = _cooldown <= 0;
        }
        else if (_charging)
        {
            _chargedVelocity += _chargeRate * Time.deltaTime;
            _chargedVelocity = Mathf.Min(_chargedVelocity, _maxVelocity);
        }
    }

    public void Aim(float pitch, float yaw)
    {
        if (_aimingTransform != null)
        {
            _aimingTransform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
        }
    }

    public void StartCharging()
    {
        if (_loaded && !_charging)
        {
            _charging = true;
            _chargedVelocity = _minimumVelocity;
        }
    }

    public void Fire()
    {
        if (_firePoint != null && _cannonballPrefab != null && _loaded)
        {
            GameObject ball = Instantiate(_cannonballPrefab, _firePoint.position, _firePoint.rotation);
            Rigidbody rb = ball.GetComponent<Rigidbody>();
            rb.velocity = _firePoint.forward.normalized * (_charging ? _chargedVelocity : _defaultBallVelocity);

            _cooldown = _reloadTime;
            _charging = false;
            _loaded = false;
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
