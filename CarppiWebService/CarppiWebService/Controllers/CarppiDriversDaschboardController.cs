using CarppiWebService.Models;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CarppiWebService.Controllers
{
    public class CarppiDriversDaschboardController : Controller
    {
        // GET: CarppiDriversDaschboard
        PidgeonEntities db = new PidgeonEntities();
        public ActionResult Index(string Hash_ID)
        {

            var User = db.Traveler_Perfil.Where(x => x.Facebook_profile_id == Hash_ID).FirstOrDefault();
            if (User.StripeDriverID != null)
            {
                string CONNECTED_STRIPE_ACCOUNT_ID = User.StripeDriverID;
                StripeConfiguration.ApiKey = "sk_live_oAblnbfDurc783Y2k8Pt2FdN00yY8tjoWJ";



                var options = new ChargeListOptions { Limit = 100 };
                var service = new ChargeService();
                var Charges = service.List(options, new RequestOptions { StripeAccount = CONNECTED_STRIPE_ACCOUNT_ID });

                var Transactions = new BalanceTransactionListOptions { Limit = 100 };
                var transactionsService = new BalanceTransactionService();
                var t_list = transactionsService.List(Transactions, new RequestOptions { StripeAccount = CONNECTED_STRIPE_ACCOUNT_ID });

                ViewBag.TransacionList = t_list;

                //< tr role = "row" class="odd">
                //                                                           <td class="sorting_1">Airi Satou</td>
                //                                                           <td>Accountant</td>
                //                                                           <td>Tokyo</td>
                //                                                           <td>33</td>
                //                                                           <td>2008/11/28</td>
                //                                                           <td>$162,700</td>
                //                                                       </tr>

                string RoleString = "";
                int counter = 0;
                foreach (var transaction in t_list)
                {
                    RoleString += "<tr role='row' class=" + (((counter % 2) == 0) ? "odd" : "even") + "'>";
                    RoleString += "<td class='sorting_1'>" + transaction.Created + "</td>";
                    RoleString += "<td>" + transaction.Amount / 100 + "</td>";
                    RoleString += "<td>" + transaction.Status + "</td>";
                    RoleString += "</tr>";

                    counter++;
                }

                ViewBag.RoleString = RoleString;

                long total_volume = 0;
                long net_volume = 0;

                foreach (var transac in t_list)
                {
                    if (transac.Type.Equals("charge"))
                    {
                        total_volume += transac.Amount;

                    }
                    net_volume += transac.Amount;
                }

                var balance = new BalanceService();
                var puts = balance.Get(new RequestOptions { StripeAccount = CONNECTED_STRIPE_ACCOUNT_ID });
                ViewBag.AvailableinBag = (puts.Available.FirstOrDefault().Amount) / 100;
                ViewBag.PendingPayment = (puts.Pending.FirstOrDefault().Amount) / 100;


                var payots = new PayoutService();
                var PayoutList = payots.List(new PayoutListOptions { Limit = 100 }, new RequestOptions { StripeAccount = CONNECTED_STRIPE_ACCOUNT_ID });


            }
            else
            {
                ViewBag.AvailableinBag = 0;
                ViewBag.PendingPayment = 0;
                string RoleString = "";
                int counter = 0;
                RoleString += "<tr role='row' class=" + (((counter % 2) == 0) ? "odd" : "even") + "'>";
                RoleString += "<td class='sorting_1'>" + 0 + "</td>";
                RoleString += "<td>" + 0 + "</td>";
                RoleString += "<td>" + 0 + "</td>";
                RoleString += "</tr>";
                ViewBag.RoleString = RoleString;
            }
            //var randomPayot = PayoutList.FirstOrDefault().Amount;


            //            var f = Charges.FirstOrDefault().
            //# Authenticate as the connected account and retrieve the first 100 charges
            //            charges = Stripe::Charge.list({limit: 100},{stripe_account: "{{CONNECTED_STRIPE_ACCOUNT_ID}}"})

            //# Iterate through charges using auto-pagination
            //charges.auto_paging_each do |charge|
            //  # Output the charge ID, amount, currency, refund, and dispute status
            //  dispute = charge.dispute ? charge.dispute.id : "none"
            //  puts "#{charge.id},#{charge.amount},#{charge.currency},#{charge.refunded},#{dispute}"
            //end

            return View();
        }
    }
}