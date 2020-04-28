package crc64b4b369298e2739b2;


public class UtilityJavascriptInterfaceGrocery
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_SendMessageInRideshare:(Ljava/lang/String;J)V:__export__\n" +
			"n_GetAllMessagesFromConversation:(J)V:__export__\n" +
			"n_GeneretaGroceryOrder:()V:__export__\n" +
			"n_UpdateProductList:(II)V:__export__\n" +
			"n_DissmissBottomModal:()V:__export__\n" +
			"n_UpdateConectedIDStripe:(Ljava/lang/String;)V:__export__\n" +
			"";
		mono.android.Runtime.register ("Carppi.Fragments.UtilityJavascriptInterfaceGrocery, Carppi", UtilityJavascriptInterfaceGrocery.class, __md_methods);
	}


	public UtilityJavascriptInterfaceGrocery ()
	{
		super ();
		if (getClass () == UtilityJavascriptInterfaceGrocery.class)
			mono.android.TypeManager.Activate ("Carppi.Fragments.UtilityJavascriptInterfaceGrocery, Carppi", "", this, new java.lang.Object[] {  });
	}

	public UtilityJavascriptInterfaceGrocery (android.content.Context p0, android.webkit.WebView p1)
	{
		super ();
		if (getClass () == UtilityJavascriptInterfaceGrocery.class)
			mono.android.TypeManager.Activate ("Carppi.Fragments.UtilityJavascriptInterfaceGrocery, Carppi", "Android.Content.Context, Mono.Android:Android.Webkit.WebView, Mono.Android", this, new java.lang.Object[] { p0, p1 });
	}

	@android.webkit.JavascriptInterface

	public void SendMessageInRideshare (java.lang.String p0, long p1)
	{
		n_SendMessageInRideshare (p0, p1);
	}

	private native void n_SendMessageInRideshare (java.lang.String p0, long p1);

	@android.webkit.JavascriptInterface

	public void GetAllMessagesFromConversation (long p0)
	{
		n_GetAllMessagesFromConversation (p0);
	}

	private native void n_GetAllMessagesFromConversation (long p0);

	@android.webkit.JavascriptInterface

	public void GeneretaGroceryOrder ()
	{
		n_GeneretaGroceryOrder ();
	}

	private native void n_GeneretaGroceryOrder ();

	@android.webkit.JavascriptInterface

	public static void UpdateProductList (int p0, int p1)
	{
		n_UpdateProductList (p0, p1);
	}

	private static native void n_UpdateProductList (int p0, int p1);

	@android.webkit.JavascriptInterface

	public void DissmissBottomModal ()
	{
		n_DissmissBottomModal ();
	}

	private native void n_DissmissBottomModal ();

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
