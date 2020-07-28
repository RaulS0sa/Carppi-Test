using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using SQLite;
using Xamarin.Essentials;

namespace Carppi.Clases
{
    public class BatteryTest
    {
        public BatteryTest()
        {
            // Register for battery changes, be sure to unsubscribe when needed
            Battery.BatteryInfoChanged += Battery_BatteryInfoChanged;
        }

        void Battery_BatteryInfoChanged(object sender, BatteryInfoChangedEventArgs e)
        {
            var level = e.ChargeLevel;
            var state = e.State;
            var source = e.PowerSource;
            if(level< 0.15 && (state == BatteryState.NotCharging || state == BatteryState.Discharging))
            {
                DispatchifUnloaded();

            }
           // Console.WriteLine($"Reading: Level: {level}, State: {state}, Source: {source}");
        }
        public enum WorkState { StopWork, StartWork };
        public void DispatchifUnloaded()
        {

            HttpClient client = new HttpClient();

            string FaceID = null;
            var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
            var db5 = new SQLiteConnection(databasePath5);
            //Post_Travel(string Argument, string FaceId, string Vehiculo, string Costo)
            try
            {

                var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID > 0).FirstOrDefault();
                FaceID = query.ProfileId;
            }
            catch (Exception ex)
            {

            }
            //
            //public HttpResponseMessage SetOrderStatus(string FaceIDHash_DeliveryBoy, GroceryOrderState Estado, Int32 OrderID)
            var uri = new Uri(string.Format("http://geolocale.azurewebsites.net/api/CarppiRepartidorApi/ChangeDeliverymanJourney?" +
                "FaceIDHash_Deliveryman=" + FaceID +
                "&workState=" + (int)WorkState.StopWork

                ));
            // HttpResponseMessage response;

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //  var  response =  client.GetAsync(uri).Result;
            var t = Task.Run(() => GetResponseFromURI(uri));
            t.Wait();
            var S_Ressult = t.Result;
            if (S_Ressult.httpStatusCode == System.Net.HttpStatusCode.Accepted)
            {

            }
            else
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

    }
}
