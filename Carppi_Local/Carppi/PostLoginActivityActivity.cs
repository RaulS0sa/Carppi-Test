
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Carppi.Fragments;
using Firebase.Iid;
using SQLite;
using Xamarin.Essentials;

namespace Carppi
{
    [Activity(Label = "PostLoginActivityActivity")]
    public class PostLoginActivityActivity : Activity
    {
        public static byte[] EcommercePhoto { get; set; }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.PostLoginLayout);
            GenerateDatabase();
            var Spiner = FindViewById<Spinner>(Resource.Id.planets_spinner);
            var arrayAdapter = ArrayAdapter.CreateFromResource(this, Resource.Array.planets_array, Android.Resource.Layout.SimpleSpinnerItem);
            arrayAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            Spiner.Adapter = arrayAdapter;



            var CategorySpiner = FindViewById<Spinner>(Resource.Id.Food_spinner);
            var arrayAdapterCategory = ArrayAdapter.CreateFromResource(this, Resource.Array.TypesOfFood, Android.Resource.Layout.SimpleSpinnerItem);
            arrayAdapterCategory.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            CategorySpiner.Adapter = arrayAdapterCategory;

            var button = FindViewById<Button>(Resource.Id.NextStep_PostLoginLayout);
            button.Click += Button_Click;

            FindViewById<Button>(Resource.Id.IageOfComerceButton).Click += ImageOfComerceButton;

            // Create your application here
            //IageOfComerceButton -checked
            //Supaname

            //SupaMail

            //planets_spinner
            //CateoryLisPostLogi
            //IageOfComerceButton
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if ((requestCode == (int)WebInterfaceAddProduct.ActivityIntentREturns.SelectEcomercePicture) && (resultCode == Result.Ok) && (data != null))
            {
                Android.Net.Uri uri = data.Data;
                Stream iStream = ContentResolver.OpenInputStream(uri);
                byte[] inputData = getBytes(iStream);
                PostLoginActivityActivity.EcommercePhoto = inputData;
                //storeicon
                var image = FindViewById<ImageView>(Resource.Id.EcommerceImage);
                /*
                Android.Graphics.Bitmap bmp;
                using (var ms = new MemoryStream(inputData))
                {
                    var aadlksd = new Image.FromStream(ms)
                    bmp = new Android.Graphics.Bitmap.(ms);
                }
                bmp.Height = image.Height;
                bmp.Width = image.Width;
                */

                image.SetImageURI(uri);
                //WebInterfaceAddProduct.Photo = inputData;
                //WebInterfaceAddProduct.UpdateBottonOfImageSelect(getFileName(uri));
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

        private void ImageOfComerceButton(object sender, EventArgs e)
        {
            var Intent = new Intent();
            Intent.SetType("image/*");
            Intent.SetAction(Intent.ActionGetContent);
            
            ((Activity)this).StartActivityForResult(Intent.CreateChooser(Intent, "Select Picture"), (int)WebInterfaceAddProduct.ActivityIntentREturns.SelectEcomercePicture);

            //throw new NotImplementedException();
        }

        private void Button_Click(object sender, EventArgs e)
        {
            var button_Kwaii = FindViewById<Button>(Resource.Id.NextStep_PostLoginLayout);
            button_Kwaii.Enabled = false;
            var databasePath10 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "RestaurantLogData.db");
            var db10 = new SQLiteConnection(databasePath10);
            var query = db10.Table<DatabaseTypes.RestauratLoginTypes>().Where(v => v.ID > 0).FirstOrDefault();

            var EcomerceName = FindViewById<EditText>(Resource.Id.Supaname).Text;
            var EcomerceMail = FindViewById<EditText>(Resource.Id.SupaMail).Text;
            var SpinerSelecction = FindViewById<Spinner>(Resource.Id.planets_spinner).SelectedItemPosition;
            var Categorias = FindViewById<EditText>(Resource.Id.CateoryLisPostLogi).Text;
            // query.FacebookId = e.mProfile.Id;
            var Foodelecction = FindViewById<Spinner>(Resource.Id.Food_spinner).SelectedItemPosition;


            query.Nombre = EcomerceName;
            query.Correo = EcomerceMail;
            query.TypeOfStore = SpinerSelecction;
            query.Categorias = Categorias;
            
            query.Foto = PostLoginActivityActivity.EcommercePhoto;
            query.TypeOfFood = Foodelecction;

            db10.RunInTransaction(() =>
            {
                db10.Update(query);
            });
            if (EcommercePhoto == null || EcomerceName == "" || EcomerceMail == "" || Categorias == "")
            {
                AlertDialog.Builder alert = new AlertDialog.Builder(this);
                alert.SetTitle("Error");
                alert.SetMessage("No estan todos los datos");



                alert.SetNegativeButton("Aceptar", (senderAlert, args) =>
                {

                    //  count--;
                    //  button.Text = string.Format("{0} clicks!", count);
                });

                Dialog dialog = alert.Create();
                dialog.Show();
            }
            else
            {


                AddRestaurant();
                //var intento = new Intent(this, typeof(ValidateAccountActivity));
                //StartActivity(intento);
            }
           
        }

