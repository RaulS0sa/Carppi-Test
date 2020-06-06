
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
//using Android.Support.Design.Widget;
//using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
//using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using Carppi.Fragments;
//using AndroidX.DrawerLayout.Widget;
using SQLite;
using V7Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Carppi
{
    [Activity(Label = "RegularMenuActivity")]
    public class RegularMenuActivity : AppCompatActivity
    {
        BottomNavigationView bottomNavigation;
        public static BottomSheetBehavior mbottomSheetBehavior { get; set; }
        internal static readonly string CHANNEL_ID = "my_notification_channel";
        internal static readonly int NOTIFICATION_ID = 100;
        public static Activity Static_Activity;
        public static Android.Support.V4.App.FragmentManager static_FragmentMAnager;
        public static bool SetDismisableModal { get; set; } = false;
        DrawerLayout drawerLayout;
        NavigationView navigationView;

       
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            configureBackdrop();

            //  base.OnCreate(bundle);
            //SetContentView(Resource.Layout.main);
            //var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            //if (toolbar != null)
            //{
            //    SetSupportActionBar(toolbar);
            //    SupportActionBar.SetDisplayHomeAsUpEnabled(false);
            //    SupportActionBar.SetHomeButtonEnabled(false);
            // }

            //bottomNavigation = FindViewById<BottomNavigationView>(Resource.Id.bottom_navigation);

            //bottomNavigation.NavigationItemSelected += BottomNavigation_NavigationItemSelected;
            //FragmentGrocery
            // Load the first fragment on creation
            var toolbar = FindViewById<V7Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            // StaticfragmentActivity = SupportFragmentManager;

            var drawerToggle = new ActionBarDrawerToggle(this, drawerLayout, toolbar, Resource.String.drawer_open, Resource.String.drawer_close);
            try
            {
                drawerLayout.SetDrawerListener(drawerToggle);
                drawerToggle.SyncState();

                navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
                setupDrawerContent(navigationView);
              //  LoadFragment(Resource.Id.nav_GroceryRequest);


//                IsPlayServicesAvailable();

               // CreateNotificationChannel();

                //UpdateRegion();

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
            }

            navigationView.InflateMenu(Resource.Menu.nav_menu);
        }
        void setupDrawerContent(NavigationView navigationView)
        {
            navigationView.NavigationItemSelected += (sender, e) => {
               // LoadFragment(e.MenuItem.ItemId);
                e.MenuItem.SetChecked(true);

                drawerLayout.CloseDrawers();
            };
        }
        private void configureBackdrop()
        {
            //uncoment This

            View asd = FindViewById(Resource.Id.LinLAy_BottomSheet);
            mbottomSheetBehavior = BottomSheetBehavior.From(asd);
            mbottomSheetBehavior.Hideable = false;




            mbottomSheetBehavior.SetBottomSheetCallback(new BottomCalback());
            mbottomSheetBehavior.FitToContents = true;

        }
        protected override async void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if ((requestCode == (int)WebInterfaceAddProduct.ActivityIntentREturns.UpdateProfilePicture) && (resultCode == Result.Ok) && (data != null))
            {
                /*
                Android.Net.Uri uri = data.Data;
                Stream iStream = ContentResolver.OpenInputStream(uri);
                byte[] inputData = getBytes(iStream);
                WebAppInterface_Fragment_profile.PersonalPhoto = inputData;
                WebAppInterface_Fragment_profile.UpdateBottonOfProfileImageSelect(getFileName(uri));
                */


            }
            else if ((requestCode == (int)WebInterfaceAddProduct.ActivityIntentREturns.SelecctionOfHomeworkPicture) && (resultCode == Result.Ok) && (data != null))
            {
                Android.Net.Uri uri = data.Data;
                Stream iStream = ContentResolver.OpenInputStream(uri);
                byte[] inputData = getBytes(iStream);
                WebInterfaceAddProduct.Photo = inputData;
                WebInterfaceAddProduct.UpdateBottonOfImageSelect(getFileName(uri));
                //  WebAppInterface_Fragment_profile.PersonalPhoto = inputData;

            }
        }
        public static byte[] getBytes(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
        public string getFileName(Android.Net.Uri uri)
        {
            string result = null;
            if (uri.Scheme.Equals("content"))//uri.getScheme().equals("content"))
            {

                var cursor = ContentResolver.Query(uri, null, null, null);//ContentResolver().query(uri, null, null, null, null);
                try
                {
                    if (cursor != null && cursor.MoveToFirst())//cursor.moveToFirst())
                    {
                        result = cursor.GetString(cursor.GetColumnIndex(OpenableColumns.DisplayName));
                        //result = cursor.getString(cursor.getColumnIndex(OpenableColumns.DISPLAY_NAME));
                    }
                }
                finally
                {
                    cursor.Close();
                    //   cursor.close();
                }
            }
            if (result == null)
            {
                result = uri.Path;//getPath();
                int cut = result.LastIndexOf('/');//.lastIndexOf('/');
                if (cut != -1)
                {
                    result = result.Substring(cut + 1);
                    //result = result.substring(cut + 1);
                }
            }
            return result;
        }


        

    }
    public class BottomCalback : BottomSheetBehavior.BottomSheetCallback
    {
        public int PreviousState = BottomSheetBehavior.StateHidden;
        public override void OnSlide(View bottomSheet, float slideOffset)
        {
            //throw new NotImplementedException();

        }




        public override void OnStateChanged(View bottomSheet, int newState)
        {
            /*
            if (PreviousState == BottomSheetBehavior.StateHalfExpanded && newState == BottomSheetBehavior.StateHidden && WebInterfaceMenuCarppi.EstadoPrevioDelUsuario == WebInterfaceMenuCarppi.enumEstado_del_usuario.BusquedaEnProceso)
            {
                CancelAlProposalls();

            }
            if (newState == BottomSheetBehavior.StateDragging && !SetDismisableModal)
            {

                MainActivity.mbottomSheetBehavior.State = BottomSheetBehavior.StateExpanded;
            }
            */

            PreviousState = newState;
        }
        public async void CancelAlProposalls()
        {
            HttpClient client = new HttpClient();
            //Post_Travel(string Argument, string FaceId, string Vehiculo, string Costo)
            var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
            var db5 = new SQLiteConnection(databasePath5);
            var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID == 1).FirstOrDefault();

            var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/TravelerCrossCityApi/CancellProposals?" +
                "user9_Hijo=" + query.ProfileId

                ));
            HttpResponseMessage response;

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            response = await client.GetAsync(uri);


            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
            {
                var errorMessage1 = response.Content.ReadAsStringAsync().Result.Replace("\\", "").Trim(new char[1]
          {
                '"'
          });


            }
        }
    }
}
