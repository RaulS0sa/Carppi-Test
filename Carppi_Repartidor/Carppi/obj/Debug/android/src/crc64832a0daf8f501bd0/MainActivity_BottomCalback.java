package crc64832a0daf8f501bd0;


public class MainActivity_BottomCalback
	extends com.google.android.material.bottomsheet.BottomSheetBehavior.BottomSheetCallback
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onSlide:(Landroid/view/View;F)V:GetOnSlide_Landroid_view_View_FHandler\n" +
			"n_onStateChanged:(Landroid/view/View;I)V:GetOnStateChanged_Landroid_view_View_IHandler\n" +
			"";
		mono.android.Runtime.register ("Carppi.MainActivity+BottomCalback, Carppi", MainActivity_BottomCalback.class, __md_methods);
	}


	public MainActivity_BottomCalback ()
	{
		super ();
		if (getClass () == MainActivity_BottomCalback.class)
			mono.android.TypeManager.Activate ("Carppi.MainActivity+BottomCalback, Carppi", "", this, new java.lang.Object[] {  });
	}


	public void onSlide (android.view.View p0, float p1)
	{
		n_onSlide (p0, p1);
	}

	private native void n_onSlide (android.view.View p0, float p1);


	public void onStateChanged (android.view.View p0, int p1)
	{
		n_onStateChanged (p0, p1);
	}

	private native void n_onStateChanged (android.view.View p0, int p1);

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
