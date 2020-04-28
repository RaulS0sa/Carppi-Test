package crc64b4b369298e2739b2;


public class FragmentGrocery_GroceryWebClient
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
		mono.android.Runtime.register ("Carppi.Fragments.FragmentGrocery+GroceryWebClient, Carppi", FragmentGrocery_GroceryWebClient.class, __md_methods);
	}


	public FragmentGrocery_GroceryWebClient ()
	{
		super ();
		if (getClass () == FragmentGrocery_GroceryWebClient.class)
			mono.android.TypeManager.Activate ("Carppi.Fragments.FragmentGrocery+GroceryWebClient, Carppi", "", this, new java.lang.Object[] {  });
	}

	public FragmentGrocery_GroceryWebClient (android.content.Context p0, android.content.res.Resources p1)
	{
		super ();
		if (getClass () == FragmentGrocery_GroceryWebClient.class)
			mono.android.TypeManager.Activate ("Carppi.Fragments.FragmentGrocery+GroceryWebClient, Carppi", "Android.Content.Context, Mono.Android:Android.Content.Res.Resources, Mono.Android", this, new java.lang.Object[] { p0, p1 });
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