        public void GenerateDatabase()
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
                        Pais = "Mexico"
                        
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

                    query.Pais = "Mexico";
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
        public static string SuperToken { get; set; }
        async Task AddRestaurant()
        {
            var GlobalRhasophrase = "";
            var GlobalErrorResponse = "";
            var EncodedQuery = "";
            try
            {
                FirebaseInstanceId.Instance.GetInstanceId().AddOnSuccessListener(new CompleteLestener());
                var MyLatLong = await Clases.Location.GetCurrentPosition();
                // var placemarks = await Geocoding.GetPlacemarksAsync(MyLatLong.Latitude, MyLatLong.Longitude);

                var placemark = GEtAllMyPlaceMarkers().Result;//placemarks?.FirstOrDefault();

                var databasePath10 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "RestaurantLogData.db");
                var db10 = new SQLiteConnection(databasePath10);
                var query = db10.Table<DatabaseTypes.RestauratLoginTypes>().Where(v => v.ID > 0).FirstOrDefault();

                HttpClient Client = new HttpClient();
                var TokeNData = SuperToken == null ? query.FirebaseID : SuperToken;
                Uri webService = new Uri("https://geolocale.azurewebsites.net/api/CarppiRestaurantApi/AddNewRestaurant?" +
                    "RegionID=" + 2
                    + "&Latitud=" + MyLatLong.Latitude.ToString().Replace(",", ".")
                    + "&Longitud=" + MyLatLong.Longitude.ToString().Replace(",", ".")
                    + "&TipoDeTienda=" + query.TypeOfStore
                    + "&FaceID=" + query.FacebookId
                    + "&NombreDelNegocio=" + query.Nombre
                    + "&CiudadArg=" + placemark.Locality
                    + "&EstadoArg=" + placemark.AdminArea
                    + "&PaisArg=" + placemark.CountryName
                    + "&FirebaseHash="+ TokeNData  //query.FirebaseID
                    + "&Correo=" + query.Correo
                    + "&contrasena=" + "none"
                    + "&Categorias=" + query.Categorias
                    + "&FoodCategory=" + query.TypeOfFood



                    );

                EncodedQuery = webService.ToString();
                using (var content = new MultipartFormDataContent())
                {

                    //HttpContent metaDataContent = new ByteArrayContent(File);
                    content.Add(CreateFileContent(new MemoryStream(query.Foto), "image.jpg", "image/jpeg"));
                     using (var message = await Client.PostAsync(webService, content))
                    {
                        Console.WriteLine(message.ReasonPhrase.ToLower());

                        var respuesra = message.Content.ReadAsStringAsync().Result;
                        GlobalErrorResponse = respuesra;
                        GlobalRhasophrase = message.ReasonPhrase.ToLower();
                        TrowError(GlobalErrorResponse, EncodedQuery, GlobalRhasophrase);
                        var button_Kwaii = FindViewById<Button>(Resource.Id.NextStep_PostLoginLayout);
                        button_Kwaii.Enabled = true;
                        if (message.ReasonPhrase.ToLower() == "Created".ToLower())
                        {
                            //await AddFilesToHomeWork(Convert.ToInt32(message.Content.ReadAsStringAsync().Result));
                            query.CarppiHash = Regex.Unescape(respuesra.Replace("\"", "")); 
                            query.RegistroValidado = true;

                            db10.RunInTransaction(() =>
                            {
                                db10.Update(query);
                                //db5.(query);
                            });
                            db10.RunInTransaction( async() =>
                            {
                                db10.Update(query);
                                //db5.(query);
                            });
                            var intento = new Intent(this, typeof(ValidateAccountActivity));
                            StartActivity(intento);
                            content.Dispose();
                        }
                        else if(message.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            query.CarppiHash = Regex.Unescape(respuesra.Replace("\"", ""));
                            query.RegistroValidado = true;

                            db10.RunInTransaction(() =>
                            {
                                db10.Update(query);
                            });

                            db10.RunInTransaction(async () =>
                            {
                                db10.Update(query);
                            });
                            var intento = new Intent(this, typeof(ValidateAccountActivity));
                            StartActivity(intento);
                            content.Dispose();
                        }
                    }

                  
                }
            }
            catch (System.Exception e)
            {

                //https://geolocale.azurewebsites.net/api/CarppiRestaurantApi/LogError?ErrorResponse=X&EncodedQueryString=Yolo&HttpError=Swog&facebookID=llll
                var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "RestaurantLogData.db");
                var db5 = new SQLiteConnection(databasePath5);
                var query = db5.Table<DatabaseTypes.RestauratLoginTypes>().Where(v => v.ID > 0).FirstOrDefault();
                //var HAsh = Regex.Unescape(query.CarppiHash.Replace("\"", ""));



