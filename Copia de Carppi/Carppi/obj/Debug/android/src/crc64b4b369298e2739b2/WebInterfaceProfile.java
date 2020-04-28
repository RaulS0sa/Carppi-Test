package crc64b4b369298e2739b2;


public class WebInterfaceProfile
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.facebook.FacebookCallback
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_DisplayPaymentsDashboard:()V:__export__\n" +
			"n_RetriveProfile:()V:__export__\n" +
			"n_AddProviderTOBussined:()V:__export__\n" +
			"n_ChangeToDriverMode:()V:__export__\n" +
			"n_DisplayMyStats:()V:__export__\n" +
			"n_ShowFacebookLogButton:()V:__export__\n" +
			"n_onCancel:()V:GetOnCancelHandler:Xamarin.Facebook.IFacebookCallbackInvoker, Xamarin.Facebook.Common.Android\n" +
			"n_onError:(Lcom/facebook/FacebookException;)V:GetOnError_Lcom_facebook_FacebookException_Handler:Xamarin.Facebook.IFacebookCallbackInvoker, Xamarin.Facebook.Common.Android\n" +
			"n_onSuccess:(Ljava/lang/Object;)V:GetOnSuccess_Ljava_lang_Object_Handler:Xamarin.Facebook.IFacebookCallbackInvoker, Xamarin.Facebook.Common.Android\n" +
			"";
		mono.android.Runtime.register ("Carppi.Fragments.WebInterfaceProfile, Carppi", WebInterfaceProfile.class, __md_methods);
	}


	public WebInterfaceProfile ()
	{
		super ();
		if (getClass () == WebInterfaceProfile.class)
			mono.android.TypeManager.Activate ("Carppi.Fragments.WebInterfaceProfile, Carppi", "", this, new java.lang.Object[] {  });
	}

	public WebInterfaceProfile (android.app.Activity p0, android.webkit.WebView p1)
	{
		super ();
		if (getClass () == WebInterfaceProfile.class)
			mono.android.TypeManager.Activate ("Carppi.Fragments.WebInterfaceProfile, Carppi", "Android.App.Activity, Mono.Android:Android.Webkit.WebView, Mono.Android", this, new java.lang.Object[] { p0, p1 });
	}

	@android.webkit.JavascriptInterface

	public void DisplayPaymentsDashboard ()
	{
		n_DisplayPaymentsDashboard ();
	}

	private native void n_DisplayPaymentsDashboard ();

	@android.webkit.JavascriptInterface

	public static void RetriveProfile ()
	{
		n_RetriveProfile ();
	}

	private static native void n_RetriveProfile ();

	@android.webkit.JavascriptInterface

	public void AddProviderTOBussined ()
	{
		n_AddProviderTOBussined ();
	}

	private native void n_AddProviderTOBussined ();

	@android.webkit.JavascriptInterface

	public void ChangeToDriverMode ()
	{
		n_ChangeToDriverMode ();
	}

	private native void n_ChangeToDriverMode ();

	@android.webkit.JavascriptInterface

	public void DisplayMyStats ()
	{
		n_DisplayMyStats ();
	}

	private native void n_DisplayMyStats ();

	@android.webkit.JavascriptInterface

	public static void ShowFacebookLogButton ()
	{
		n_ShowFacebookLogButton ();
	}

	private static native void n_ShowFacebookLogButton ();


	public void onCancel ()
	{
		n_onCancel ();
	}

	private native void n_onCancel ();


	public void onError (com.facebook.FacebookException p0)
	{
		n_onError (p0);
	}

	private native void n_onError (com.facebook.FacebookException p0);


	public void onSuccess (java.lang.Object p0)
	{
		n_onSuccess (p0);
	}

	private native void n_onSuccess (java.lang.Object p0);

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
