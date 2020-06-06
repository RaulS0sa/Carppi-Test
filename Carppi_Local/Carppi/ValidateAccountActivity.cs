
using System;
using System.Collections.Generic;
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
using SQLite;

namespace Carppi
{
    [Activity(Label = "ValidateAccountActivity")]
    public class ValidateAccountActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ValidateMailLayout);
            var botonFinal = FindViewById<Button>(Resource.Id.FinishHimButton);
            botonFinal.Click += BotonFinal_Click;

            FindViewById<Button>(Resource.Id.RegrasarPstLogin).Click += ComebackKid_TotheLastPage;

            // Create your application here
        }

        private void ComebackKid_TotheLastPage(object sender, EventArgs e)
        {
            var intento = new Intent(this, typeof(PostLoginActivityActivity));
            StartActivity(intento);
            // throw new NotImplementedException();
        }

        private void BotonFinal_Click(object sender, EventArgs e)
        {
            var validationNumber = FindViewById<EditText>(Resource.Id.ValidtioNumber).Text;
            if (validationNumber == "")
            {
            }
            else
            {
                try
                {



                    HttpClient client = new HttpClient();

                    string FaceID = null;
                    var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "RestaurantLogData.db");
                    var db5 = new SQLiteConnection(databasePath5);
                    var HAsh = "";
                    //Post_Travel(string Argument, string FaceId, string Vehiculo, string Costo)
                    var query = new DatabaseTypes.RestauratLoginTypes();
                    try
                    {

                         query = db5.Table<DatabaseTypes.RestauratLoginTypes>().Where(v => v.ID > 0).FirstOrDefault();
                         HAsh = Regex.Unescape(query.CarppiHash.Replace("\"", ""));
                         FaceID = query.FacebookId;

                    }
                    catch (Exception ex)
                    {

                    }
                    var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantApi/ValidateSecurityCode_login?" +
                       "LoginCode=" + validationNumber +
                       "&CarppiHashloginVerification=" + HAsh


                       ));
                    /*
                    var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantApi/ValidateSecurityCode_login?" +
                        "LoginCode=" + validationNumber +
                        "&CarppiHashloginVerification=" + HAsh


                        ));
                        */
                    /*
                var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRestaurantApi/ValidateSecurityCode_loginfacebook?" +
                   "LoginCode=" + validationNumber +
                   "&facebookhash=" + FaceID


                   ));
                */
                    //<!--ScrollableVegtableList-->
                    // HttpResponseMessage response;

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //  var  response =  client.GetAsync(uri).Result;
                    var t = Task.Run(() => GetResponseFromURI(uri));
                    t.Wait();
                    var respone = t.Result;
                    if(respone.httpStatusCode == System.Net.HttpStatusCode.Created)
                    {
                        query.VerificacionDecuenta = true; ;
                        query.CarppiHash = Regex.Unescape(respone.Response.Replace("\"", ""));

                        db5.RunInTransaction(() =>
                        {
                            db5.Update(query);
                        });
                        var intento = new Intent(this, typeof(MainActivity));
                        StartActivity(intento);

                    }
                    else if (respone.httpStatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        AlertDialog.Builder alert = new AlertDialog.Builder(this);
                        alert.SetTitle("Error");
                        alert.SetMessage("Clave Incorrecta");



                        alert.SetNegativeButton("Aceptar", (senderAlert, args) =>
                        {

                            //  count--;
                            //  button.Text = string.Format("{0} clicks!", count);
                        });

                        Dialog dialog = alert.Create();
                        dialog.Show();
                    }
                   
                   

                    // var rrr = JsonConvert.DeserializeObject<CarppiRegiones>(S_Ressult.Response);
                    //UpdateProductGrid(Data)

                }

                catch (Exception Ex)
                {

                }
              //  var intento = new Intent(this, typeof(RegularMenuActivity));
              //  StartActivity(intento);
            }
            //throw new NotImplementedException();
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

    }
}
