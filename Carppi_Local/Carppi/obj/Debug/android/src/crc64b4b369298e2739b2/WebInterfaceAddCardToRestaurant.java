package crc64b4b369298e2739b2;


public class WebInterfaceAddCardToRestaurant
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_TransferrMyMoney:()V:__export__\n" +
			"n_QueryChainedNumber:(Ljava/lang/String;)V:__export__\n" +
			"n_ChangeToStipeView:()V:__export__\n" +
			"n_UpdateConectedIDStripe:(Ljava/lang/String;)V:__export__\n" +
			"";
		mono.android.Runtime.register ("Carppi.Fragments.WebInterfaceAddCardToRestaurant, Carppi", WebInterfaceAddCardToRestaurant.class, __md_methods);
	}


	public WebInterfaceAddCardToRestaurant ()
	{
		super ();
		if (getClass () == WebInterfaceAddCardToRestaurant.class)
			mono.android.TypeManager.Activate ("Carppi.Fragments.WebInterfaceAddCardToRestaurant, Carppi", "", this, new java.lang.Object[] {  });
	}

	public WebInterfaceAddCardToRestaurant (android.app.Activity p0, android.webkit.WebView p1)
	{
		super ();
		if (getClass () == WebInterfaceAddCardToRestaurant.class)
			mono.android.TypeManager.Activate ("Carppi.Fragments.WebInterfaceAddCardToRestaurant, Carppi", "Android.App.Activity, Mono.Android:Android.Webkit.WebView, Mono.Android", this, new java.lang.Object[] { p0, p1 });
	}

	@android.webkit.JavascriptInterface

	public void TransferrMyMoney ()
	{
		n_TransferrMyMoney ();
	}

	private native void n_TransferrMyMoney ();

	@android.webkit.JavascriptInterface

	public void QueryChainedNumber (java.lang.String p0)
	{
		n_QueryChainedNumber (p0);
	}

	private native void n_QueryChainedNumber (java.lang.String p0);

	@android.webkit.JavascriptInterface

	public void ChangeToStipeView ()
	{
		n_ChangeToStipeView ();
	}

	private native void n_ChangeToStipeView ();

	@android.webkit.JavascriptInterface

	public void UpdateConectedIDStripe (java.lang.String p0)
	{
		n_UpdateConectedIDStripe (p0);
	}

	private native void n_UpdateConectedIDStripe (java.lang.String p0);

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
