package crc64b4b369298e2739b2;


public class GroceryRequestWebClient
	extends android.webkit.WebViewClient
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onPageFinished:(Landroid/webkit/WebView;Ljava/lang/String;)V:GetOnPageFinished_Landroid_webkit_WebView_Ljava_lang_String_Handler\n" +
			"";
		mono.android.Runtime.register ("Carppi.Fragments.GroceryRequestWebClient, Carppi", GroceryRequestWebClient.class, __md_methods);
	}


	public GroceryRequestWebClient ()
	{
		super ();
		if (getClass () == GroceryRequestWebClient.class)
			mono.android.TypeManager.Activate ("Carppi.Fragments.GroceryRequestWebClient, Carppi", "", this, new java.lang.Object[] {  });
	}

	public GroceryRequestWebClient (android.content.Context p0)
	{
		super ();
		if (getClass () == GroceryRequestWebClient.class)
			mono.android.TypeManager.Activate ("Carppi.Fragments.GroceryRequestWebClient, Carppi", "Android.Content.Context, Mono.Android", this, new java.lang.Object[] { p0 });
	}


	public void onPageFinished (android.webkit.WebView p0, java.lang.String p1)
	{
		n_onPageFinished (p0, p1);
	}

	private native void n_onPageFinished (android.webkit.WebView p0, java.lang.String p1);

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
