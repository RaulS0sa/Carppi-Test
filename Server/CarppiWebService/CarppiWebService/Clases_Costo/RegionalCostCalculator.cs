using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarppiWebService.Clases_Costo
{
    public class RegionalCostCalculator
    {
       public double LatitudOrigen;
        public double LongitudOrigen;
        public double LatitudDestino;
        public double LongitudDestino;
        public double Distance;
        public double CostoRideShare;
        public double CostoPool;
        public double ComisionRideShare;
        public double ComisionPool;

        public RegionalCostCalculator(double LatitudOrigen_arg, double LongitudOrigen_arg, double LatitudDestino_arg, double LongitudDestino_arg)
        {
            LatitudOrigen = LatitudOrigen_arg;
            LongitudOrigen = LongitudOrigen_arg;
            LatitudDestino = LatitudDestino_arg;
            LongitudDestino = LongitudDestino_arg;
            Distance = ((Math.Abs((LatitudOrigen - LatitudDestino)) + Math.Abs((LongitudOrigen - LongitudDestino))) * (1 / 0.009090));
            var Costo_Rideshare = ValidaViaje_y_Calcula_costo_RideShare((Distance).ToString());
            var Costo_Pool = ValidaViaje_y_Calcula_costo((Distance).ToString());


            CostoRideShare = Costo_Rideshare < 15 ? 15.0 : Costo_Rideshare;
            CostoPool = Costo_Pool < 10 ? 10.0 : Costo_Pool;
            var FeePercentage = FeeCalculator();
            ComisionRideShare = (CostoRideShare * FeePercentage) + 4.5;
            ComisionPool = (CostoPool * FeePercentage) + 4.5;

        }
        public CostCalculatedReturnValue CalculateRegularCost()
        {
            return new CostCalculatedReturnValue(CostoRideShare + ComisionRideShare, CostoPool + ComisionPool);
        }

        public double FeeCalculator()
        {

            var HighTarif = leftelbow();
            var Normal = Triangle();
            var lowTarif = rigthelbow();
            var FuzzySum = HighTarif + Normal + lowTarif;

            var Percentage = (0.13 * (HighTarif / FuzzySum)) + (0.10 * (Normal / FuzzySum)) + (0.08 * (lowTarif / FuzzySum));
            return Percentage;


        }
        public double leftelbow()
        {
            var alpha = 3.0;
            var beta = 6.5;
            if(Distance<= alpha)
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
        public double Triangle()
        {
            var alpha = 3.0;
            var beta = 10.0;
            var delta = 6.5;
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

        public double rigthelbow()
        {
            var alpha = 6.5;
            var beta = 10.0;
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




       
        

        

            public double ValidaViaje_y_Calcula_costo_RideShare(string distancia)
        {
            Interpolate_Points P1 = new Interpolate_Points();
            P1.X0 = 0; P1.X1 = 1.8; P1.Y0 = 6; P1.Y1 = 9.85;// = {0,0,0,0 };
            Interpolate_Points P2 = new Interpolate_Points();
            P2.X0 = 1.8; P2.X1 = 2.4; P2.Y0 = 9.85; P2.Y1 = 9.15;// = {0,0,0,0 };
            Interpolate_Points P3 = new Interpolate_Points();
            P3.X0 = 2.4; P3.X1 = 3.8; P3.Y0 = 9.15; P3.Y1 = 9.32;// = {0,0,0,0 };
            Interpolate_Points P4 = new Interpolate_Points();
            P4.X0 = 3.8; P4.X1 = 4.5; P4.Y0 = 9.32; P4.Y1 = 8.47;// = {0,0,0,0 };
            Interpolate_Points P5 = new Interpolate_Points();
            P5.X0 = 4.5; P5.X1 = 8; P5.Y0 = 8.47; P5.Y1 = 5.2;// = {0,0,0,0 };
            List<Interpolate_Points> puntos = new List<Interpolate_Points>();
            puntos.Add(P1);
            puntos.Add(P2);
            puntos.Add(P3);
            puntos.Add(P4);
            puntos.Add(P5);
            double price = calculate_interpolation_RideShare(puntos, Convert.ToDouble(distancia));
            //AlertDialog.Builder alertDialog = new AlertDialog.Builder(mContext);
            //alertDialog.SetTitle("Alert");
            //alertDialog.SetMessage((price * Convert.ToDouble(distancia)).ToString());
            //alertDialog.SetPositiveButton("Delete", (senderAlert, args) =>
            //{
            //    Toast.MakeText(mContext, "Deleted!", ToastLength.Short).Show();
            //});
            //Dialog dialog = alertDialog.Create();
            ////dialog.Show();
            //Cost_Adapter
            var InterpolatedCost = (((price * Convert.ToDouble(distancia))));
            // Fragment1.Cost = (InterpolatedCost).ToString();

            //Action action = () =>
            //{
            //    //var jsr = new JavascriptResult();
            //    var script = "Cost_Adapter(" + (InterpolatedCost).ToString() + ")";
            //    webi.EvaluateJavascript(script, null);

            //};

            // Create a task but do not start it.
            // Task t1 = new Task(action, "alpha");

            // webi.Post(action);

            return InterpolatedCost;


        }
        public double calculate_interpolation_RideShare(List<Interpolate_Points> List_of_points, double x_arg)
        {
            double result = 0;
            if (x_arg >= 8)
            {
                result = 5.2;
                //var point = List_of_points[4];
                //result = ((point.Y0 * (point.X1 - x_arg)) + (point.Y1 * (x_arg - point.X0))) / (point.X1 - point.X0);
            }
            else
            {
                foreach (var point in List_of_points)
                {
                    if (x_arg >= point.X0 && x_arg < point.X1)
                    {
                        result = ((point.Y0 * (point.X1 - x_arg)) + (point.Y1 * (x_arg - point.X0))) / (point.X1 - point.X0);
                    }

                }
            }

            return result;
        }

        public double ValidaViaje_y_Calcula_costo(string distancia)
        {
            Interpolate_Points P1 = new Interpolate_Points();
            P1.X0 = 0; P1.X1 = 20; P1.Y0 = 2; P1.Y1 = 1.5;// = {0,0,0,0 };
            Interpolate_Points P2 = new Interpolate_Points();
            P2.X0 = 20; P2.X1 = 60; P2.Y0 = 1.5; P2.Y1 = 1.05;// = {0,0,0,0 };
            Interpolate_Points P3 = new Interpolate_Points();
            P3.X0 = 60; P3.X1 = 120; P3.Y0 = 1.05; P3.Y1 = 0.97;// = {0,0,0,0 };
            Interpolate_Points P4 = new Interpolate_Points();
            P4.X0 = 120; P4.X1 = 290; P4.Y0 = 0.97; P4.Y1 = 0.765;// = {0,0,0,0 };
            Interpolate_Points P5 = new Interpolate_Points();
            P5.X0 = 290; P5.X1 = 500; P5.Y0 = 0.765; P5.Y1 = 0.735;// = {0,0,0,0 };
            List<Interpolate_Points> puntos = new List<Interpolate_Points>();
            puntos.Add(P1);
            puntos.Add(P2);
            puntos.Add(P3);
            puntos.Add(P4);
            puntos.Add(P5);
            double price = calculate_interpolation(puntos, Convert.ToDouble(distancia));
            //AlertDialog.Builder alertDialog = new AlertDialog.Builder(mContext);
            //alertDialog.SetTitle("Alert");
            //alertDialog.SetMessage((price * Convert.ToDouble(distancia)).ToString());
            //alertDialog.SetPositiveButton("Delete", (senderAlert, args) =>
            //{
            //    Toast.MakeText(mContext, "Deleted!", ToastLength.Short).Show();
            //});
            //Dialog dialog = alertDialog.Create();
            //dialog.Show();
            //Cost_Adapter
            var InterpolatedCost = (((price * Convert.ToDouble(distancia)) * 1.0745) + 3.0);
            // Fragment1.Cost = (InterpolatedCost).ToString();

            //Action action = () =>
            //{
            //    //var jsr = new JavascriptResult();
            //    var script = "Cost_Adapter(" + (InterpolatedCost).ToString() + ")";
            //    webi.EvaluateJavascript(script, null);

            //};

            // Create a task but do not start it.
            // Task t1 = new Task(action, "alpha");

            // webi.Post(action);

            return InterpolatedCost;


        }


        public double calculate_interpolation(List<Interpolate_Points> List_of_points, double x_arg)
        {
            double result = 0;
            if (x_arg >= 500)
            {
                result = 0.74;
                //var point = List_of_points[4];
                //result = ((point.Y0 * (point.X1 - x_arg)) + (point.Y1 * (x_arg - point.X0))) / (point.X1 - point.X0);
            }
            else
            {
                foreach (var point in List_of_points)
                {
                    if (x_arg >= point.X0 && x_arg < point.X1)
                    {
                        result = ((point.Y0 * (point.X1 - x_arg)) + (point.Y1 * (x_arg - point.X0))) / (point.X1 - point.X0);
                    }

                }
            }

            return result;
        }
    }
    public class Interpolate_Points
    {
        public double X0;
        public double X1;
        public double Y0;
        public double Y1;// = {0,0,0,0 };
    }
    public class CostCalculatedReturnValue
    {
        public double RideShareCost;
        public double CarpoolCost;
        public CostCalculatedReturnValue(double rideshare, double pool)
        {
            RideShareCost = rideshare;
            CarpoolCost = pool;
        }
    }
}