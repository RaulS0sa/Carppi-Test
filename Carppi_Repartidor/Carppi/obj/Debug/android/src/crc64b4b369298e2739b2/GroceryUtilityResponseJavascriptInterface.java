package crc64b4b369298e2739b2;


public class GroceryUtilityResponseJavascriptInterface
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_SendMessageRestaurantToDeliverMan:(Ljava/lang/String;Ljava/lang/String;)V:__export__\n" +
			"n_GetAllMessagesRestaurantDeliverMan:(Ljava/lang/String;)V:__export__\n" +
			"n_GoToClientOrder:(J)V:__export__\n" +
			"n_EnderOrden:(J)V:__export__\n" +
			"n_SendMessageInRideshare:(Ljava/lang/String;J)V:__export__\n" +
			"n_GetAllMessagesFromConversation:(J)V:__export__\n" +
			"n_RateUser:(I)V:__export__\n" +
			"n_DissmissBottomModal:()V:__export__\n" +
			"";
		mono.android.Runtime.register ("Carppi.Fragments.GroceryUtilityResponseJavascriptInterface, Carppi", GroceryUtilityResponseJavascriptInterface.class, __md_methods);
	}


	public GroceryUtilityResponseJavascriptInterface ()
	{
		super ();
		if (getClass () == GroceryUtilityResponseJavascriptInterface.class)
			mono.android.TypeManager.Activate ("Carppi.Fragments.GroceryUtilityResponseJavascriptInterface, Carppi", "", this, new java.lang.Object[] {  });
	}

	public GroceryUtilityResponseJavascriptInterface (android.content.Context p0, android.webkit.WebView p1)
	{
		super ();
		if (getClass () == GroceryUtilityResponseJavascriptInterface.class)
			mono.android.TypeManager.Activate ("Carppi.Fragments.GroceryUtilityResponseJavascriptInterface, Carppi", "Android.Content.Context, Mono.Android:Android.Webkit.WebView, Mono.Android", this, new java.lang.Object[] { p0, p1 });
	}

	@android.webkit.JavascriptInterface

	public void SendMessageRestaurantToDeliverMan (java.lang.String p0, java.lang.String p1)
	{
		n_SendMessageRestaurantToDeliverMan (p0, p1);
	}

	private native void n_SendMessageRestaurantToDeliverMan (java.lang.String p0, java.lang.String p1);

	@android.webkit.JavascriptInterface

	public void GetAllMessagesRestaurantDeliverMan (java.lang.String p0)
	{
		n_GetAllMessagesRestaurantDeliverMan (p0);
	}

	private native void n_GetAllMessagesRestaurantDeliverMan (java.lang.String p0);

	@android.webkit.JavascriptInterface

	public void GoToClientOrder (long p0)
	{
		n_GoToClientOrder (p0);
	}

	private native void n_GoToClientOrder (long p0);

	@android.webkit.JavascriptInterface

	public void EnderOrden (long p0)
	{
		n_EnderOrden (p0);
	}

	private native void n_EnderOrden (long p0);

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

	public void RateUser (int p0)
	{
		n_RateUser (p0);
	}

	private native void n_RateUser (int p0);

	@android.webkit.JavascriptInterface

	public void DissmissBottomModal ()
	{
		n_DissmissBottomModal ();
	}

	private native void n_DissmissBottomModal ();

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
