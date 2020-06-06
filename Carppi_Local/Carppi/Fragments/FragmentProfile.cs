
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Java.Interop;
using Newtonsoft.Json;
using SQLite;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;
using Xamarin.Facebook.Login.Widget;
using static Carppi.Fragments.FragmentMain;
using Fragment = Android.Support.V4.App.Fragment;
namespace Carppi.Fragments
{
    public class FragmentProfile : Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            var view1 = inflater.Inflate(Resource.Layout.Fragment1_Webview, container, false);

            string content;
            AssetManager assets = this.Activity.Assets;
            // act = this.Activity;
            //   using (StreamReader sr = new StreamReader(assets.Open("Conversation2.html")))

            var Template = !isLoggedIn() ? "LogNotFound.html" : "LogMenu.html";
            using (StreamReader sr = new StreamReader(assets.Open(Template)))
            {
                content = sr.ReadToEnd();
                var webi = view1.FindViewById<WebView>(Resource.Id.webView_);
                var wew = new WebInterfaceProfile(this.Activity, webi);
                //MainWebView = wew;
                // ;
                webi.AddJavascriptInterface(wew, "Android");

                webi.Settings.JavaScriptEnabled = true;

                webi.Settings.DomStorageEnabled = true;
                webi.Settings.LoadWithOverviewMode = true;
                webi.Settings.UseWideViewPort = true;
                webi.Settings.BuiltInZoomControls = true;
                webi.Settings.DisplayZoomControls = false;
                webi.Settings.SetSupportZoom(true);

                webi.Settings.JavaScriptEnabled = true;
                // webi.LoadUrl("http://geolocale.azurewebsites.net/geoloc");
                webi.LoadDataWithBaseURL(null, content, "text/html", "utf-8", null);

                // webi.LoadData(content, "text/html", null);
                webi.SetWebViewClient(new WebViewClient());
                WebInterfaceProfile.RetriveProfile();

                //wew.Get10LastHomeworks();
                //HTML String

                //Load HTML Data in WebView

            }

