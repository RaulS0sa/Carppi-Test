package crc64b4b369298e2739b2;


public class WebInterfaceGroceryRequest
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_DisplayOptionsToGroceryBuyOrder:(I)V:__export__\n" +
			"";
		mono.android.Runtime.register ("Carppi.Fragments.WebInterfaceGroceryRequest, Carppi", WebInterfaceGroceryRequest.class, __md_methods);
	}


	public WebInterfaceGroceryRequest ()
	{
		super ();
		if (getClass () == WebInterfaceGroceryRequest.class)
			mono.android.TypeManager.Activate ("Carppi.Fragments.WebInterfaceGroceryRequest, Carppi", "", this, new java.lang.Object[] {  });
	}

	public WebInterfaceGroceryRequest (android.app.Activity p0, android.webkit.WebView p1)
	{
		super ();
		if (getClass () == WebInterfaceGroceryRequest.class)
			mono.android.TypeManager.Activate ("Carppi.Fragments.WebInterfaceGroceryRequest, Carppi", "Android.App.Activity, Mono.Android:Android.Webkit.WebView, Mono.Android", this, new java.lang.Object[] { p0, p1 });
	}

	@android.webkit.JavascriptInterface

	public void DisplayOptionsToGroceryBuyOrder (int p0)
	{
		n_DisplayOptionsToGroceryBuyOrder (p0);
	}

	private native void n_DisplayOptionsToGroceryBuyOrder (int p0);

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
