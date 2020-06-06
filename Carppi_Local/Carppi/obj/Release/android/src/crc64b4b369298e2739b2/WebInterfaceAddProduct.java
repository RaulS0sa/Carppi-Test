package crc64b4b369298e2739b2;


public class WebInterfaceAddProduct
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_FetchAllTehData:(Ljava/lang/String;Ljava/lang/String;Ljava/lang/String;Ljava/lang/String;I)V:__export__\n" +
			"n_SelecImage_HomeWork:()V:__export__\n" +
			"n_UpdateBottonOfImageSelect:(Ljava/lang/String;)V:__export__\n" +
			"n_ReloadUpdateProduct:()V:__export__\n" +
			"n_StopFetcherButtonAndReload:()V:__export__\n" +
			"n_StopFetcherButton:()V:__export__\n" +
			"";
		mono.android.Runtime.register ("Carppi.Fragments.WebInterfaceAddProduct, Carppi", WebInterfaceAddProduct.class, __md_methods);
	}


	public WebInterfaceAddProduct ()
	{
		super ();
		if (getClass () == WebInterfaceAddProduct.class)
			mono.android.TypeManager.Activate ("Carppi.Fragments.WebInterfaceAddProduct, Carppi", "", this, new java.lang.Object[] {  });
	}

	public WebInterfaceAddProduct (android.app.Activity p0, android.webkit.WebView p1)
	{
		super ();
		if (getClass () == WebInterfaceAddProduct.class)
			mono.android.TypeManager.Activate ("Carppi.Fragments.WebInterfaceAddProduct, Carppi", "Android.App.Activity, Mono.Android:Android.Webkit.WebView, Mono.Android", this, new java.lang.Object[] { p0, p1 });
	}

	@android.webkit.JavascriptInterface

	public void FetchAllTehData (java.lang.String p0, java.lang.String p1, java.lang.String p2, java.lang.String p3, int p4)
	{
		n_FetchAllTehData (p0, p1, p2, p3, p4);
	}

	private native void n_FetchAllTehData (java.lang.String p0, java.lang.String p1, java.lang.String p2, java.lang.String p3, int p4);

	@android.webkit.JavascriptInterface

	public void SelecImage_HomeWork ()
	{
		n_SelecImage_HomeWork ();
	}

	private native void n_SelecImage_HomeWork ();

	@android.webkit.JavascriptInterface

	public static void UpdateBottonOfImageSelect (java.lang.String p0)
	{
		n_UpdateBottonOfImageSelect (p0);
	}

	private static native void n_UpdateBottonOfImageSelect (java.lang.String p0);

	@android.webkit.JavascriptInterface

	public static void ReloadUpdateProduct ()
	{
		n_ReloadUpdateProduct ();
	}

	private static native void n_ReloadUpdateProduct ();

	@android.webkit.JavascriptInterface

	public static void StopFetcherButtonAndReload ()
	{
		n_StopFetcherButtonAndReload ();
	}

	private static native void n_StopFetcherButtonAndReload ();

	@android.webkit.JavascriptInterface

	public static void StopFetcherButton ()
	{
		n_StopFetcherButton ();
	}

	private static native void n_StopFetcherButton ();

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
