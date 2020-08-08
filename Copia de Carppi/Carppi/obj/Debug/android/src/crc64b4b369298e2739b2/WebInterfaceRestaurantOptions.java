package crc64b4b369298e2739b2;


public class WebInterfaceRestaurantOptions
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_RequestPermissions:()V:__export__\n" +
			"n_ShowExtraOptionsOnDeliverManAwait:()V:__export__\n" +
			"n_ShowOptionInOrderCreated:()V:__export__\n" +
			"n_ShowExtraOptionsOnGroceryAwait:()V:__export__\n" +
			"n_SearchByText:(Ljava/lang/String;)V:__export__\n" +
			"n_SearchBackAllRestaurants:()V:__export__\n" +
			"n_SearchFoodByBox:(J)V:__export__\n" +
			"n_RestaurantDetailedView:(JLjava/lang/String;Ljava/lang/String;Ljava/lang/String;Z)V:__export__\n" +
			"";
		mono.android.Runtime.register ("Carppi.Fragments.WebInterfaceRestaurantOptions, Carppi", WebInterfaceRestaurantOptions.class, __md_methods);
	}


	public WebInterfaceRestaurantOptions ()
	{
		super ();
		if (getClass () == WebInterfaceRestaurantOptions.class)
			mono.android.TypeManager.Activate ("Carppi.Fragments.WebInterfaceRestaurantOptions, Carppi", "", this, new java.lang.Object[] {  });
	}

	public WebInterfaceRestaurantOptions (android.app.Activity p0, android.webkit.WebView p1)
	{
		super ();
		if (getClass () == WebInterfaceRestaurantOptions.class)
			mono.android.TypeManager.Activate ("Carppi.Fragments.WebInterfaceRestaurantOptions, Carppi", "Android.App.Activity, Mono.Android:Android.Webkit.WebView, Mono.Android", this, new java.lang.Object[] { p0, p1 });
	}

	@android.webkit.JavascriptInterface

	public void RequestPermissions ()
	{
		n_RequestPermissions ();
	}

	private native void n_RequestPermissions ();

	@android.webkit.JavascriptInterface

	public void ShowExtraOptionsOnDeliverManAwait ()
	{
		n_ShowExtraOptionsOnDeliverManAwait ();
	}

	private native void n_ShowExtraOptionsOnDeliverManAwait ();

	@android.webkit.JavascriptInterface

	public void ShowOptionInOrderCreated ()
	{
		n_ShowOptionInOrderCreated ();
	}

	private native void n_ShowOptionInOrderCreated ();

	@android.webkit.JavascriptInterface

	public void ShowExtraOptionsOnGroceryAwait ()
	{
		n_ShowExtraOptionsOnGroceryAwait ();
	}

	private native void n_ShowExtraOptionsOnGroceryAwait ();

	@android.webkit.JavascriptInterface

	public void SearchByText (java.lang.String p0)
	{
		n_SearchByText (p0);
	}

	private native void n_SearchByText (java.lang.String p0);

	@android.webkit.JavascriptInterface

	public void SearchBackAllRestaurants ()
	{
		n_SearchBackAllRestaurants ();
	}

	private native void n_SearchBackAllRestaurants ();

	@android.webkit.JavascriptInterface

	public void SearchFoodByBox (long p0)
	{
		n_SearchFoodByBox (p0);
	}

	private native void n_SearchFoodByBox (long p0);

	@android.webkit.JavascriptInterface

	public void RestaurantDetailedView (long p0, java.lang.String p1, java.lang.String p2, java.lang.String p3, boolean p4)
	{
		n_RestaurantDetailedView (p0, p1, p2, p3, p4);
	}

	private native void n_RestaurantDetailedView (long p0, java.lang.String p1, java.lang.String p2, java.lang.String p3, boolean p4);

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
