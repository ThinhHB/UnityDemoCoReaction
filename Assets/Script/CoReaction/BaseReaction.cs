using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace CoReaction
{
	public class BaseReaction
	{
		#region Publics

		/// Store all condition, reaction will run when all condition was passed
		public List<ConditionForm> conditionList = new List<ConditionForm>();
		/// the action will be invoke when state was ACTIVE
		private Action _runAction = null;
		/// The state of this Reaction, default is false
		private bool _isActive = false;

		public virtual void AddCondition (int parameterID, CompareType compareType, object value = null)
		{
			// create conditionForm
			var condition = new ConditionForm();
			condition.parameterID = parameterID;
			condition.compareType = compareType;
			condition.compareValue = value;
			// push new conditionForm to the conditionList
			conditionList.Add(condition);
		}


		public virtual void SetAction(Action action)
		{
			if (action != null)
			{
				_runAction = action;
			}
			else
			{
				Common.Warning(false, "SetAction with null parameter");
			}
		}


		public virtual void SetActive(bool active)
		{
			_isActive = active;
		}

		/// If any condition are FAILED, then _isActive = false
		public void CheckActiveState()
		{
			bool isFailedAnyCondition = false;
			for (int m = 0; m < conditionList.Count; m++)
			{
				var condition = conditionList[m];
				if (! condition.compareFunc(condition.compareType, condition.compareValue))
				{
					isFailedAnyCondition = true;
					break;
				}
			}
			SetActive(! isFailedAnyCondition);
		}

		public virtual void Run ()
		{
			if (_isActive && _runAction != null)
				_runAction();
		}

		#endregion
	}
}