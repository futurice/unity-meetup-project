using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonController : MonoBehaviour
{
    [Header("Cannon controls")]
    [SerializeField]
    private Transform           _aimingTransform                    = null;
    [SerializeField]
    private Transform           _firePoint                          = null;

    [Header("Cannonball and firing")]
    [SerializeField]
    private GameObject          _cannonballPrefab                   = null;
    [SerializeField]
    private float               _minimumVelocity                    = 1.0f;
    [SerializeField]
    private float               _maxVelocity                        = 15.0f;
    [SerializeField]
    private float               _chargeRate                         = 2.0f;
    [SerializeField]
    private float               _defaultBallVelocity                = 5.0f;
    private float               _chargedVelocity                    = 0.0f;
    private bool                _charging                           = false;
    private bool                _onCooldown                         = false;
    [SerializeField]
    private float               _reloadTime                         = 1.0f;

    [Header("Effects")]
	[SerializeField]
	private ParticleSystem      _fireParticleSystem                 = null;
	[SerializeField]
	private GameObject          _igniteParticleSystemContainer      = null;
	[SerializeField]
	private AudioSource         _fireSound                          = null;


    void Update()
    {
        if (!_onCooldown && _charging)
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
        if (!_onCooldown && !_charging)
        {
            _charging = true;
            _chargedVelocity = _minimumVelocity;
        }
    }

    public void Fire()
    {
        if (_firePoint != null && _cannonballPrefab != null && !_onCooldown)
        {
            GameObject ball = Instantiate(_cannonballPrefab, _firePoint.position, _firePoint.rotation);
            Rigidbody rb = ball.GetComponent<Rigidbody>();

            rb.velocity = _firePoint.forward.normalized * (_charging ? _chargedVelocity : _defaultBallVelocity);
			_fireParticleSystem.Emit(1);
			_fireSound.Play();

            // Cooldown
            _charging = false;
            StartCoroutine(Cooldown());
        }
    }

	public void Ignite()
	{
		_igniteParticleSystemContainer.SetActive(true);
	}

    public float GetPitch()
    {
        return _aimingTransform.eulerAngles.x;
    }

    public float GetYaw()
    {
        return _aimingTransform.eulerAngles.y;
    }

	private IEnumerator Cooldown()
	{
		_onCooldown = true;
		yield return new WaitForSeconds (_reloadTime);
		_onCooldown = false;
		_fireParticleSystem.Stop (true, ParticleSystemStopBehavior.StopEmittingAndClear);
	}
}
