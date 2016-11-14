using UnityEngine;
using System.Collections;
using CoReaction;

public class MovingController : MonoBehaviour
{
	#region Init, config

	[SerializeField] Rigidbody2D controlRigidbody = null;
	[SerializeField] float jumpSpeed = 5f;
	[SerializeField] float movingLimitSpeedX = 5f;
	[SerializeField] float movingAccelerate = 5f;

	// fields
	ReactionController _reactionCtrl;

	void Awake()
	{
		CreateCoreation();
	}

	void CreateCoreation()
	{
		_reactionCtrl = gameObject.AddComponent<ReactionController>();
		// add parameter
		AddParameterToReactionController();
		// add reactions
		AddJumReaction();
		AddMoveLeftReaction();
		AddMoveRightReaction();
		AddRevertBodyToLeftReaction();
		AddRevertBodyToRightReaction();
	}

	void AddParameterToReactionController()
	{
		_reactionCtrl.AddParameter(ParameterID.PressJumpKey, ParameterType.Trigger);
		_reactionCtrl.AddParameter(ParameterID.PressLeftKey, ParameterType.Trigger);
		_reactionCtrl.AddParameter(ParameterID.PressRightKey, ParameterType.Trigger);

		_reactionCtrl.AddParameter(ParameterID.OnGroundState, ParameterType.Boolean, false);
		_reactionCtrl.AddParameter(ParameterID.HoldLeftKey, ParameterType.Boolean, false);
		_reactionCtrl.AddParameter(ParameterID.HoldRightKey, ParameterType.Boolean, false);

		_reactionCtrl.AddParameter(ParameterID.VeloX, ParameterType.Float, 0f);
	}

	void AddJumReaction()
	{
		var jump = new BaseReaction();
		jump.AddCondition(ParameterID.PressJumpKey, CompareType.Trigger, null);
		jump.AddCondition(ParameterID.OnGroundState, CompareType.BooleanTrue);
		jump.SetAction(Jump);
		_reactionCtrl.AddReaction(jump);
	}

	void AddMoveLeftReaction()
	{
		var moveLeft = new BaseReaction();
		moveLeft.AddCondition(ParameterID.HoldLeftKey, CompareType.BooleanTrue);
		moveLeft.AddCondition(ParameterID.VeloX, CompareType.FloatGreater, -movingLimitSpeedX);
		moveLeft.SetAction(() => Move(-1f));
		_reactionCtrl.AddReaction(moveLeft);
	}

	void AddMoveRightReaction()
	{
		var moveRight = new BaseReaction();
		moveRight.AddCondition(ParameterID.HoldRightKey, CompareType.BooleanTrue);
		moveRight.AddCondition(ParameterID.VeloX, CompareType.FloatLess, movingLimitSpeedX);
		moveRight.SetAction(() => Move(1f));
		_reactionCtrl.AddReaction(moveRight);
	}

	void AddRevertBodyToLeftReaction()
	{
		var revertLeft = new BaseReaction();
		revertLeft.AddCondition(ParameterID.PressLeftKey, CompareType.Trigger);
		revertLeft.SetAction(() => RevertRender(true));
		_reactionCtrl.AddReaction(revertLeft);
	}

	void AddRevertBodyToRightReaction()
	{
		var revertRight = new BaseReaction();
		revertRight.AddCondition(ParameterID.PressRightKey, CompareType.Trigger);
		revertRight.SetAction(() => RevertRender(false));
		_reactionCtrl.AddReaction(revertRight);
	}

	#endregion


	#region Private

	void Update()
	{
		// move left
		_reactionCtrl.SetBool(ParameterID.HoldLeftKey, Input.GetKey(KeyCode.A));
		_reactionCtrl.SetBool(ParameterID.HoldRightKey, Input.GetKey(KeyCode.D));

		// press key, use for revert render
		if (Input.GetKeyDown(KeyCode.A))
		{
			_reactionCtrl.SetTrigger(ParameterID.PressLeftKey);
		}

		if (Input.GetKeyDown(KeyCode.D))
		{
			_reactionCtrl.SetTrigger(ParameterID.PressRightKey);
		}

		// jump
		if (Input.GetKeyDown(KeyCode.W))
		{
			_reactionCtrl.SetTrigger(ParameterID.PressJumpKey);
		}

		// speed
		var veloX = controlRigidbody.velocity.x;
		_reactionCtrl.SetFloat(ParameterID.VeloX, veloX);
	}


	void Jump()
	{
		var velo = controlRigidbody.velocity;
		velo.y = jumpSpeed;
		controlRigidbody.velocity = velo;
	}


	void Move(float XaxisRaw)
	{
		var velocity = controlRigidbody.velocity;
		velocity.x += movingAccelerate * XaxisRaw * Time.deltaTime;
		controlRigidbody.velocity = velocity;
	}


	void RevertRender(bool isToLeft)
	{
		var localScale = transform.localScale;
		var ratio = isToLeft ? -1f : 1f;
		localScale.x = Mathf.Abs(localScale.x) * ratio;
		transform.localScale = localScale;
	}

	#endregion


	#region Publics

	public void OnGroundStateChanged(bool isOnGround)
	{
		_reactionCtrl.SetBool(ParameterID.OnGroundState, isOnGround);
	}

	#endregion


	#region Classes

	class ParameterID
	{
		public const int OnGroundState = 0;
		public const int PressJumpKey = 1;

		public const int PressLeftKey = 2;
		public const int PressRightKey = 3;

		public const int HoldLeftKey = 4;
		public const int HoldRightKey = 5;

		public const int VeloX = 6;
	}

	#endregion
}

