using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class OnGroundChecking : MonoBehaviour
{
	#region Init

	[SerializeField] Transform footTrans = null;
	[SerializeField] float checkingDistance = 0;
	[SerializeField] LayerMask groundLayerMask = 0;

	void OnValidate()
	{
		Common.Warning(footTrans != null, "Missing footTrans", gameObject);
	}

	void Awake()
	{
		if (footTrans == null)
			SetActive(false);
	}

	#endregion



	#region Public

	public void SetActive(bool active)
	{
		// disable Update() -> mean no more events will be called
		enabled = active;
	}

	#endregion


	#region Events

	public UnityEventBoolean ReceiveOnGroundStatus;
	void InvokeReceiveOnGroundStatus(bool isOnGround)
	{
		ReceiveOnGroundStatus.Invoke(isOnGround);
	}

	#endregion


	#region Private

	bool _isOnGround;

	void Update()
	{
		var fromPos = footTrans.position;
		var toPos = fromPos;
		toPos.y -= checkingDistance;

		var hit = Physics2D.Linecast(fromPos, toPos, groundLayerMask);
		_isOnGround = hit.collider != null;
		InvokeReceiveOnGroundStatus(_isOnGround);
	}

	#endregion


	#region Custom classes

	[System.Serializable]
	public class UnityEventBoolean : UnityEvent<bool>
	{
	}

	#endregion
}