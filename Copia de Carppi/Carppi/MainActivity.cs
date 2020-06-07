using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Carppi.Fragments;
using Android.Support.Design.Widget;
using Android.Views;
using Android;
using SQLite;
using System.Net.Http;
using System;
using System.Net.Http.Headers;
using Android.Gms.Common;
using static Carppi.Fragments.FragmentMain;
using V7Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V4.Widget;
using Xamarin.Essentials;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Facebook;
using AlertDialog = Android.Support.V7.App.AlertDialog;
using System.Collections.Generic;
using Xamarin.Facebook.Login.Widget;
using Newtonsoft.Json;
using static Carppi.Fragments.WebInterfaceProfile;
using Android.Support.V4.App;
using Fragment = Android.Support.V4.App.Fragment;
using ActionBarDrawerToggle = Android.Support.V7.App.ActionBarDrawerToggle;

[assembly: Application(Debuggable = false)]

namespace Carppi
{
    
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    [assembly: UsesFeature("android.hardware.camera", Required = false)]
    [assembly: UsesFeature("android.hardware.camera.autofocus", Required = false)]
    public class MainActivity : AppCompatActivity
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
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Static_Activity = this;
            static_FragmentMAnager = SupportFragmentManager;

            
            Android.Support.V4.App.ActivityCompat.RequestPermissions(this, new System.String[] {  Manifest.Permission.AccessFineLocation, Manifest.Permission.AccessCoarseLocation, Manifest.Permission.LocationHardware, Manifest.Permission.Internet, }, 1);
            
            SetContentView(Resource.Layout.activity_main);
            configureBackdrop();

           /*
            * To comeback when there are more rows

            bottomNavigation = FindViewById<BottomNavigationView>(Resource.Id.bottom_navigation);

            bottomNavigation.NavigationItemSelected += BottomNavigation_NavigationItemSelected;
            */
            
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
                //LoadFragment(Resource.Id.nav_home);
                LoadFragment(Resource.Id.menu_video);


                IsPlayServicesAvailable();

                CreateNotificationChannel();
               
