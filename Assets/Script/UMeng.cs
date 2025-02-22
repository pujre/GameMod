using System.Runtime.InteropServices;

public class UMeng
{
	[DllImport("__Internal")]
	private static extern void Hello();

	[DllImport("__Internal")]
	private static extern void HelloString(string str);

	[DllImport("__Internal")]
	private static extern void UMA_Init(string _key, bool _useOpenId, bool _debug);
	[DllImport("__Internal")]
	private static extern void UMA_TrackEvent(string _event, string _param);

	public static void HellowTest()
	{
#if !UNITY_EDITOR
		Hello();
#endif
	}

	public static void Init(string _key, bool _useOpenId, bool _debug)
	{
#if !UNITY_EDITOR
		UMA_Init(_key, _useOpenId, _debug);
#endif
	}


	public static void TrackEvent(string _event, string _param)
	{
#if !UNITY_EDITOR
        UMA_TrackEvent(_event, _param);
#endif
	}

}
