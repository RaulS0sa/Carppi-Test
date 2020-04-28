package crc64b4b369298e2739b2;


public class FragmentMain_UtilityJavascriptInterface
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_GetAllMessagesFromConversation:(J)V:__export__\n" +
			"n_DriverIsCancelingTrip:(J)V:__export__\n" +
			"n_SendMessageInRideshare:(Ljava/lang/String;J)V:__export__\n" +
			"n_DismissPaymentModal:()V:__export__\n" +
			"n_SSToast:(I)V:__export__\n" +
			"n_LookForRequestOfTrip:(I)V:__export__\n" +
			"n_SwitchToTripState:()V:__export__\n" +
			"n_SolicitarViaje:(I)V:__export__\n" +
			"n_RateUser:(I)V:__export__\n" +
			"n_DissmissBottomModal:()V:__export__\n" +
			"n_Look_For_Ride_AidMethod:(Ljava/lang/String;)V:__export__\n" +
			"n_SearchCarpoolTrip:()V:__export__\n" +
			"n_SearchGirlTrip:()V:__export__\n" +
			"n_SearchRegularTrip:()V:__export__\n" +
			"";
		mono.android.Runtime.register ("Carppi.Fragments.FragmentMain+UtilityJavascriptInterface, Carppi", FragmentMain_UtilityJavascriptInterface.class, __md_methods);
	}


	public FragmentMain_UtilityJavascriptInterface ()
	{
		super ();
		if (getClass () == FragmentMain_UtilityJavascriptInterface.class)
			mono.android.TypeManager.Activate ("Carppi.Fragments.FragmentMain+UtilityJavascriptInterface, Carppi", "", this, new java.lang.Object[] {  });
	}

	public FragmentMain_UtilityJavascriptInterface (android.content.Context p0, android.webkit.WebView p1)
	{
		super ();
		if (getClass () == FragmentMain_UtilityJavascriptInterface.class)
			mono.android.TypeManager.Activate ("Carppi.Fragments.FragmentMain+UtilityJavascriptInterface, Carppi", "Android.Content.Context, Mono.Android:Android.Webkit.WebView, Mono.Android", this, new java.lang.Object[] { p0, p1 });
	}

	@android.webkit.JavascriptInterface

	public void GetAllMessagesFromConversation (long p0)
	{
		n_GetAllMessagesFromConversation (p0);
	}

	private native void n_GetAllMessagesFromConversation (long p0);

	@android.webkit.JavascriptInterface

	public void DriverIsCancelingTrip (long p0)
	{
		n_DriverIsCancelingTrip (p0);
	}

	private native void n_DriverIsCancelingTrip (long p0);

	@android.webkit.JavascriptInterface

	public void SendMessageInRideshare (java.lang.String p0, long p1)
	{
		n_SendMessageInRideshare (p0, p1);
	}

	private native void n_SendMessageInRideshare (java.lang.String p0, long p1);

	@android.webkit.JavascriptInterface

	public void DismissPaymentModal ()
	{
		n_DismissPaymentModal ();
	}

	private native void n_DismissPaymentModal ();

	@android.webkit.JavascriptInterface

	public void SSToast (int p0)
	{
		n_SSToast (p0);
	}

	private native void n_SSToast (int p0);

	@android.webkit.JavascriptInterface

	public void LookForRequestOfTrip (int p0)
	{
		n_LookForRequestOfTrip (p0);
	}

	private native void n_LookForRequestOfTrip (int p0);

	@android.webkit.JavascriptInterface

	public void SwitchToTripState ()
	{
		n_SwitchToTripState ();
	}

	private native void n_SwitchToTripState ();

	@android.webkit.JavascriptInterface

	public void SolicitarViaje (int p0)
	{
		n_SolicitarViaje (p0);
	}

	private native void n_SolicitarViaje (int p0);

	@android.webkit.JavascriptInterface

	public void RateUser (int p0)
	{
		n_RateUser (p0);
	}

	private native void n_RateUser (int p0);

	@android.webkit.JavascriptInterface

	public void DissmissBottomModal ()
	{
		n_DissmissBottomModal ();
	}

	private native void n_DissmissBottomModal ();

	@android.webkit.JavascriptInterface

	public void Look_For_Ride_AidMethod (java.lang.String p0)
	{
		n_Look_For_Ride_AidMethod (p0);
	}

	private native void n_Look_For_Ride_AidMethod (java.lang.String p0);

	@android.webkit.JavascriptInterface

	public void SearchCarpoolTrip ()
	{
		n_SearchCarpoolTrip ();
	}

	private native void n_SearchCarpoolTrip ();

	@android.webkit.JavascriptInterface

	public void SearchGirlTrip ()
	{
		n_SearchGirlTrip ();
	}

	private native void n_SearchGirlTrip ();

	@android.webkit.JavascriptInterface

	public void SearchRegularTrip ()
	{
		n_SearchRegularTrip ();
	}

	private native void n_SearchRegularTrip ();

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
