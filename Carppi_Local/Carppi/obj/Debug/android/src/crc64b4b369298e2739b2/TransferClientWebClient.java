package crc64b4b369298e2739b2;


public class TransferClientWebClient
	extends android.webkit.WebViewClient
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_shouldOverrideUrlLoading:(Landroid/webkit/WebView;Ljava/lang/String;)Z:GetShouldOverrideUrlLoading_Landroid_webkit_WebView_Ljava_lang_String_Handler\n" +
			"n_onPageFinished:(Landroid/webkit/WebView;Ljava/lang/String;)V:GetOnPageFinished_Landroid_webkit_WebView_Ljava_lang_String_Handler\n" +
			"";
		mono.android.Runtime.register ("Carppi.Fragments.TransferClientWebClient, Carppi", TransferClientWebClient.class, __md_methods);
	}


	public TransferClientWebClient ()
	{
		super ();
		if (getClass () == TransferClientWebClient.class)
			mono.android.TypeManager.Activate ("Carppi.Fragments.TransferClientWebClient, Carppi", "", this, new java.lang.Object[] {  });
	}

	public TransferClientWebClient (android.webkit.WebView p0)
	{
		super ();
		if (getClass () == TransferClientWebClient.class)
			mono.android.TypeManager.Activate ("Carppi.Fragments.TransferClientWebClient, Carppi", "Android.Webkit.WebView, Mono.Android", this, new java.lang.Object[] { p0 });
	}


	public boolean shouldOverrideUrlLoading (android.webkit.WebView p0, java.lang.String p1)
	{
		return n_shouldOverrideUrlLoading (p0, p1);
	}

	private native boolean n_shouldOverrideUrlLoading (android.webkit.WebView p0, java.lang.String p1);


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
