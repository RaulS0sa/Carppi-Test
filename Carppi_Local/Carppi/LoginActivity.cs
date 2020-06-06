
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Carppi.Fragments;
using SQLite;
using Xamarin.Facebook;
using Xamarin.Facebook.Login.Widget;

namespace Carppi
{
    [Activity(Label = "LoginActivity")]
    public class LoginActivity : Activity
    {
        public static LoginButton BtnFBLogin { get; private set; }
        public static ICallbackManager mFBCallManager;
        private static MyProfileTracker mprofileTracker;
        static LoginActivity CurrentActivity { get; set; }

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            FacebookSdk.SdkInitialize(this);
            CurrentActivity = this;
            AccessToken.RefreshCurrentAccessTokenAsync();
            //CrossCurrentActivity.Current.Activity = this;
            Android.Support.V4.App.ActivityCompat.RequestPermissions(this, new System.String[] { Manifest.Permission.AccessFineLocation, Manifest.Permission.AccessCoarseLocation, Manifest.Permission.LocationHardware, Manifest.Permission.Internet, }, 100);
            //var status = await CrossPermissions.Current.RequestPermissionAsync<LocationPermission>();
            //var status2 = await CrossPermissions.Current.RequestPermissionAsync<LocationWhenInUsePermission>();


            SetContentView(Resource.Layout.LoginLayout);


            BtnFBLogin = FindViewById<LoginButton>(Resource.Id.fblogin);

            var LogText = FindViewById<TextView>(Resource.Id.link_signup);
            LogText.Click += LogText_Click;

            BtnFBLogin.SetPermissions(new List<string> { "public_profile", "email" });
            //  BtnFBLogin.Fragment = Fragment_profile.act_fragment;


            mFBCallManager = CallbackManagerFactory.Create();
            var facebookCallback_Static = new facebookCallback();
            BtnFBLogin.RegisterCallback(mFBCallManager, facebookCallback_Static);

            mprofileTracker = new MyProfileTracker();
            mprofileTracker.mOnProfileChanged += mProfileTracker_mOnProfileChanged;
            mprofileTracker.StartTracking();

            // Create your application here
        }

        private void LogText_Click(object sender, EventArgs e)
        {
            Intent i = new Intent(this, typeof(PostLoginActivityActivity));
            StartActivity(i);
            //throw new NotImplementedException();
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Android.Content.Intent data)
        {

            base.OnActivityResult(requestCode, resultCode, data);

            // mFBCallManager.OnActivityResult(requestCode, (int)resultCode, data);
            mFBCallManager.OnActivityResult(requestCode, (int)resultCode, data);
            if(resultCode == Result.Ok)
            {
                Intent i = new Intent(this, typeof(PostLoginActivityActivity));
                StartActivity(i);
            }
        }

        public static async void mProfileTracker_mOnProfileChanged(object sender, OnProfileChangedEventArgs e)
        {

          

                if (e.mProfile != null)
                {
                    try
                    {
                    
                       var databasePath10 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "RestaurantLogData.db");
                       var db10 = new SQLiteConnection(databasePath10);
                       db10.CreateTable<DatabaseTypes.RestauratLoginTypes>();



                        var query = db10.Table<DatabaseTypes.RestauratLoginTypes>().Where(v => v.ID > 0).FirstOrDefault();
                        if (query == null)
                        {


                            var s = db10.Insert(new DatabaseTypes.RestauratLoginTypes()
                            {
                              
                                FacebookId = e.mProfile.Id
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
                          
                            query.FacebookId = e.mProfile.Id;
                            db10.RunInTransaction(() =>
                            {
                                db10.Update(query);
                            });
                        }
                        

                        try
                        {
                           // await LoggInWebTask();
                        }
                        catch (System.Exception) { }

                       // MainActivity.LoadFragmentStatic(Resource.Id.nav_GroceryRequest);

                          //Intent i = new Intent(this, typeof(Activity1));
                    //      Intent i = new Intent(CurrentActivity.ApplicationContext, typeof(PostLoginActivityActivity));
                    //i.SetFlags(ActivityFlags.NewTask);
                      //    CurrentActivity.ApplicationContext.StartActivity(i);
                    

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
                        // xLoginManager.getInstance().logOut();
                        var databasePath10_ = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "RestaurantLogData.db");
                        var db10_ = new SQLiteConnection(databasePath10_);

                        db10_.RunInTransaction(() =>
                        {
                            //  db10_.DropTable<DatabaseTypes.Log_info>();
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
    }
}
