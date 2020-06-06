package crc64b4b369298e2739b2;


public class RestaurantOrders
	extends android.webkit.WebViewClient
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_ShowExtraOptionsOnOrderAwait:(I)V:__export__\n" +
			"n_AcceptedOrderOptions:(J)V:__export__\n" +
			"";
		mono.android.Runtime.register ("Carppi.Fragments.RestaurantOrders, Carppi", RestaurantOrders.class, __md_methods);
	}


	public RestaurantOrders ()
	{
		super ();
		if (getClass () == RestaurantOrders.class)
			mono.android.TypeManager.Activate ("Carppi.Fragments.RestaurantOrders, Carppi", "", this, new java.lang.Object[] {  });
	}

	public RestaurantOrders (android.content.Context p0, android.webkit.WebView p1)
	{
		super ();
		if (getClass () == RestaurantOrders.class)
			mono.android.TypeManager.Activate ("Carppi.Fragments.RestaurantOrders, Carppi", "Android.Content.Context, Mono.Android:Android.Webkit.WebView, Mono.Android", this, new java.lang.Object[] { p0, p1 });
	}

	@android.webkit.JavascriptInterface

	public static void ShowExtraOptionsOnOrderAwait (int p0)
	{
		n_ShowExtraOptionsOnOrderAwait (p0);
	}

	private static native void n_ShowExtraOptionsOnOrderAwait (int p0);

	@android.webkit.JavascriptInterface

	public void AcceptedOrderOptions (long p0)
	{
		n_AcceptedOrderOptions (p0);
	}

	private native void n_AcceptedOrderOptions (long p0);

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
