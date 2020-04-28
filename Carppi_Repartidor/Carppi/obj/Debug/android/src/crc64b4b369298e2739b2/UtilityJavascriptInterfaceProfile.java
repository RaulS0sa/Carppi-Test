package crc64b4b369298e2739b2;


public class UtilityJavascriptInterfaceProfile
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_UpdateConectedIDStripe:(Ljava/lang/String;)V:__export__\n" +
			"";
		mono.android.Runtime.register ("Carppi.Fragments.UtilityJavascriptInterfaceProfile, Carppi", UtilityJavascriptInterfaceProfile.class, __md_methods);
	}


	public UtilityJavascriptInterfaceProfile ()
	{
		super ();
		if (getClass () == UtilityJavascriptInterfaceProfile.class)
			mono.android.TypeManager.Activate ("Carppi.Fragments.UtilityJavascriptInterfaceProfile, Carppi", "", this, new java.lang.Object[] {  });
	}

	public UtilityJavascriptInterfaceProfile (android.content.Context p0, android.webkit.WebView p1)
	{
		super ();
		if (getClass () == UtilityJavascriptInterfaceProfile.class)
			mono.android.TypeManager.Activate ("Carppi.Fragments.UtilityJavascriptInterfaceProfile, Carppi", "Android.Content.Context, Mono.Android:Android.Webkit.WebView, Mono.Android", this, new java.lang.Object[] { p0, p1 });
	}

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
