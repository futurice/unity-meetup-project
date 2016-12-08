using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateController : MonoBehaviour
{
	private TargetController 	_targetController	= null;
	private Rigidbody 			_rigidbody 			= null;

	public TargetController TargetController
	{
		set
		{
			_targetController = value;
		}
	}

	public Rigidbody CrateRigidbody
	{
		get
		{
			if (_rigidbody == null)
			{
				_rigidbody = GetComponent<Rigidbody>();
			}

			return _rigidbody;
		}
	}

	private void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.CompareTag("cannonball"))
		{
			_targetController.TriggerCollisionEvent();
		}
	}
}
