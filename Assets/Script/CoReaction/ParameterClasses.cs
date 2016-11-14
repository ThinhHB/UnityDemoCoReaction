using UnityEngine;
using System.Collections;

namespace CoReaction
{
	public abstract class BaseParameter
	{
		public ParameterType paraType;
		public virtual void SetValue (object value){}
		public virtual bool IsValueEqual(object value){ return false; }
		public virtual bool IsPass(CompareType compareType, object value){return false;}
	}


	//---------------------
	//--------- Trigger
	//---------------------

	public sealed class TriggerParameter : BaseParameter
	{
		bool _value = false;

		public TriggerParameter()
		{
			paraType = ParameterType.Trigger;
		}

		public override void SetValue (object value)
		{
			if (value is bool)
			{
				_value = (bool)value;
			}
			// if value = null, then default = false
			else if (value == null)
			{
				_value = false;
			}
			else
			{
				Common.LogWarning(null, "TriggerPara, must setValue with Bool, not {0}", value.GetType());
			}
		}

		public override bool IsPass (CompareType compareType, object input)
		{
			// only work with Trigger CompareType
			if (compareType == CompareType.Trigger)
			{
				// with trigger, dont care about input
				return _value;
			}
			return false;
		}

		public override bool IsValueEqual (object value)
		{
			if (value is bool)
			{
				return _value == (bool)value;
			}
			return false;
		}
	}


	//---------------------
	//--------- Boolean
	//---------------------
	public sealed class BooleanParameter : BaseParameter
	{
		bool _value = false;

		public BooleanParameter()
		{
			paraType = ParameterType.Boolean;
		}

		public override void SetValue (object value)
		{
			if (value is bool)
			{
				_value = (bool) value;
			}
			// if value = null, then default = false
			else if (value == null)
			{
				_value = false;
			}
			else
			{
				Common.LogWarning(null, "BooleanPara, must setValue with Bool, not {0}", value.GetType());
			}
		}

		public override bool IsPass (CompareType compareType, object value)
		{
			// only work with Boolean CompareType
			// With Boolean, depend on compareType, not the value
			switch(compareType)
			{
				case CompareType.BooleanTrue:
					return _value == true;

				case CompareType.BooleanFalse:
					return _value == false;
			}
			return false;
		}

		public override bool IsValueEqual (object value)
		{
			if (value is bool)
			{
				return _value == (bool)value;
			}
			return false;
		}
	}


	//---------------------
	//--------- Float
	//---------------------
	public sealed class FloatParameter : BaseParameter
	{
		float _value = 0;

		public FloatParameter()
		{
			paraType = ParameterType.Float;
		}

		public override void SetValue (object value)
		{
			// default value = 0
			if (value == null)
			{
				_value = 0f;
			}
			else
			{
				_value = (float)value;
			}
		}

		public override bool IsPass (CompareType compareType, object value)
		{
			// only work with float value
			// and Float compareType
			if (value is float)
			{
				var castInput = (float)value;
				switch(compareType)
				{
					case CompareType.FloatGreater:
						return _value > castInput;

					case CompareType.FloatLess:
						return _value < castInput;
					
					default :
						return false;
				}
			}
			else
			{
				return false;
			}
		}

		public override bool IsValueEqual (object value)
		{
			if (value is float)
			{
				return Mathf.Abs(_value - (float)value) <= float.Epsilon;
			}
			return false;
		}
	}


	//---------------------
	//--------- Integer
	//---------------------
	public sealed class IntegerParameter : BaseParameter
	{
		int _value = 0;

		public IntegerParameter()
		{
			paraType = ParameterType.Integer;
		}

		public override void SetValue (object value)
		{
			if (value == null)
			{
				_value = 0;
			}
			else
			{
				_value = (int)value;
			}
		}

		public override bool IsPass (CompareType compareType, object value)
		{
			// only work with integer value
			// and Integer compareTYpe
			if (value is int)
			{
				var castInput = (int)value;
				switch(compareType)
				{
					case CompareType.IntegerEqual:
						return _value == castInput;

					case CompareType.IntegerGreater:
						return _value > castInput;

					case CompareType.IntegerLess:
						return _value < castInput;
					
					case CompareType.IntegerNotEqual:
						return _value != castInput;

					default :
						return false;
				}
			}
			else
			{
				return false;
			}
		}

		public override bool IsValueEqual (object value)
		{
			if (value is int)
			{
				return _value == (int) value;
			}
			return false;
		}
	}
}