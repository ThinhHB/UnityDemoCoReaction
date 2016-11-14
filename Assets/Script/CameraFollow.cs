#pragma warning disable 0649
using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour 
{
	#region Init, config
	[Header("Config for the smooth tracking")]
	[SerializeField] float xMargin = 1f;		// Distance in the x axis the player can move before the camera follows.
	[SerializeField] float yMargin = 1f;		// Distance in the y axis the player can move before the camera follows.
	[SerializeField] float xSmooth = 8f;		// How smoothly the camera catches up with it's target movement in the x axis.
	[SerializeField] float ySmooth = 8f;		// How smoothly the camera catches up with it's target movement in the y axis.

	[Header("Camera wont go out of this range")]
	[SerializeField] Transform limitTop;
	[SerializeField] Transform limitBotom;
	[SerializeField] Transform limitRight;
	[SerializeField] Transform limitLeft;

	[Header("The tracking target, can set it in Inspector")]
	[SerializeField] Transform target;		// Reference to the player's transform.

	Transform _myTransform;
	Camera _myCamera;

	void Awake()
	{
		_myTransform = transform;
		_myCamera = GetComponent<Camera>();

		Common.Warning(limitTop != null, "CameraControl, missing limitTop");
		Common.Warning(limitBotom != null, "CameraControl, missing limitBottom");
		Common.Warning(limitLeft != null, "CameraControl, missing limitLeft");
		Common.Warning(limitRight != null, "CameraControl, missing limitRight");

		if (limitTop == null || limitBotom == null || limitRight == null || limitLeft == null)
		{
			enabled = false;
		}
	}

	#endregion



	#region Public

	/// Set to null -> mean deactive camera
	public void SetTarget(Transform target)
	{
		if (target != null)
		{
			this.target = target;
			enabled = true;
		}
		else
		{
			this.target = null;
			enabled = false;
		}
	}

	#endregion



	#region Working

	void FixedUpdate ()
	{
		if (target != null)
			TrackPlayer();
	}


	bool CheckXMargin()
	{
		// Returns true if the distance between the camera and the player in the x axis is greater than the x margin.
		return Mathf.Abs(_myTransform.position.x - target.position.x) > xMargin;
	}


	bool CheckYMargin()
	{
		// Returns true if the distance between the camera and the player in the y axis is greater than the y margin.
		return Mathf.Abs(_myTransform.position.y - target.position.y) > yMargin;
	}


	float ClampToHorizonLimit(float posX)
	{
		var w = _myCamera.orthographicSize * _myCamera.aspect;
		return Mathf.Clamp(posX, limitLeft.position.x + w, limitRight.position.x - w);
	}


	float ClampToVerticalLimit(float posY)
	{
		var h = _myCamera.orthographicSize;
		return Mathf.Clamp(posY, limitBotom.position.y + h, limitTop.position.y - h);
	}


	void TrackPlayer ()
	{
		// By default the target x and y coordinates of the camera are it's current x and y coordinates.
		float targetX = _myTransform.position.x;
		float targetY = _myTransform.position.y;

		// If the player has moved beyond the x margin...
		if (CheckXMargin())
		{
			// ... the target x coordinate should be a Lerp between the camera's current x position and the player's current x position.
			targetX = Mathf.Lerp(_myTransform.position.x, target.position.x, xSmooth * Time.fixedDeltaTime);
		}

		// If the player has moved beyond the y margin...
		if (CheckYMargin())
		{
			// ... the target y coordinate should be a Lerp between the camera's current y position and the player's current y position.
			targetY = Mathf.Lerp(_myTransform.position.y, target.position.y, ySmooth * Time.fixedDeltaTime);
		}

		// The target x and y coordinates should not be larger than the maximum or smaller than the minimum.
//		targetX = Mathf.Clamp(targetX, limitLeft.position.x, limitRight.position.x);
//		targetY = Mathf.Clamp(targetY, limitBotom.position.y, limitTop.position.y);
		targetX = ClampToHorizonLimit(targetX);
		targetY = ClampToVerticalLimit(targetY);

		// Set the camera's position to the target position with the same z component.
		_myTransform.position = new Vector3(targetX, targetY, _myTransform.position.z);
	}

	#endregion
}
