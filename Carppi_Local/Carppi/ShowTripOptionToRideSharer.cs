
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using static Carppi.Fragments.FragmentMain;
//using static Carppi.Fragments.FragmentMain.WebInterfaceMenuCarppi;

namespace Carppi
{
    [Activity(Label = "ShowTripOptionToRideSharer")]
    public class ShowTripOptionToRideSharer : Activity
    {
      //  public static CarppiRequestForDrive Data;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

           // WebInterfaceMenuCarppi.DisplayAceptRejectTripModal();

           // Create your application here
        }
        //public ShowTripOptionToRideSharer(CarppiRequestForDrive Data)
        //{
        //    //base.OnCreate(savedInstanceState);

        //    // Create your application here
        //}

       
    }
    
}
