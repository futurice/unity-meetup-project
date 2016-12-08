using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonballController : MonoBehaviour
{
	[SerializeField]
	private float _timeout = 10.0f;
	private float _startTime = 0.0f;

	private void Start()
	{
		_startTime = Time.time;
	}

	private void Update()
	{
		// Destroy the cannon ball after a specific timeout
		if (Time.time - _startTime > _timeout)
		{
			Destroy(this.gameObject);
		}
	}
}
