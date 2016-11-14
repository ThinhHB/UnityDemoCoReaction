
namespace CoReaction
{
	public class ConditionForm
	{
		/// Coreaction will need this to create the compareFunc
		public int parameterID;
		/// Will be create from CoReaction when add new reaction to it
		public CheckingPass compareFunc;
		/// Will use when checking the active state
		public CompareType compareType;
		/// Will use when checking the active state
		public object compareValue;
	}

	/// A custom delegate to use with parameter classes
	public delegate bool CheckingPass(CompareType compareType, object value);
}