package crc64b4b369298e2739b2;


public class WebInterfaceFragmentGrocery
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_ShowExtraOptionsOnGroceryAwait:()V:__export__\n" +
			"n_UpdateGroceryMapState:()V:__export__\n" +
			"n_UpdateProductList:(Ljava/lang/String;I)V:__export__\n" +
			"n_DisplayShopingKart:()V:__export__\n" +
			"";
		mono.android.Runtime.register ("Carppi.Fragments.WebInterfaceFragmentGrocery, Carppi", WebInterfaceFragmentGrocery.class, __md_methods);
	}


	public WebInterfaceFragmentGrocery ()
	{
		super ();
		if (getClass () == WebInterfaceFragmentGrocery.class)
			mono.android.TypeManager.Activate ("Carppi.Fragments.WebInterfaceFragmentGrocery, Carppi", "", this, new java.lang.Object[] {  });
	}

	public WebInterfaceFragmentGrocery (android.app.Activity p0, android.webkit.WebView p1)
	{
		super ();
		if (getClass () == WebInterfaceFragmentGrocery.class)
			mono.android.TypeManager.Activate ("Carppi.Fragments.WebInterfaceFragmentGrocery, Carppi", "Android.App.Activity, Mono.Android:Android.Webkit.WebView, Mono.Android", this, new java.lang.Object[] { p0, p1 });
	}

	@android.webkit.JavascriptInterface

	public static void ShowExtraOptionsOnGroceryAwait ()
	{
		n_ShowExtraOptionsOnGroceryAwait ();
	}

	private static native void n_ShowExtraOptionsOnGroceryAwait ();

	@android.webkit.JavascriptInterface

	public static void UpdateGroceryMapState ()
	{
		n_UpdateGroceryMapState ();
	}

	private static native void n_UpdateGroceryMapState ();

	@android.webkit.JavascriptInterface

	public static void UpdateProductList (java.lang.String p0, int p1)
	{
		n_UpdateProductList (p0, p1);
	}

	private static native void n_UpdateProductList (java.lang.String p0, int p1);

	@android.webkit.JavascriptInterface

	public static void DisplayShopingKart ()
	{
		n_DisplayShopingKart ();
	}

	private static native void n_DisplayShopingKart ();

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
