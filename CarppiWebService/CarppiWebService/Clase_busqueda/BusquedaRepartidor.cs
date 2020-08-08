using CarppiWebService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarppiWebService.Clase_busqueda
{
    public class BusquedaRepartidor
    {
        PidgeonEntities db = new PidgeonEntities();
        public enum GroceryOrderState { RequestCreated, RequestBeingAttended, RequestAccepted, RequestGoingToClient, RequestEnded, RequestRejected };

        public CarppiGrocery_Repartidores Repartidor = new CarppiGrocery_Repartidores();
        public double LatAtribute = 0.0;
        public double LongAtribute = 0.0;
        public double AreaOfServiceAtribute = 0.0;
        public bool DriverHasToomuchOrders = false;
        public BusquedaRepartidor(double LatPass, double LongPass, long AreaOfService)
        {
            LatAtribute = LatPass;
            LongAtribute = LongPass;
            AreaOfServiceAtribute = AreaOfService;
            // Repartidor = requestNearesDriver();

        }

        public long GetUsageRegion()
        {
            //double TemporalLatitude = Convert.ToDouble(x.Latitud);
            // IEnumerable<double> result =sequence.Where(x => x.HasValue).Select(x => x.Value);
          var RegionenelArea = db.CarppiGrocery_Regiones.Where(x => (Math.Pow(Math.Pow((x.Latitud) - LatAtribute, 2) + Math.Pow((x.Longitud) - LongAtribute, 2), 0.5) / 0.00909) < 15).FirstOrDefault();
            
            double? templong = (double?)LongAtribute;
           // var RegionenelArea2 = db.CarppiGrocery_Regiones.Where(x => (Math.Pow(Math.Pow((x.Latitud) - LatAtribute, 2) + Math.Pow((x.Longitud) - templong, 2), 0.5) /0.00909   )  < 15);
            if (RegionenelArea == null)
            {
                return 0;
            }
            else
            {
                return RegionenelArea.ID;
            }

        }

        public double OperationCost(double LatPedido, double LongPedido)
        {
            var DistanceCustomerRestaurant = ManhattanDisane(LatAtribute, LongAtribute, LatPedido, LongPedido) * 1 / 0.00909;
            //var DistanceDeliverManRestaurant = ManhattanDisane(Convert.ToDouble(Repartidor_nuevo.Latitud), Convert.ToDouble(Repartidor_nuevo.Longitud), Convert.ToDouble(Orden.LatitudRestaurante), Convert.ToDouble(Orden.LongitudRestaurante)) * 1 / 0.00909;
            var Fee = FeeCalculator(DistanceCustomerRestaurant);
            return Fee;
        }

        public bool IsInThisRegion(double LatRequest, double LongRequest)
        {
            // var CircleSoround = GenerateCircle(0.0003, Convert.ToDouble(Request_Trip.LatitudViajePendiente), Convert.ToDouble(Request_Trip.LongitudViajePendiente));
            //            var IsInDeliverRegion = PointInPolygon(CircleSoround.X_Array, CircleSoround.Y_Array, Convert.ToDouble(Driver.Latitud), Convert.ToDouble(Driver.Longitud));

            var CircleSoround = GenerateCircle(0.0006, Convert.ToDouble(LatRequest), Convert.ToDouble(LongRequest));
            var IsInDeliverRegion = PointInPolygon(CircleSoround.X_Array, CircleSoround.Y_Array, Convert.ToDouble(LatAtribute), Convert.ToDouble(LongAtribute));

            return IsInDeliverRegion;

        }
        public CarppiGrocery_Repartidores SearchForNearestDeliveryBoy()
        {

            double Distance = 100000;

            var ListOfDrivers = db.CarppiGrocery_Repartidores.Where(x => x.Region == AreaOfServiceAtribute && x.IsAvailableForDeliver == true);
            if (ListOfDrivers == null)
            {
                return null;
            }

            CarppiGrocery_Repartidores Conductor = ListOfDrivers.FirstOrDefault();
            foreach (var Driver in ListOfDrivers)
            {
                var Temp_TOpassenger = ManhattanDisane(LatAtribute, LongAtribute, Convert.ToDouble(Driver.Latitud), Convert.ToDouble(Driver.Longitud));
                var Temp_TODestiny = 0.0;
                var buyOrders_notCloseTofinish = db.CarppiRestaurant_BuyOrders.Where(x => x.FaceIDRepartidor_RepartidorCadena == Driver.FaceID_Repartidor && x.Stat != (int)GroceryOrderState.RequestGoingToClient && x.Stat != (int)GroceryOrderState.RequestEnded);
                var buyOrders_CloseTofinish = db.CarppiRestaurant_BuyOrders.Where(x => x.FaceIDRepartidor_RepartidorCadena == Driver.FaceID_Repartidor && x.Stat != (int)GroceryOrderState.RequestGoingToClient && x.Stat != (int)GroceryOrderState.RequestEnded);
                var buyOrders_all = db.CarppiRestaurant_BuyOrders.Where(x => x.FaceIDRepartidor_RepartidorCadena == Driver.FaceID_Repartidor);

                foreach (var order in buyOrders_all)
                {
                    Temp_TODestiny += ManhattanDisane(Convert.ToDouble(Driver.Longitud), Convert.ToDouble(Driver.Latitud), Convert.ToDouble(order.LongitudPeticion), Convert.ToDouble(order.LatitudPeticion));
                }

                if ((Temp_TODestiny + Temp_TOpassenger) < Distance)
                {
                    Distance = (Temp_TODestiny + Temp_TOpassenger);
                    Conductor = Driver;
                }

            }
            //OrdenesTotales

            if (Conductor != null)
            {
                var OrdenesTotales = db.CarppiRestaurant_BuyOrders.Where(x => x.FaceIDRepartidor_RepartidorCadena == Conductor.FaceID_Repartidor && (x.Stat != (int)GroceryOrderState.RequestEnded && x.Stat != (int)GroceryOrderState.RequestRejected));
                if (OrdenesTotales != null)
                {
                    if (OrdenesTotales.Count() >= 4)
                    {
                        DriverHasToomuchOrders = true;
                    }
                }
            }
            //Repartidor = Conductor;
            return Conductor;
        }

        public CarppiGrocery_Repartidores requestNearesDriver()
        {
            double Distance = 100000;

            var ListOfDrivers = db.CarppiGrocery_Repartidores.Where(x => x.Region == AreaOfServiceAtribute && x.IsAvailableForDeliver == true);
            if (ListOfDrivers == null)
            {
                return null;
            }

            CarppiGrocery_Repartidores Conductor = ListOfDrivers.FirstOrDefault();
            foreach (var Driver in ListOfDrivers)
            {
                var Temp_TOpassenger = ManhattanDisane(LatAtribute, LongAtribute, Convert.ToDouble(Driver.Latitud), Convert.ToDouble(Driver.Longitud));
                var Temp_TODestiny = 0.0;
                var buyOrders_notCloseTofinish = db.CarppiGrocery_BuyOrders.Where(x => x.FaceIDRepartidor_RepartidorCadena == Driver.FaceID_Repartidor && x.Stat != (int)GroceryOrderState.RequestGoingToClient && x.Stat != (int)GroceryOrderState.RequestEnded);
                var buyOrders_CloseTofinish = db.CarppiGrocery_BuyOrders.Where(x => x.FaceIDRepartidor_RepartidorCadena == Driver.FaceID_Repartidor && x.Stat != (int)GroceryOrderState.RequestGoingToClient && x.Stat != (int)GroceryOrderState.RequestEnded);
                var buyOrders_all = db.CarppiGrocery_BuyOrders.Where(x => x.FaceIDRepartidor_RepartidorCadena == Driver.FaceID_Repartidor);

                foreach (var order in buyOrders_all)
                {
                    Temp_TODestiny += ManhattanDisane(Convert.ToDouble(Driver.Longitud), Convert.ToDouble(Driver.Latitud), Convert.ToDouble(order.Longitud), Convert.ToDouble(order.Latitud));
                }

                if ((Temp_TODestiny + Temp_TOpassenger) < Distance)
                {
                    Distance = (Temp_TODestiny + Temp_TOpassenger);
                    Conductor = Driver;
                }

            }
            //OrdenesTotales

            if (Conductor != null)
            {
                var OrdenesTotales = db.CarppiGrocery_BuyOrders.Where(x => x.FaceIDRepartidor_RepartidorCadena == Conductor.FaceID_Repartidor && (x.Stat != (int)GroceryOrderState.RequestEnded && x.Stat != (int)GroceryOrderState.RequestRejected));
                if (OrdenesTotales != null)
                {
                    if (OrdenesTotales.Count() > 10)
                    {
                        DriverHasToomuchOrders = true;
                    }
                }
            }
            return Conductor;
        }
        public Double ManhattanDisane(double LatOrige, double longOriges, double LatDestino, double LongDestiono)
        {
            return Math.Abs(LatOrige - LatDestino) + Math.Abs(longOriges - LongDestiono);
        }

        static bool PointInPolygon(double[] polyX, double[] polyY, double x, double y)
        {
            int polyCorners = polyX.Length;

            int i, j = polyCorners - 1;
            bool oddNodes = false;
            if (x < polyX.Min() || x > polyX.Max() || y < polyY.Min() || y > (polyY.Max()))
            {

                // We're outside the polygon!
            }
            else
            {
                // oddNodes = true;


                for (i = 0; i < polyCorners; i++)
                {
                    if ((polyY[i] < y && polyY[j] >= y || polyY[j] < y && polyY[i] >= y) && (polyX[i] <= x || polyX[j] <= x))
                    {
                        if (polyX[i] + (y - polyY[i]) / (polyY[j] - polyY[i]) * (polyX[j] - polyX[i]) < x)
                        {
                            oddNodes = !oddNodes;
                        }
                    }
                    j = i;
                }

            }
            return oddNodes;
        }

        public CircleArray GenerateCircle(double Radio, double CentroX, double CentroY)
        {
            List<double> x = new List<double>();
            List<double> y = new List<double>();
            for (var i = 0.0; i < Math.PI * 2; i = i + 0.1)
            {
                x.Add(CentroX + (Radio * Math.Sin(i)));
                y.Add(CentroY + (Radio * Math.Cos(i)));
            }
            return new CircleArray(x, y);

        }
        public class CircleArray
        {
            public CircleArray(List<double> X, List<double> y)
            {
                X_Array = X.ToArray();
                Y_Array = y.ToArray();

            }
            public double[] X_Array;
            public double[] Y_Array;
        }

        public double FeeCalculator(double TotalDistance )
        {

            var HighTarif = leftelbow(TotalDistance);
            var Normal = Triangle(TotalDistance);
            var lowTarif = rigthelbow(TotalDistance);
            var FuzzySum = HighTarif + Normal + lowTarif;

            var Percentage = (15 * (HighTarif / FuzzySum)) + (20 * (Normal / FuzzySum)) + (30 * (lowTarif / FuzzySum));
            return Percentage;


        }
        public double leftelbow(double Distance)
        {
            var alpha = 2.0;
            var beta = 3.5;
            if (Distance <= alpha)
            {
                return 1;
            }
            else if (Distance > alpha && Distance < beta)
            {
                return ((Distance - beta) / (alpha - beta));
            }
            else
            {
                return 0;
            }
        }
        public double Triangle(double Distance)
        {
            var alpha = 2.0;
            var beta = 8.0;
            var delta = 3.5;
            if (Distance > alpha && Distance < delta)
            {
                return ((Distance - alpha) / (beta - alpha));
            }
            else if (Distance > delta && Distance < beta)
            {
                return ((Distance - beta) / (delta - beta));
            }
            else
            {
                return 0;
            }
        }

        public double rigthelbow(double Distance)
        {
            var alpha =3.5;
            var beta = 8.0;
            if (Distance > beta)
            {
                return 1;
            }
            else if (Distance > alpha && Distance < beta)
            {
                return ((Distance - alpha) / (beta - alpha));
            }
            else
            {
                return 0;
            }
        }




    }
}