                HttpClient client = new HttpClient();

                var uri = new Uri(string.Format("https://geolocale.azurewebsites.net/api/CarppiRestaurantApi/LogError?" +
                    "ErrorResponse=" + GlobalErrorResponse + "&EncodedQueryString=" + Base64Encode(EncodedQuery) + "&HttpError=" + GlobalRhasophrase + "&facebookID=" + query.FacebookId + "   " + e.ToString()
                    ));


                var t = Task.Run(() => GetResponseFromURI(uri));
                t.Wait();
                var S_Ressult = t.Result;
                Toast.MakeText(this, S_Ressult.httpStatusCode.ToString(), ToastLength.Long).Show();
                //StopFetcherButton();
                //Console.WriteLine(e.ToString());
            }
            
        }


        async void TrowError(string GlobalErrorResponse , string Query ,string  GlobalRhasophrase)
        {
            try
            {
                //https://geolocale.azurewebsites.net/api/CarppiRestaurantApi/LogError?ErrorResponse=X&EncodedQueryString=Yolo&HttpError=Swog&facebookID=llll
                var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "RestaurantLogData.db");
                var db5 = new SQLiteConnection(databasePath5);
                var query = db5.Table<DatabaseTypes.RestauratLoginTypes>().Where(v => v.ID > 0).FirstOrDefault();
                //var HAsh = Regex.Unescape(query.CarppiHash.Replace("\"", ""));



                HttpClient client = new HttpClient();

                var uri = new Uri(string.Format("https://geolocale.azurewebsites.net/api/CarppiRestaurantApi/LogError?" +
                    "ErrorResponse=" + GlobalErrorResponse + "&EncodedQueryString=" + Base64Encode(Query) + "&HttpError=" + GlobalRhasophrase + "&facebookID=" + query.FacebookId
                    ));


                var t = Task.Run(() => GetResponseFromURI(uri));
                t.Wait();
                var S_Ressult = t.Result;
                Toast.MakeText(this, S_Ressult.httpStatusCode.ToString(), ToastLength.Long).Show();
            }
            catch(Exception)
            {

            }
        }
        public async Task<Placemark> GEtAllMyPlaceMarkers()
        {
            try
            {
                var Place = new Placemark();
                Place.Locality = "Tulancingo de Bravo";
                Place.AdminArea = "Hidalgo";
                Place.CountryName = "México";
                return Place;
                /*
                var MyLatLong = await Clases.Location.GetCurrentPosition();
                var placemarks = await Geocoding.GetPlacemarksAsync(MyLatLong.Latitude, MyLatLong.Longitude);

                var placemark = placemarks?.FirstOrDefault();
                return placemark;
                */
            }
            catch(Exception ex)
            {
                var Place = new Placemark();
                Place.Locality = "Tulancingo de Bravo";
                Place.AdminArea = "Hidalgo";
                Place.CountryName = "México";
                return Place;
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
        public static string Base64Encode(string plainText)
        {
            try
            {
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
                return System.Convert.ToBase64String(plainTextBytes);
            }
            catch(Exception ex)
            {
                return "";
            }
        }
        public class UriResponse
        {
            public String Response;
            public System.Net.HttpStatusCode httpStatusCode;
        }
        private StreamContent CreateFileContent(Stream stream, string fileName, string contentType)
        {
            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "\"files\"",
                FileName = "\"" + fileName + "\""
            }; // the extra quotes are key here
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            return fileContent;
        }
    }
    class CompleteLestener : Java.Lang.Object, Android.Gms.Tasks.IOnSuccessListener
    {
       

        public void OnSuccess(Java.Lang.Object result)
        {
            var token = result.Class.GetMethod("getToken").Invoke(result).ToString();
            PostLoginActivityActivity.SuperToken = token;
        }
    }
}
