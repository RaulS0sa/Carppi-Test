using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using SQLite;

namespace Carppi_Cliente.Clases
{
    class Location
    {

        public static async Task<Position> GetCurrentPosition()
        {
            Position position = null;
            try
            {
                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 1;

                position = await locator.GetLastKnownLocationAsync();

                if (position != null)
                {
                    //got a cahched position, so let's use it.
                    return position;
                }

                if (!locator.IsGeolocationAvailable || !locator.IsGeolocationEnabled)
                {
                    //not available or enabled
                    return null;
                }

                position = await locator.GetPositionAsync(TimeSpan.FromSeconds(1), null, true);

            }
            catch (Exception ex)
            {
              //  Log.Debug("Out", "Fucks: " + ex);
                // Debug.WriteLine("Unable to get location: " + ex);
            }

            if (position == null)
                return null;

            var output = string.Format("Time: {0} \nLat: {1} \nLong: {2} \nAltitude: {3} \nAltitude Accuracy: {4} \nAccuracy: {5} \nHeading: {6} \nSpeed: {7}",
                    position.Timestamp, position.Latitude, position.Longitude,
                    position.Altitude, position.AltitudeAccuracy, position.Accuracy, position.Heading, position.Speed);

            // Debug.WriteLine(output);
           // Log.Debug("Output", output);

            return position;
        }

        public static async Task StartListening()
        {
            if (CrossGeolocator.Current.IsListening)
                return;

            await CrossGeolocator.Current.StartListeningAsync(TimeSpan.FromSeconds(5), 10, true);

            CrossGeolocator.Current.PositionChanged += PositionChanged;
            // CrossGeolocator.Current.PositionError += PositionError;
        }

        public static async void PositionChanged(object sender, PositionEventArgs e)
        {

            //If updating the UI, ensure you invoke on main thread
            var position = e.Position;
            var output = "Full: Lat: " + position.Latitude + " Long: " + position.Longitude;
            output += "\n" + $"Time: {position.Timestamp}";
            output += "\n" + $"Heading: {position.Heading}";
            output += "\n" + $"Speed: {position.Speed}";
            output += "\n" + $"Accuracy: {position.Accuracy}";
            output += "\n" + $"Altitude: {position.Altitude}";
            output += "\n" + $"Altitude Accuracy: {position.AltitudeAccuracy}";
            //var placemarks = await Geocoding.GetPlacemarksAsync(position.Latitude, position.Longitude);

            //var placemark = placemarks?.FirstOrDefault();
            //if (placemark != null)
            //{
            //    var geocodeAddress =
            //        $"AdminArea:       {placemark.AdminArea}\n" +
            //        $"CountryCode:     {placemark.CountryCode}\n" +
            //        $"CountryName:     {placemark.CountryName}\n" +
            //        $"FeatureName:     {placemark.FeatureName}\n" +
            //        $"Locality:        {placemark.Locality}\n" +
            //        $"PostalCode:      {placemark.PostalCode}\n" +
            //        $"SubAdminArea:    {placemark.SubAdminArea}\n" +
            //        $"SubLocality:     {placemark.SubLocality}\n" +
            //        $"SubThoroughfare: {placemark.SubThoroughfare}\n" +
            //        $"Thoroughfare:    {placemark.Thoroughfare}\n";

            //    Console.WriteLine(geocodeAddress);
            //}

            UpdateLocation();
        }

        public static async void UpdateLocation()
        {
            try
            {
                var MyLatLong = await Clases.Location.GetCurrentPosition();
                //WhereoGo.LatitudeOrigen = Loc.Latitude;
                //WhereoGo.LongitudOrigen = Loc.Longitude;

                HttpClient client = new HttpClient();
                //Post_Travel(string Argument, string FaceId, string Vehiculo, string Costo)
                var databasePath5 = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Log_info_user.db");
                var db5 = new SQLiteConnection(databasePath5);
                var query = db5.Table<DatabaseTypes.Log_info>().Where(v => v.ID == 1).FirstOrDefault();

                var uri = new Uri(string.Format("https://geolocale.azurewebsites.net/api/TravelerCrossCityApi/ActualizaLocacion?" +
                    "user5=" + query.ProfileId
                    + "&Latitud=" + MyLatLong.Latitude
                     + "&Longitud=" + MyLatLong.Longitude

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
                    //  Toast.MakeText(mContext, "Sesion Fiinalizada", ToastLength.Long).Show();
                }
            }
            catch (Exception)
            {

            }

        }


    }
}
