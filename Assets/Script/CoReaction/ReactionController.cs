using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace CoReaction
{
	public class ReactionController : MonoBehaviour
	{
		#region InitConfig

		List<BaseReaction> _reactionList = new List<BaseReaction>();
		Dictionary<int, BaseParameter> _parameterDictionary = new Dictionary<int, BaseParameter>();

		#endregion


		#region Publics - add parameters, reactions

		public void AddParameter(int parameterID, ParameterType paramType, object initValue = null)
		{
			if (_parameterDictionary.ContainsKey(parameterID))
			{
				Common.Warning(false, "Coreaction, duplicate parameterID : " + parameterID);
				return;
			}

			BaseParameter parameter = null;
			switch (paramType)
			{
				case ParameterType.Boolean:
					parameter = new BooleanParameter();
					break;

				case ParameterType.Trigger:
					parameter = new TriggerParameter();
					break;

				case ParameterType.Integer:
					parameter = new IntegerParameter();
					break;

				case ParameterType.Float:
					parameter = new FloatParameter();
					break;
			}

			if (parameter == null)
			{
				Common.Warning(false, "Not found parameterTYpe : " + paramType);
				return;
			}

			parameter.SetValue(initValue);
			_parameterDictionary.Add(parameterID, parameter);
		}


		public void AddReaction (BaseReaction reaction)
		{
			if (reaction == null)
			{
				Common.Warning(false, "Null reaction !!!");
				return;
			}

			// create CompareFunc for reaction
			var conditionList = reaction.conditionList;
			for (int m = 0; m < conditionList.Count; m++)
			{
				var condition = conditionList[m];
				var parameter = _parameterDictionary[condition.parameterID];

				if (parameter == null)
				{
					Common.Warning(false, "AddReaction, Not found parameterID : " + condition.parameterID);
					continue;
				}

				// set the compareFunc for reaction's condition
				condition.compareFunc = parameter.IsPass;
			}

			// final, add it to our reactionList
			_reactionList.Add(reaction);
		}

		#endregion


		#region Publics - interact with parameter

		public void SetBool(int parameterID, bool value)
		{
			SetParameterValue(parameterID, value);
		}

		public void SetTrigger(int parameterID)
		{
			SetParameterValue(parameterID, true);
		}

		public void SetInt(int parameterID, int value)
		{
			SetParameterValue(parameterID, value);
		}

		public void SetFloat(int parameterID, float value)
		{
			SetParameterValue(parameterID, value);
		}

		#endregion


		#region Private

		/// Dirty bit, Is any parameter just changed value
		bool _isAnyParameterDirty = false;
		/// Dirty bit, is any Trigger parameter just changed
		bool _isAnyTriggerParameterDirty = false;

		void SetParameterValue(int parameterID, object value)
		{
			if (! _parameterDictionary.ContainsKey(parameterID))
			{
				Common.Warning(false, "SetParameter, not found parameterID = " + parameterID);
				return;
			}

			var parameter = _parameterDictionary[parameterID];
			if (parameter.IsValueEqual(value))
			{
				// new value is the same, then do nothing
				return;
			}

			// change value and also set dirty flag here
			parameter.SetValue(value);
			_isAnyParameterDirty = true;
			if (parameter.paraType == ParameterType.Trigger)
			{
				_isAnyTriggerParameterDirty = true;
			}
		}


		void LateUpdate ()
		{
			// checking active state for reaction first
			if (_isAnyParameterDirty)
			{
				CheckActiveStateForReactions();
				_isAnyParameterDirty = false;
			}

			// run them, after checking active
			RunReactions();

			// Trigger parameter need to be reset after first use,
			// reset them here, and re-check active state for reaction again
			if (_isAnyTriggerParameterDirty)
			{
				ResetAllTriggerPamameter();
				CheckActiveStateForReactions();
				_isAnyTriggerParameterDirty = false;
			}
		}


		void RunReactions()
		{
			for (int m = 0; m < _reactionList.Count; m++)
			{
				_reactionList[m].Run();
			}
		}


		void CheckActiveStateForReactions()
		{
			for (int m = 0; m < _reactionList.Count; m++)
			{
				_reactionList[m].CheckActiveState();
			}
		}


		void ResetAllTriggerPamameter()
		{
			foreach (var item in _parameterDictionary)
			{
				var param = item.Value;
				if (param.paraType == ParameterType.Trigger)
				{
					param.SetValue(false);
				}
			}
		}

		#endregion
	}
}