package crc64b4b369298e2739b2;


public class UtilityJavascriptInterfacePaymentModal
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_DismissPaymentModal:()V:__export__\n" +
			"";
		mono.android.Runtime.register ("Carppi.Fragments.UtilityJavascriptInterfacePaymentModal, Carppi", UtilityJavascriptInterfacePaymentModal.class, __md_methods);
	}


	public UtilityJavascriptInterfacePaymentModal ()
	{
		super ();
		if (getClass () == UtilityJavascriptInterfacePaymentModal.class)
			mono.android.TypeManager.Activate ("Carppi.Fragments.UtilityJavascriptInterfacePaymentModal, Carppi", "", this, new java.lang.Object[] {  });
	}

	public UtilityJavascriptInterfacePaymentModal (android.app.Activity p0, android.webkit.WebView p1, android.app.Dialog p2)
	{
		super ();
		if (getClass () == UtilityJavascriptInterfacePaymentModal.class)
			mono.android.TypeManager.Activate ("Carppi.Fragments.UtilityJavascriptInterfacePaymentModal, Carppi", "Android.App.Activity, Mono.Android:Android.Webkit.WebView, Mono.Android:Android.App.Dialog&, Mono.Android", this, new java.lang.Object[] { p0, p1, p2 });
	}

	@android.webkit.JavascriptInterface

	public void DismissPaymentModal ()
	{
		n_DismissPaymentModal ();
	}

	private native void n_DismissPaymentModal ();

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
