package crc64b4b369298e2739b2;


public class MenuWebView
	extends android.webkit.WebViewClient
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_ShowExtraOptionsOnOrderAwait:(I)V:__export__\n" +
			"n_DisplayOptionsOfProduct:(J)V:__export__\n" +
			"n_RestaurantOpenOptions:()V:__export__\n" +
			"";
		mono.android.Runtime.register ("Carppi.Fragments.MenuWebView, Carppi", MenuWebView.class, __md_methods);
	}


	public MenuWebView ()
	{
		super ();
		if (getClass () == MenuWebView.class)
			mono.android.TypeManager.Activate ("Carppi.Fragments.MenuWebView, Carppi", "", this, new java.lang.Object[] {  });
	}

	public MenuWebView (android.content.Context p0, android.webkit.WebView p1)
	{
		super ();
		if (getClass () == MenuWebView.class)
			mono.android.TypeManager.Activate ("Carppi.Fragments.MenuWebView, Carppi", "Android.Content.Context, Mono.Android:Android.Webkit.WebView, Mono.Android", this, new java.lang.Object[] { p0, p1 });
	}

	@android.webkit.JavascriptInterface

	public static void ShowExtraOptionsOnOrderAwait (int p0)
	{
		n_ShowExtraOptionsOnOrderAwait (p0);
	}

	private static native void n_ShowExtraOptionsOnOrderAwait (int p0);

	@android.webkit.JavascriptInterface

	public void DisplayOptionsOfProduct (long p0)
	{
		n_DisplayOptionsOfProduct (p0);
	}

	private native void n_DisplayOptionsOfProduct (long p0);

	@android.webkit.JavascriptInterface

	public void RestaurantOpenOptions ()
	{
		n_RestaurantOpenOptions ();
	}

	private native void n_RestaurantOpenOptions ();

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
