package crc64b4b369298e2739b2;


public class FragmentRepartidorView_FragmentRepartidoWebClient
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
		mono.android.Runtime.register ("Carppi.Fragments.FragmentRepartidorView+FragmentRepartidoWebClient, Carppi", FragmentRepartidorView_FragmentRepartidoWebClient.class, __md_methods);
	}


	public FragmentRepartidorView_FragmentRepartidoWebClient ()
	{
		super ();
		if (getClass () == FragmentRepartidorView_FragmentRepartidoWebClient.class)
			mono.android.TypeManager.Activate ("Carppi.Fragments.FragmentRepartidorView+FragmentRepartidoWebClient, Carppi", "", this, new java.lang.Object[] {  });
	}

	public FragmentRepartidorView_FragmentRepartidoWebClient (android.view.View p0, android.content.Context p1)
	{
		super ();
		if (getClass () == FragmentRepartidorView_FragmentRepartidoWebClient.class)
			mono.android.TypeManager.Activate ("Carppi.Fragments.FragmentRepartidorView+FragmentRepartidoWebClient, Carppi", "Android.Views.View, Mono.Android:Android.Content.Context, Mono.Android", this, new java.lang.Object[] { p0, p1 });
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
