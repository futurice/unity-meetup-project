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
	[SerializeField]
	private ParticleSystem _fireParticleSystem = null;
	[SerializeField]
	private GameObject _igniteParticleSystemContainer = null;
	[SerializeField]
	private AudioSource _fireSound = null;

	private bool _onCooldown = false;

    public void Aim(float pitch, float yaw)
    {
        if (_aimingTransform != null)
        {
            _aimingTransform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
        }
    }

    public void Fire()
    {
        if (_firePoint != null && _cannonballPrefab != null && !_onCooldown)
        {
            GameObject ball = Instantiate(_cannonballPrefab, _firePoint.position, _firePoint.rotation);
            Rigidbody rb = ball.GetComponent<Rigidbody>();
            rb.velocity = _firePoint.forward.normalized * _ballVelocity;
			_fireParticleSystem.Emit(1);
			_fireSound.Play();

			// Cooldown
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
