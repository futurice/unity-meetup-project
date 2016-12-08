using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
	private CrateController[] _crates;

	private void Awake()
	{
		_crates = GetComponentsInChildren<CrateController>(true);

		for (int i = 0; i < _crates.Length; ++i)
		{
			_crates[i].TargetController = this;
		}

		ToggleGravity(false);
	}

	private void ToggleGravity(bool enabled)
	{
		for (int i = 0; i < _crates.Length; ++i)
		{
			_crates[i].CrateRigidbody.useGravity = enabled;
		}
	}

	public void TriggerCollisionEvent()
	{
		ToggleGravity(true);
	}
}
