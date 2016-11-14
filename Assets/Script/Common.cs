#if UNITY_EDITOR
	#define USE_LOG
	#define ASSERT
#endif
using UnityEngine;
using System.Collections;
using System.Diagnostics;
using Debug = UnityEngine.Debug;


public class Common
{
	//-----------------------------------
	//--------------------- Log , warning, 
	
	[Conditional("USE_LOG")]
	public static void Log(object message)
	{
		Debug.Log(message);
	}
	
	[Conditional("USE_LOG")]
	public static void Log(string format, params object[] args)
	{
		Debug.Log(string.Format(format, args));
	}

	
	[Conditional("USE_LOG")]
	public static void LogWarning(object message, Object context)
	{
		Debug.LogWarning(message, context);
	}
	
	[Conditional("USE_LOG")]
	public static void LogWarning(Object context, string format, params object[] args)
	{
		Debug.LogWarning(string.Format(format, args), context);
	}
	
	
	[Conditional("USE_LOG")]
	public static void Warning(bool condition, object message)
	{
		if ( ! condition) Debug.LogWarning(message);
	}
	
	[Conditional("USE_LOG")]
	public static void Warning(bool condition, object message, Object context)
	{
		if ( ! condition) Debug.LogWarning(message, context);
	}
}