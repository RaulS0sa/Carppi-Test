package crc64b4b369298e2739b2;


public class FragmentMain_WebInterfaceMenuCarppi
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_ShowExtraOptionsOnDriverSearchingForPassenger:()V:__export__\n" +
			"n_OpenDrawer:()V:__export__\n" +
			"n_ShowExtraOptionsOnPassengerWaitToDriver:()V:__export__\n" +
			"n_OptiosDuringTripPickUp:()V:__export__\n" +
			"n_ShowNonActiveOptionsToDrivers:()V:__export__\n" +
			"n_ShowOptionsToDrivers:()V:__export__\n" +
			"n_StartPendingTrip:()V:__export__\n" +
			"n_FinishPendingTrip:()V:__export__\n" +
			"n_DisplayAceptRejectTripModal:()V:__export__\n" +
			"n_DisplayTripTypeSelector:(DD)V:__export__\n" +
			"n_DisplayDestinySearchBottomModal:()V:__export__\n" +
			"n_DissmissBottomModal:()V:__export__\n" +
			"n_Look_For_Ride:(DDDDIII)V:__export__\n" +
			"n_SwitchSearchProgressBarState:()V:__export__\n" +
			"n_DisplayDestinySelector:()V:__export__\n" +
			"n_SetStateOfRideShareRequest:(I)V:__export__\n" +
			"";
		mono.android.Runtime.register ("Carppi.Fragments.FragmentMain+WebInterfaceMenuCarppi, Carppi", FragmentMain_WebInterfaceMenuCarppi.class, __md_methods);
	}


	public FragmentMain_WebInterfaceMenuCarppi ()
	{
		super ();
		if (getClass () == FragmentMain_WebInterfaceMenuCarppi.class)
			mono.android.TypeManager.Activate ("Carppi.Fragments.FragmentMain+WebInterfaceMenuCarppi, Carppi", "", this, new java.lang.Object[] {  });
	}

	public FragmentMain_WebInterfaceMenuCarppi (android.app.Activity p0, android.webkit.WebView p1)
	{
		super ();
		if (getClass () == FragmentMain_WebInterfaceMenuCarppi.class)
			mono.android.TypeManager.Activate ("Carppi.Fragments.FragmentMain+WebInterfaceMenuCarppi, Carppi", "Android.App.Activity, Mono.Android:Android.Webkit.WebView, Mono.Android", this, new java.lang.Object[] { p0, p1 });
	}

	@android.webkit.JavascriptInterface

	public void ShowExtraOptionsOnDriverSearchingForPassenger ()
	{
		n_ShowExtraOptionsOnDriverSearchingForPassenger ();
	}

	private native void n_ShowExtraOptionsOnDriverSearchingForPassenger ();

	@android.webkit.JavascriptInterface

	public void OpenDrawer ()
	{
		n_OpenDrawer ();
	}

	private native void n_OpenDrawer ();

	@android.webkit.JavascriptInterface

	public void ShowExtraOptionsOnPassengerWaitToDriver ()
	{
		n_ShowExtraOptionsOnPassengerWaitToDriver ();
	}

	private native void n_ShowExtraOptionsOnPassengerWaitToDriver ();

	@android.webkit.JavascriptInterface

	public void OptiosDuringTripPickUp ()
	{
		n_OptiosDuringTripPickUp ();
	}

	private native void n_OptiosDuringTripPickUp ();

	@android.webkit.JavascriptInterface

	public void ShowNonActiveOptionsToDrivers ()
	{
		n_ShowNonActiveOptionsToDrivers ();
	}

	private native void n_ShowNonActiveOptionsToDrivers ();

	@android.webkit.JavascriptInterface

	public void ShowOptionsToDrivers ()
	{
		n_ShowOptionsToDrivers ();
	}

	private native void n_ShowOptionsToDrivers ();

	@android.webkit.JavascriptInterface

	public static void StartPendingTrip ()
	{
		n_StartPendingTrip ();
	}

	private static native void n_StartPendingTrip ();

	@android.webkit.JavascriptInterface

	public static void FinishPendingTrip ()
	{
		n_FinishPendingTrip ();
	}

	private static native void n_FinishPendingTrip ();

	@android.webkit.JavascriptInterface

	public static void DisplayAceptRejectTripModal ()
	{
		n_DisplayAceptRejectTripModal ();
	}

	private static native void n_DisplayAceptRejectTripModal ();

	@android.webkit.JavascriptInterface

	public void DisplayTripTypeSelector (double p0, double p1)
	{
		n_DisplayTripTypeSelector (p0, p1);
	}

	private native void n_DisplayTripTypeSelector (double p0, double p1);

	@android.webkit.JavascriptInterface

	public void DisplayDestinySearchBottomModal ()
	{
		n_DisplayDestinySearchBottomModal ();
	}

	private native void n_DisplayDestinySearchBottomModal ();

	@android.webkit.JavascriptInterface

	public void DissmissBottomModal ()
	{
		n_DissmissBottomModal ();
	}

	private native void n_DissmissBottomModal ();

	@android.webkit.JavascriptInterface

	public void Look_For_Ride (double p0, double p1, double p2, double p3, int p4, int p5, int p6)
	{
		n_Look_For_Ride (p0, p1, p2, p3, p4, p5, p6);
	}

	private native void n_Look_For_Ride (double p0, double p1, double p2, double p3, int p4, int p5, int p6);

	@android.webkit.JavascriptInterface

	public void SwitchSearchProgressBarState ()
	{
		n_SwitchSearchProgressBarState ();
	}

	private native void n_SwitchSearchProgressBarState ();

	@android.webkit.JavascriptInterface

	public void DisplayDestinySelector ()
	{
		n_DisplayDestinySelector ();
	}

	private native void n_DisplayDestinySelector ();

	@android.webkit.JavascriptInterface

	public void SetStateOfRideShareRequest (int p0)
	{
		n_SetStateOfRideShareRequest (p0);
	}

	private native void n_SetStateOfRideShareRequest (int p0);

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