            return view1;
            // return base.OnCreateView(inflater, container, savedInstanceState);
        }
        public static FragmentProfile NewInstance()
        {
            var frag1 = new FragmentProfile { Arguments = new Bundle() };
            return frag1;
        }
        public bool isLoggedIn()
        {
           
              AccessToken accessToken = AccessToken.CurrentAccessToken;//AccessToken.getCurrentAccessToken();
              return accessToken != null;
            //return false;
        }
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
    public class WebInterfaceProfile: Java.Lang.Object, Xamarin.Facebook.IFacebookCallback
    {
        Context mContext;
        public static Context Static_mContext;
        WebView webi;
        public static WebView Static_Webi;
        //private ICallbackManager mFBCallManager;
        public static ICallbackManager mFBCallManager;
        private static MyProfileTracker mprofileTracker;
        private ProfilePictureView mprofile;
        public static facebookCallback facebookCallback_Static;

        public WebInterfaceProfile(Activity Act, WebView web)
        {
            mContext = Act;
            webi = web;
            Static_Webi = web;
            Static_mContext = Act;
            facebookCallback_Static = new facebookCallback();
        }

        public static LoginButton BtnFBLogin { get; private set; }

        public void OnCancel()
        {
            //throw new NotImplementedException();
        }

        public void OnError(FacebookException error)
        {
            //throw new NotImplementedException();
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            //throw new NotImplementedException();
        }

        [JavascriptInterface]
        [Export("DisplayPaymentsDashboard")]
        public void DisplayPaymentsDashboard()
        {
            /*
            Action action_WhowAlert = () =>
            {
                var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
                var db5 = new SQLiteConnection(databasePath5);
                var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID == 1).FirstOrDefault();

                AlertDialog.Builder alert = new AlertDialog.Builder(mContext);

                LocalWebView sss = new LocalWebView(mContext);
                sss.Settings.JavaScriptEnabled = true;

                sss.Settings.DomStorageEnabled = true;
                sss.Settings.LoadWithOverviewMode = true;
                sss.Settings.UseWideViewPort = true;
                sss.Settings.BuiltInZoomControls = true;
                sss.Settings.DisplayZoomControls = false;
                sss.Settings.SetSupportZoom(true);
                sss.Settings.JavaScriptEnabled = true;
                sss.SetWebViewClient(new LocalWebViewClient());
                var Viewww = new UtilityJavascriptInterface(mContext, sss);
                sss.AddJavascriptInterface(Viewww, "Android");
                var CAt = "https://geolocale.azurewebsites.net/CarppiDriversDaschBoard/Index?Hash_ID=" + query.ProfileId;
                sss.LoadUrl(CAt);

                alert.SetView(sss);
                //      var webi_alert = c_view.FindViewById<WebView>(Resource.Id.OportinisticwebView_);


                alert.SetPositiveButton("Cerrar", (senderAlert, args) =>
                {
                    //count++;
                    // button.Text = string.Format("{0} clicks!", count);
                });

                // alert.SetNegativeButton("Substract", (senderAlert, args) =>
                //  {
                // count--;
                // button.Text = string.Format("{0} clicks!", count);
                //  });



                //LoginManager.Instance.RegisterCallback(mFBCallManager, this);
                Dialog dialog = alert.Create();
                //  dialog.Window.fla
                dialog.Window.ClearFlags(WindowManagerFlags.NotFocusable | WindowManagerFlags.AltFocusableIm | WindowManagerFlags.LocalFocusMode);
                dialog.Window.SetSoftInputMode(SoftInput.StateVisible | SoftInput.StateAlwaysVisible);
                dialog.Show();

            };
            // Device.BeginInvokeOnMainThread(action_WhowAlert);
            ((Activity)mContext).RunOnUiThread(action_WhowAlert);
            */
        }



        [JavascriptInterface]
        [Export("RetriveProfile")]
        public static async void RetriveProfile()
        {
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
                    var RealRole = (enumRoles)Profile.Rol;
                    if(RealRole == enumRoles.Conductor)
                    {
                        var script = "Dismiss_BeADriver_Block();";
                        Action action = () =>
                        {
                            Static_Webi.EvaluateJavascript(script, null);

                        };
                        Static_Webi.Post(action);
                    }
                    if(RealRole == enumRoles.Conductor && Profile.StripeDriverID == null)
                    {
                        var script = "ShowAddClabe();";
                        Action action = () =>
                        {
                            Static_Webi.EvaluateJavascript(script, null);

                        };
                        Static_Webi.Post(action);
                    }
                    if(RealRole == enumRoles.Conductor && Profile.StripeDriverID != null)
                    {
                        var script = "ShowBalanceBlock();";
                        Action action = () =>
                        {
                            Static_Webi.EvaluateJavascript(script, null);

                        };
                        Static_Webi.Post(action);
                    }

                    //  MainActivity.LoadFragmentStatic(Resource.Id.menu_video);


                }
            }
            catch (System.Exception) { }
        }

        public partial class Traveler_Perfil
        {
            public int ID { get; set; }
            public string Nombre_usuario { get; set; }
            public string Numero_de_telefono { get; set; }
            public string Correo { get; set; }
            public string Resenas { get; set; }
            public string Edad { get; set; }
            public bool? Telefono_verificado { get; set; }
            public string Stripe_id { get; set; }
            public string Facebook_profile_id { get; set; }
            public int? Rol { get; set; }
            public string Viaje_asociado { get; set; }
            public System.DateTime? Fecha { get; set; }
            public string Latitud { get; set; }
            public string Longitud { get; set; }
            public int? pidiendo_viaje { get; set; }
            public string Latitud_un_decimal { get; set; }
            public string Longitud_un_decimal { get; set; }
            public bool? TarjetaValidada { get; set; }
            public bool? IdentidadComprobada { get; set; }
            public string Telefono { get; set; }
            public string Foto { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string FirebaseID { get; set; }
            public string StripeClientID { get; set; }
            public string StripeDriverID { get; set; }
            public string BusquedaGuardada_Latitud_salida { get; set; }
            public string BusquedaGuardada_Longitud_salida { get; set; }
            public string BusquedaGuardada_Latitud_Llegada { get; set; }
            public string BusquedaGuardada_Longitud_Llegada { get; set; }
            public int Rating { get; set; }
            public double Rating_double { get; set; }
            public bool IsUserADriver { get; set; }

        }

        //AddProviderTOBussined
        [JavascriptInterface]
        [Export("AddProviderTOBussined")]
        public async void AddProviderTOBussined()
        {/*

            Action action_WhowAlert = () =>
            {
                AlertDialog.Builder alert = new AlertDialog.Builder(mContext);
                //alertToShow.getWindow().setSoftInputMode(
                //WindowManager.LayoutParams.SOFT_INPUT_STATE_ALWAYS_VISIBLE);
                //alert.
                //alert.SetTitle("Login");
                // alert.SetMessage("Do you want to add or substract?");
                // var c_view = (Fragment1.act).LayoutInflater.Inflate(Resource.Layout.OportinisticWebViiew, null);
                // WebView sss = new WebView(mContext);
                LocalWebView sss = new LocalWebView(mContext);
                sss.Settings.JavaScriptEnabled = true;

                sss.Settings.DomStorageEnabled = true;
                sss.Settings.LoadWithOverviewMode = true;
                sss.Settings.UseWideViewPort = true;
                sss.Settings.BuiltInZoomControls = true;
                sss.Settings.DisplayZoomControls = false;
                sss.Settings.SetSupportZoom(true);
                sss.Settings.JavaScriptEnabled = true;
                sss.SetWebViewClient(new LocalWebViewClient());
                var Viewww = new UtilityJavascriptInterfaceProfile(mContext, sss);
                sss.AddJavascriptInterface(Viewww, "Android");
                sss.LoadUrl("https://dashboard.stripe.com/express/oauth/authorize?response_type=code&client_id=ca_GrXXNCPXj12lpfIP3tEiUl2BaZGsKUKc&scope=read_write");

                alert.SetView(sss);
                //      var webi_alert = c_view.FindViewById<WebView>(Resource.Id.OportinisticwebView_);


                alert.SetPositiveButton("Cerrar", (senderAlert, args) =>
                {
                    //count++;
                    // button.Text = string.Format("{0} clicks!", count);
                });

                // alert.SetNegativeButton("Substract", (senderAlert, args) =>
                //  {
                // count--;
                // button.Text = string.Format("{0} clicks!", count);
                //  });



                //LoginManager.Instance.RegisterCallback(mFBCallManager, this);
                Dialog dialog = alert.Create();
                //  dialog.Window.fla
                dialog.Window.ClearFlags(WindowManagerFlags.NotFocusable | WindowManagerFlags.AltFocusableIm | WindowManagerFlags.LocalFocusMode);
                dialog.Window.SetSoftInputMode(SoftInput.StateVisible | SoftInput.StateAlwaysVisible);
                dialog.Show();

            };
            // Device.BeginInvokeOnMainThread(action_WhowAlert);
            ((Activity)mContext).RunOnUiThread(action_WhowAlert);
            */
        }



        //ChangeToDriverMode
        [JavascriptInterface]
        [Export("ChangeToDriverMode")]
        public async void ChangeToDriverMode()
        {
            Action action = () =>
            {
                AlertDialog.Builder alert = new AlertDialog.Builder(mContext);
                alert.SetTitle("Advertencia!");
                alert.SetMessage("En la version beta de la app, lo unico que tienes que hacer para ser conductor es"+
                    " aceptar en esta ventana e introducir tu clabe (para que te depositemos), en el futuro, pediremos "
                    + "que valides tu identidad");

                alert.SetPositiveButton("Aceptar",async (senderAlert, args) =>
                {
                    try
                    {
                        HttpClient client = new HttpClient();
                        //Post_Travel(string Argument, string FaceId, string Vehiculo, string Costo)
                        var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
                        var db5 = new SQLiteConnection(databasePath5);
                        var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID == 1).FirstOrDefault();

                        var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/TravelerCrossCityApi/ChangeRoleOfUser?" +
                            "user11_Hijo=" + query.ProfileId + //VistaHTMLProffesores.Grupo_Activo +
                            "&NewRole=" + ((int)enumRoles.Conductor).ToString()
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


                          //  MainActivity.LoadFragmentStatic(Resource.Id.nav_messages);


                        }
                    }
                    catch (System.Exception) { }

                });

                alert.SetNegativeButton("Cerrar", (senderAlert, args) =>
                {
                   
                });

                Dialog dialog = alert.Create();
                dialog.Show();
            };
            ((Activity)mContext).RunOnUiThread(action);
        }



        //DisplayMyStats
        [JavascriptInterface]
        [Export("DisplayMyStats")]
        public async void DisplayMyStats()
        {
            Action action = () =>
            {
                AlertDialog.Builder alert = new AlertDialog.Builder(mContext);
                alert.SetTitle("Error");
                alert.SetMessage("Las estadisticas no estan disponibles durante la version beta :c");

                //alert.SetPositiveButton("Añadir", (senderAlert, args) =>
                //{
                   
                //});

                alert.SetNegativeButton("Cerrar", (senderAlert, args) =>
                {
                    //  count--;
                    //  button.Text = string.Format("{0} clicks!", count);
                });

                Dialog dialog = alert.Create();
                dialog.Show();
            };
            ((Activity)mContext).RunOnUiThread(action);
        }

        [JavascriptInterface]
        [Export("ShowFacebookLogButton")]
        public static async void ShowFacebookLogButton()
        {

            Action action = () =>
            {
                FacebookSdk.SdkInitialize(Static_mContext);
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
                AlertDialog.Builder alert = new AlertDialog.Builder(Static_mContext);
                alert.SetTitle("Login");
                // alert.SetMessage("Do you want to add or substract?");
                var c_view = ((Activity)Static_mContext).LayoutInflater.Inflate(Resource.Layout.LoginLayout, null);
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
                BtnFBLogin.RegisterCallback(mFBCallManager, facebookCallback_Static);

                mprofileTracker = new MyProfileTracker();
                mprofileTracker.mOnProfileChanged += mProfileTracker_mOnProfileChanged;
                mprofileTracker.StartTracking();


                Dialog dialog = alert.Create();

                //dialog.SetOnShowListener(new S_listen(dialog, (AlertDialog)dialog, TextTag, c_view, mContext));
                dialog.Show();
            };
            ((Activity)Static_mContext).RunOnUiThread(action);
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

            ((Activity)Static_mContext).RunOnUiThread(action);

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
                    "&LastName=" + query.LastName
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





    }
    public class MyProfileTracker : ProfileTracker
    {
        public event EventHandler<OnProfileChangedEventArgs> mOnProfileChanged;

        protected override void OnCurrentProfileChanged(Profile oldProfile, Profile newProfile)
        {
            if (mOnProfileChanged != null)
            {
                mOnProfileChanged.Invoke(this, new OnProfileChangedEventArgs(newProfile));
            }
        }
    }
    public class OnProfileChangedEventArgs : EventArgs
    {
        public Profile mProfile;

        public OnProfileChangedEventArgs(Profile profile) { mProfile = profile; }
    }
    public class UtilityJavascriptInterfaceProfile : Java.Lang.Object
    {
        Context mContext;
        WebView webi;
        public UtilityJavascriptInterfaceProfile(Context Act, WebView web)
        {
            mContext = Act;
            webi = web;
        }
        [JavascriptInterface]
        [Export("UpdateConectedIDStripe")]
        public async void UpdateConectedIDStripe(string ConnectedAccoundID)
        {
            Action action = async () =>
            {
                var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
                var db5 = new SQLiteConnection(databasePath5);
                var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID == 1).FirstOrDefault();


                HttpClient client = new HttpClient();

                var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/TravelerCrossCityApi/UpdateStripeTutorID?" +
                    "Tutori_participantToUpdate=" + query.ProfileId
                    + "&StripeComercianteID=" + ConnectedAccoundID
                    ));
                HttpResponseMessage response;

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                response = await client.GetAsync(uri);

                if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                {
                  //  MainActivity.LoadFragmentStatic(Resource.Id.nav_messages);


                   
                }
            };
            ((Activity)mContext).RunOnUiThread(action);


        }


    }

}
