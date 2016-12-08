using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("UI")]
    [SerializeField]
    private Image               _chargeMeter                        = null;
    [SerializeField]
    private GameObject          _moveArrowsContainer                = null;
    [SerializeField]
    private GameObject          _rotateArrow                        = null;

    [Header("Effects")]
	[SerializeField]
	private ParticleSystem      _fireParticleSystem                 = null;
	[SerializeField]
	private GameObject          _igniteParticleSystemContainer      = null;
	[SerializeField]
	private AudioSource         _fireSound                          = null;

    void Awake()
    {
        if (_chargeMeter != null)
        {
            _chargeMeter.fillAmount = 0.0f;
        }
        if (_moveArrowsContainer != null)
        {
            _moveArrowsContainer.SetActive(false);
        }
        if (_rotateArrow != null)
        {
            _rotateArrow.SetActive(false);
        }
    }


    void Update()
    {
        if (!_onCooldown && _charging)
        {
            _chargedVelocity += _chargeRate * Time.deltaTime;
            _chargedVelocity = Mathf.Min(_chargedVelocity, _maxVelocity);
            _chargeMeter.fillAmount = (_chargedVelocity - _minimumVelocity) / (_maxVelocity - _minimumVelocity);
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
            _chargeMeter.fillAmount = 0.0f;
            StartCoroutine(Cooldown());
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

	private IEnumerator Cooldown()
	{
		_onCooldown = true;
		yield return new WaitForSeconds (_reloadTime);
		_onCooldown = false;
		_fireParticleSystem.Stop (true, ParticleSystemStopBehavior.StopEmittingAndClear);
	}

    public void SetMoveMode()
    {
        _chargeMeter.enabled = false;
        _igniteParticleSystemContainer.SetActive(false);
        _rotateArrow.SetActive(false);
        _moveArrowsContainer.SetActive(true);
    }

    public void SetRotateMode()
    {
        _chargeMeter.enabled = false;
        _igniteParticleSystemContainer.SetActive(false);
        _moveArrowsContainer.SetActive(false);
        _rotateArrow.SetActive(true);
    }

    public void SetFiringMode()
    {
        _rotateArrow.SetActive(false);
        _moveArrowsContainer.SetActive(false);
        _chargeMeter.enabled = true;
        _igniteParticleSystemContainer.SetActive(true);
    }
}