                //UpdateRegion();

            }
            catch(Exception ex)
            {

                Console.WriteLine(ex.ToString());
            }

            navigationView.InflateMenu(Resource.Menu.nav_menu);
          //  Task.Delay(1500).ContinueWith(t => HideOptionsInMenu(navigationView));
            
        }
        async void HideOptionsInMenu(NavigationView mNavigationView)
        {
            try { 
            // var Mnu = FindViewById<IMenu>(Resource.Id.MuttaMenu);
            var menuNav = mNavigationView.Menu;
            if (isLoggedIn() == false)
            {
                var aca = menuNav.FindItem(Resource.Id.nav_messages).SetVisible(false);

                menuNav.FindItem(Resource.Id.nav_friends).SetVisible(false);
                menuNav.FindItem(Resource.Id.nav_discussion).SetVisible(false);
                menuNav.FindItem(Resource.Id.nav_Clabe).SetVisible(false);
                menuNav.FindItem(Resource.Id.nav_Balance).SetVisible(false);
                menuNav.FindItem(Resource.Id.nav_LogOutButton).SetVisible(false);

            }
            else
            {
                var aca = menuNav.FindItem(Resource.Id.nav_messages).SetVisible(false);

                menuNav.FindItem(Resource.Id.nav_friends).SetVisible(false);
                menuNav.FindItem(Resource.Id.nav_LoginButton).SetVisible(false);
                try
                {
                    HttpClient client = new HttpClient();
                    //Post_Travel(string Argument, string FaceId, string Vehiculo, string Costo)
                    var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
                    var db5 = new SQLiteConnection(databasePath5);
                    var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID == 1).FirstOrDefault();

                    var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/TravelerCrossCityApi/GEetOwnProfile?" +
                        "user10_Hijo=" + query.ProfileId
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
                        var Profile = JsonConvert.DeserializeObject<Traveler_Perfil>(errorMessage1);
                        var RealRole = Profile.IsUserADriver;
                        if (RealRole == true)//UserIsADriver
                        {
                            menuNav.FindItem(Resource.Id.nav_discussion).SetVisible(false);
                            if (Profile.StripeDriverID == null)
                            {
                                menuNav.FindItem(Resource.Id.nav_Balance).SetVisible(false);

                            }
                            else
                            {
                                menuNav.FindItem(Resource.Id.nav_Clabe).SetVisible(false);

                            }
                        }
                        else
                        {
                            menuNav.FindItem(Resource.Id.nav_Clabe).SetVisible(false);
                            menuNav.FindItem(Resource.Id.nav_Balance).SetVisible(false);
                            // menuNav.FindItem(Resource.Id.nav_LogOutButton).SetVisible(false);

                        }

                        //  MainActivity.LoadFragmentStatic(Resource.Id.menu_video);


                    }
                }
                catch (System.Exception) { }

            }
            /*
                < item
      android: id = "@+id/nav_home"
      android: icon = "@drawable/ic_dashboard"
      android: title = "Pantalla Principal" />
 
     < item
      android: id = "@+id/nav_messages"
      android: icon = "@drawable/ic_event"
      android: title = "Cambiar Foto" />
 
     < item
      android: id = "@+id/nav_friends"
      android: icon = "@drawable/ic_headset"
      android: title = "Mis Estadisticas" />
 
     < item
      android: id = "@+id/nav_discussion"
      android: icon = "@drawable/ic_forum"
      android: title = "Quiero Ser Conductor!" />
 
     < item
      android: id = "@+id/nav_Clabe"
      android: icon = "@drawable/ic_forum"
      android: title = "Ingresa Tu Clabe" />
 
     < item
      android: id = "@+id/nav_Balance"
      android: icon = "@drawable/ic_forum"
      android: title = "Mi Balance" />
 
     < item
      android: id = "@+id/nav_LoginButton"
      android: icon = "@drawable/ic_forum"
      android: title = "Log In" />
 
      < item
      android: id = "@+id/nav_LogOutButton"
      android: icon = "@drawable/ic_forum"
      android: title = "Log Out" />
      */
        }
            catch(Exception)
            { }
        }
        public bool isLoggedIn()
        {

            AccessToken accessToken = AccessToken.CurrentAccessToken;//AccessToken.getCurrentAccessToken();
            return accessToken != null;
            //return false;
        }
        public async void UpdateLocation()
        {
            try
            {
                //http://geolocale.azurewebsites.net/api/tcarppirideshare/SearchForPassengerArea?LatitudUser=20&LongitudUser=-100
                var MyLatLong = await Clases.Location.GetCurrentPosition();


                var placemarks = await Geocoding.GetPlacemarksAsync(MyLatLong.Latitude, MyLatLong.Longitude);

                var placemark = placemarks?.FirstOrDefault();



                HttpClient client = new HttpClient();
                //Post_Travel(string Argument, string FaceId, string Vehiculo, string Costo)
                var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
                var db5 = new SQLiteConnection(databasePath5);
                var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID == 1).FirstOrDefault();

                var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/tcarppirideshare/SearchForPassengerAreaByStateAndCountry?" +
                    "Country=" + placemark.CountryName
                    + "&State=" + placemark.AdminArea
                    + "&FacebookID_UpdateArea=" + null


                    ));
                // HttpResponseMessage response;

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //  var  response =  client.GetAsync(uri).Result;
                var t = Task.Run(() => GetResponseFromURI(uri));
                t.Wait();
                var S_Ressult = t.Result;
               // var rrr = JsonConvert.DeserializeObject<CarppiRegiones>(S_Ressult.Response);
                try
                {
               //     query.Region = rrr.ID;
               //     query.ServiciosRegionales = rrr.ServicioRegional;
               //     db5.RunInTransaction(() =>
               //     {
               //         db5.Update(query);
               //     });
                }
                catch (Exception ex)
                {

                }
            }

            catch (Exception Ex)
            {

            }
        }
        public static async Task<UriResponse> GetResponseFromURI(Uri u)
        {
            var response = "";
            var Respuesta = new UriResponse();
            using (var client = new HttpClient())
            {
                HttpResponseMessage result = await client.GetAsync(u);
                // var result = client.GetAsync(u).Result;
                Respuesta.httpStatusCode = result.StatusCode;
                //if (result.IsSuccessStatusCode)
                //{
                Respuesta.Response = await result.Content.ReadAsStringAsync();
                //}
            }
            return Respuesta;
        }
        public class UriResponse
        {
            public String Response;
            public System.Net.HttpStatusCode httpStatusCode;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {

         //   navigationView.InflateMenu(Resource.Menu.nav_menu);
         //   HideOptionsInMenu(menu);
            return true;

        }
        void setupDrawerContent(NavigationView navigationView)
        {
            navigationView.NavigationItemSelected += (sender, e) => {
                LoadFragment(e.MenuItem.ItemId);
                e.MenuItem.SetChecked(true);
                
                drawerLayout.CloseDrawers();
            };
        }
        public bool IsPlayServicesAvailable()
        {
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (resultCode != ConnectionResult.Success)
            {
                if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
                {
                    // msgText.Text = GoogleApiAvailability.Instance.GetErrorString(resultCode);
                }
                else
                {
                    //  msgText.Text = "This device is not supported";
                    Finish();
                }
                return false;
            }
            else
            {
                //  msgText.Text = "Google Play Services is available.";
                return true;
            }
        }

        void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                // Notification channels are new in API 26 (and not a part of the
                // support library). There is no need to create a notification
                // channel on older versions of Android.
                return;
            }

            var channel = new NotificationChannel(CHANNEL_ID,
                                                  "FCM Notifications",
                                                  NotificationImportance.Default)
            {

                Description = "Firebase Cloud Messages appear in this channel"
            };

            var notificationManager = (NotificationManager)GetSystemService(Android.Content.Context.NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void configureBackdrop()
        {
            //uncoment This
            var scale = Resources.DisplayMetrics.Density;//density i.e., pixels per inch or cms

            var widthPixels = Resources.DisplayMetrics.WidthPixels;////getting the height in pixels  
            var width = (double)((widthPixels - 0.5f) / scale);//width in units //411 //359
            var heightPixels = Resources.DisplayMetrics.HeightPixels;////getting the height in pixels  
            var height = (double)((heightPixels - 0.5f) / scale);//731 //680


            View asd = FindViewById(Resource.Id.LinLAy_BottomSheet);
            mbottomSheetBehavior = BottomSheetBehavior.From(asd);
            //mbottomSheetBehavior.OnAttachedToLayoutParams(new CoordinatorLayout.LayoutParams((int)width, (int)(height*0.8)));
            mbottomSheetBehavior.PeekHeight = (int)(height * 0.8);
            
            mbottomSheetBehavior.Hideable = false;




            mbottomSheetBehavior.SetBottomSheetCallback(new BottomCalback());
            mbottomSheetBehavior.FitToContents = true;

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
                // MainActivity.mbottomSheetBehavior.PeekHeight = 0;

                if (newState != BottomSheetBehavior.StateCollapsed)
                {
                    
                    MainActivity.mbottomSheetBehavior.PeekHeight = 500;
                }
                else
                {
                    MainActivity.mbottomSheetBehavior.PeekHeight = 0;
                }
               if(PreviousState  == BottomSheetBehavior.StateHalfExpanded && newState == BottomSheetBehavior.StateHidden && WebInterfaceMenuCarppi.EstadoPrevioDelUsuario== WebInterfaceMenuCarppi.enumEstado_del_usuario.BusquedaEnProceso)
                {
                    CancelAlProposalls();
                 
                }
                if(newState == BottomSheetBehavior.StateDragging && !SetDismisableModal)
                {

                    MainActivity.mbottomSheetBehavior.State = BottomSheetBehavior.StateExpanded;
                }

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

      
        protected override void OnActivityResult(int requestCode, Result resultCode, Android.Content.Intent data)
        {

            base.OnActivityResult(requestCode, resultCode, data);

           // mFBCallManager.OnActivityResult(requestCode, (int)resultCode, data);
            mFBCallManager.OnActivityResult(requestCode, (int)resultCode, data);
        }

        public override void OnAttachFragment(Android.App.Fragment fragment)
        {
            base.OnAttachFragment(fragment);
        }

        private void BottomNavigation_NavigationItemSelected(object sender, BottomNavigationView.NavigationItemSelectedEventArgs e)
        {
            LoadFragment(e.Item.ItemId);
        }
        void LoadFragment(int id)
        {
            Android.Support.V4.App.Fragment fragment = null;
            switch (id)
            {
                case Resource.Id.nav_home:
                    {
                        fragment = FragmentMain.NewInstance();
                        if (FragmentGrocery.aTimer != null)
                        {
                            FragmentGrocery.aTimer.Enabled = false;

                        }

                    }
                    break;
              
                case Resource.Id.nav_LogOutButton:
                    ShowFacebookLogButton();
                    
                    break;
                case Resource.Id.nav_LoginButton:
                    ShowFacebookLogButton();
                    
                    break;
                    /*
                     * CarpoolTocomeback
                case Resource.Id.menu_home:
                    {
                        
                        fragment = FragmentMain.NewInstance();
                        if(FragmentGrocery.aTimer != null)
                        {
                            FragmentGrocery.aTimer.Enabled = false;

                        }

                    }
                    break;
                    */
                case Resource.Id.menu_video:
                    {
                        fragment = FragmentSelectTypeOfPurchase.NewInstance();
                        /*
                        fragment = FragmentGrocery.NewInstance();

                        if (FragmentMain.aTimer != null)
                        {
                            FragmentMain.aTimer.Enabled = false;

                        }
                        */
                    }
                    break;

                case Resource.Id.nav_GroceryRequest:
                    fragment = FragmentGroceryRequest.NewInstance();
                    break;
                case Resource.Id.nav_GroceryConversation:
                    fragment = FragmentGroceryRequestConversaciones.NewInstance();
                    break;
                case Resource.Id.nav_discussion://Quiero ser Conductor opcion
                    throw new NotImplementedException();
                    break;
                case Resource.Id.nav_Clabe:
                    throw new NotImplementedException();
                    break;
                case Resource.Id.nav_Balance:
                    throw new NotImplementedException();
                    break;
              
            }

            if (fragment == null)
                return;

            SupportFragmentManager.BeginTransaction()
                .Replace(Resource.Id.content_frame, fragment)
                .Commit();
        }

        public static void LoadFragment_Static(int id)
        {
            Android.Support.V4.App.Fragment fragment = null;
            switch (id)
            {
                case Resource.Id.nav_home:
                    fragment = FragmentMain.NewInstance();
                    break;

                case Resource.Id.nav_LogOutButton:
                    ShowFacebookLogButton();

                    break;
                case Resource.Id.nav_LoginButton:
                    ShowFacebookLogButton();

                    break;
                    /*
                     * Carppol ToComeback
                case Resource.Id.menu_home:
                    fragment = FragmentMain.NewInstance();
                    break;
                    */
                case Resource.Id.menu_video:
                    fragment = FragmentSelectTypeOfPurchase.NewInstance();
                    //fragment = FragmentGrocery.NewInstance();
                    break;

                case Resource.Id.nav_GroceryRequest:
                    fragment = FragmentGroceryRequest.NewInstance();
                    break;
                case Resource.Id.nav_discussion://Quiero ser Conductor opcion
                    throw new NotImplementedException();
                    break;
                case Resource.Id.nav_Clabe:
                    throw new NotImplementedException();
                    break;
                case Resource.Id.nav_Balance:
                    throw new NotImplementedException();
                    break;

            }

            if (fragment == null)
                return;
            //fragment = FragmentSelectTypeOfPurchase.NewInstance();
            MainActivity.static_FragmentMAnager.BeginTransaction()
                .Replace(Resource.Id.content_frame, fragment)
                .Commit();
        }
        public static LoginButton BtnFBLogin { get; private set; }
        public static ICallbackManager mFBCallManager;
        private static MyProfileTracker mprofileTracker;
        public static async void ShowFacebookLogButton()
        {

            Action action = () =>
            {
                FacebookSdk.SdkInitialize(Static_Activity);
                AccessToken.RefreshCurrentAccessTokenAsync();
                //  LoginManager.Instance.LogInWithReadPermissions((Activity)mContext, new List<string> { "public_profile" });

                //MyNameLoginID
                //MyLoginEmailID
                /*

                FacebookSdk.SdkInitialize(Fragment_profile.act);
                AccessToken.RefreshCurrentAccessTokenAsync();
                LoginManager.Instance.LogInWithReadPermissions(Fragment_profile.act, new List<string> { "public_profile" });
                */
                var TextTag = false;
                try
                {
                    var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
                    var db5 = new SQLiteConnection(databasePath5);
                    var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID > 0).FirstOrDefault();
                    if (query != null)
                    {
                        TextTag = true;
                    }
                }
                catch (SQLiteException)
                {

                }
                //AlertDialog MainAlert = new AlertDialog(mContext);
                AlertDialog.Builder alert = new AlertDialog.Builder(Static_Activity);
                alert.SetTitle("Login");
                // alert.SetMessage("Do you want to add or substract?");
                var c_view = ((Activity)Static_Activity).LayoutInflater.Inflate(Resource.Layout.LoginLayout, null);
                //c_view.FindViewById<EditText>(Resource.Id.MyNameLoginID).Visibility = TextTag ? ViewStates.Invisible : ViewStates.Visible;
                //c_view.FindViewById<EditText>(Resource.Id.MyLoginEmailID).Visibility = TextTag ? ViewStates.Invisible : ViewStates.Visible;
                //c_view.FindViewById<EditText>(Resource.Id.MyLastNameLoginID).Visibility = TextTag ? ViewStates.Invisible : ViewStates.Visible;


                alert.SetView(c_view);

                //alert.SetPositiveButton(TextTag ? "Cerrar Sesion" : "Crear Perfil", (senderAlert, args) =>
                //{



                //});

                alert.SetNegativeButton("Cerrar", (senderAlert, args) =>
                {
                    // count--;
                    // button.Text = string.Format("{0} clicks!", count);
                });





                BtnFBLogin = c_view.FindViewById<LoginButton>(Resource.Id.fblogin);

                BtnFBLogin.SetPermissions(new List<string> { "public_profile", "email" });
                //  BtnFBLogin.Fragment = Fragment_profile.act_fragment;


                mFBCallManager = CallbackManagerFactory.Create();
                var facebookCallback_Static = new facebookCallback();
                BtnFBLogin.RegisterCallback(mFBCallManager, facebookCallback_Static);

                mprofileTracker = new MyProfileTracker();
                mprofileTracker.mOnProfileChanged += mProfileTracker_mOnProfileChanged;
                mprofileTracker.StartTracking();


                Dialog dialog = alert.Create();

                //dialog.SetOnShowListener(new S_listen(dialog, (AlertDialog)dialog, TextTag, c_view, mContext));
                dialog.Show();
            };
            ((Activity)Static_Activity).RunOnUiThread(action);
        }
        public static async void mProfileTracker_mOnProfileChanged(object sender, OnProfileChangedEventArgs e)
        {

            Action action = async () =>
            {

                if (e.mProfile != null)
                {
                    try
                    {
                        // LoginManager.Instance.LogInWithReadPermissions((Activity)mContext, new List<string>() { "public_profile" });
                        //var databasePath5 = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");

                        var databasePath10 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
                        var db10 = new SQLiteConnection(databasePath10);
                        // var query = db5.Table<DatabaseTypes.Log_Info>().Where(v => v.ID > 0).FirstOrDefault();
                        // var db5 = new SQLiteConnection(databasePath5);
                        //var query = db5.Table<App6.DatabaseTypes.Log_info>().Where(v => v.ID > 0).FirstOrDefault();
                        db10.CreateTable<DatabaseTypes.Log_info>();



                        //    TxtFirstName.Text = e.mProfile.FirstName;
                        //TxtLastName.Text = e.mProfile.LastName;
                        //TxtName.Text = e.mProfile.Name;
                        //mprofile.ProfileId = e.mProfile.Id;
                        //  var uri = "https://graph.facebook.com/" + e.mProfile.Id + "/picture?type=large";

                        //System.Net.WebRequest request = System.Net.WebRequest.Create(uri);  //  System.Net.WebRequest.CreateDefault();
                        //System.Net.WebResponse response = request.GetResponse();
                        //What?
                        //Todo: Handle with the login whitout internet

                        //     System.IO.Stream responseStream = response.GetResponseStream();

                        //var uri2 = "https://graph.facebook.com/" + e.mProfile.Id + "/birthday";

                        //System.Net.WebRequest request2 = System.Net.WebRequest.Create(uri2);  //  System.Net.WebRequest.CreateDefault();
                        //System.Net.WebResponse response2 = request.GetResponse();
                        //System.IO.Stream responseStream2 = response2.GetResponseStream();

                        //   var arra = ReadFully(responseStream);

                        //var cadena = Convert.ToBase64String(arra);
                        //ClasContainer.Imagen_base64 = cadena;
                        //ClasContainer.nombre_del_usuario = e.mProfile.Name;

                        var query = db10.Table<DatabaseTypes.Log_info>().Where(v => v.ID > 0).FirstOrDefault();
                        if (query == null)
                        {


                            var s = db10.Insert(new DatabaseTypes.Log_info()
                            {
                                Name = e.mProfile.Name,
                                LastName = e.mProfile.LastName,
                                Firstname = e.mProfile.FirstName,
                                // Photo = cadena,
                                Viajesrelizados = "0",
                                Calificacion = "0",
                                ProfileId = e.mProfile.Id
                                //NombreUsuaro = e.mProfile.FirstName,
                                //ApellidoPaterno = e.mProfile.LastName,
                                //Calificacion = 5.0,
                                //FaceID = e.mProfile.Id,
                                //IdentidadValidada = false,
                                //EsPrimerUso = true,
                                //Escuela = "",
                                ////Foto = arra,
                                //CostoPorHora = new decimal(0)



                            });
                        }
                        else
                        {
                            query.Name = e.mProfile.Name;
                            query.LastName = e.mProfile.LastName;
                            query.Firstname = e.mProfile.FirstName;
                            // Photo = cadena,
                            query.Viajesrelizados = "0";
                            query.Calificacion = "0";
                            query.ProfileId = e.mProfile.Id;
                            db10.RunInTransaction(() =>
                            {
                                db10.Update(query);
                            });
                        }


                        try
                        {
                            await LoggInWebTask();
                        }
                        catch (System.Exception) { }

                       // MainActivity.LoadFragmentStatic(Resource.Id.nav_messages);

                        //Intent i = new Intent(this, typeof(Activity1));
                        //  Intent i = new Intent(this, typeof(Activity_Informacion_Post_logueo));
                        //  StartActivity(i);

                        // Toast.MakeText(this, e.mProfile.LinkUri.ToString(), ToastLength.Short).Show();
                    }
                    catch (System.Exception ex)
                    {

                    }
                }

                else
                {
                    try
                    {
                        //   LoginManager.Instance.LogOut();
                        // LoginManager.getInstance().logOut();
                        var databasePath10_ = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
                        var db10_ = new SQLiteConnection(databasePath10_);

                        db10_.RunInTransaction(() =>
                        {
                            db10_.DropTable<DatabaseTypes.Log_info>();
                            //db2.Delete(db2.Table<intento_todo_parte_2.Database_clases.Notificaciones>().Where(x => x.ID >0));
                        });

                        //Todo: Exception trown at unlog
                        //TxtFirstName.Text = "First Name";
                        //TxtLastName.Text = "Last Name";
                        //TxtName.Text = "Name";
                        // mprofile.ProfileId = null;
                    }
                    catch (System.Exception) { }
                }

            }
;

            ((Activity)Static_Activity).RunOnUiThread(action);

        }
        public enum enumRoles { Pasajero, Conductor, Observador };
        public static async Task LoggInWebTask()
        {
            //public HttpResponseMessage RegisterUserProfile(string Face_identifier_2, enumRoles Role, string Nombre_usuario)
            try
            {
                HttpClient client = new HttpClient();
                //Post_Travel(string Argument, string FaceId, string Vehiculo, string Costo)
                var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
                var db5 = new SQLiteConnection(databasePath5);
                var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID == 1).FirstOrDefault();

                var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/TravelerCrossCityApi/RegisterUserProfile?" +
                    "Face_identifier_2=" + query.ProfileId + //VistaHTMLProffesores.Grupo_Activo +
                    "&Role=" + ((int)enumRoles.Pasajero).ToString() +
                    "&Nombre_usuario=" + query.Name +
                    "&FirstName=" + query.Firstname +
                    "&LastName=" + query.LastName +
                    "&FirebaseToken=" + query.FirebaseID
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
            catch (System.Exception) { }
        }



        public class facebookCallback : Java.Lang.Object, Xamarin.Facebook.IFacebookCallback
        {
            public void OnCancel()
            {
                // throw new NotImplementedException();
            }

            public void OnError(FacebookException error)
            {
                // throw new NotImplementedException();
            }

            public void OnSuccess(Java.Lang.Object result)
            {
                // throw new NotImplementedException();
            }
        }

        public static void LoadFragmentStatic(int id)
        {
            Android.Support.V4.App.Fragment fragment = null;
            switch (id)
            {
                case Resource.Id.nav_home:
                    fragment = FragmentMain.NewInstance();
                    break;
                // case Resource.Id.menu_audio:
                //     fragment = Fragment2.NewInstance();
                //     break;
                case Resource.Id.nav_messages:
                    fragment = FragmentProfile.NewInstance();
                    break;
            }

            if (fragment == null)
                return;

            MainActivity.static_FragmentMAnager.BeginTransaction()
                .Replace(Resource.Id.content_frame, fragment)
                .Commit();
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            var token = result.Class.GetMethod("getToken").Invoke(result).ToString();
            SendRegistrationToServerAsync(token);
        }
        async void SendRegistrationToServerAsync(string token)
        {

            var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
            var db5 = new SQLiteConnection(databasePath5);
            var query_log = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID == 1).FirstOrDefault();
            HttpClient client = new HttpClient();
            //Post_Travel(string Argument, string FaceId, string Vehiculo, string Costo)


            var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/TravelerCrossCityApi/UpdateFirebaseToken?" +
                "FaceID=" + query_log.ProfileId +//VistaHTMLProffesores.Grupo_Activo + Trip_Id
                "&FirebaseID=" + token

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
            // Add custom implementation, as needed.
        }
    }
}