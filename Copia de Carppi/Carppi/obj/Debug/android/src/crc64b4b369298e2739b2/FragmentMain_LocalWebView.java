package crc64b4b369298e2739b2;


public class FragmentMain_LocalWebView
	extends android.webkit.WebView
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCheckIsTextEditor:()Z:GetOnCheckIsTextEditorHandler\n" +
			"";
		mono.android.Runtime.register ("Carppi.Fragments.FragmentMain+LocalWebView, Carppi", FragmentMain_LocalWebView.class, __md_methods);
	}


	public FragmentMain_LocalWebView (android.content.Context p0)
	{
		super (p0);
		if (getClass () == FragmentMain_LocalWebView.class)
			mono.android.TypeManager.Activate ("Carppi.Fragments.FragmentMain+LocalWebView, Carppi", "Android.Content.Context, Mono.Android", this, new java.lang.Object[] { p0 });
	}


	public FragmentMain_LocalWebView (android.content.Context p0, android.util.AttributeSet p1)
	{
		super (p0, p1);
		if (getClass () == FragmentMain_LocalWebView.class)
			mono.android.TypeManager.Activate ("Carppi.Fragments.FragmentMain+LocalWebView, Carppi", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android", this, new java.lang.Object[] { p0, p1 });
	}


	public FragmentMain_LocalWebView (android.content.Context p0, android.util.AttributeSet p1, int p2)
	{
		super (p0, p1, p2);
		if (getClass () == FragmentMain_LocalWebView.class)
			mono.android.TypeManager.Activate ("Carppi.Fragments.FragmentMain+LocalWebView, Carppi", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android:System.Int32, mscorlib", this, new java.lang.Object[] { p0, p1, p2 });
	}


	public FragmentMain_LocalWebView (android.content.Context p0, android.util.AttributeSet p1, int p2, boolean p3)
	{
		super (p0, p1, p2, p3);
		if (getClass () == FragmentMain_LocalWebView.class)
			mono.android.TypeManager.Activate ("Carppi.Fragments.FragmentMain+LocalWebView, Carppi", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android:System.Int32, mscorlib:System.Boolean, mscorlib", this, new java.lang.Object[] { p0, p1, p2, p3 });
	}


	public FragmentMain_LocalWebView (android.content.Context p0, android.util.AttributeSet p1, int p2, int p3)
	{
		super (p0, p1, p2, p3);
		if (getClass () == FragmentMain_LocalWebView.class)
			mono.android.TypeManager.Activate ("Carppi.Fragments.FragmentMain+LocalWebView, Carppi", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android:System.Int32, mscorlib:System.Int32, mscorlib", this, new java.lang.Object[] { p0, p1, p2, p3 });
	}


	public boolean onCheckIsTextEditor ()
	{
		return n_onCheckIsTextEditor ();
	}

	private native boolean n_onCheckIsTextEditor ();

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
