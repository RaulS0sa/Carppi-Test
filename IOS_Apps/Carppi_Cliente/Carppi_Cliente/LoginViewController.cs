using System;
using System.IO;
using System.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AuthenticationServices;
using CoreImage;
using Foundation;
using SQLite;
using UIKit;
using Xamarin.Auth;

namespace Carppi_Cliente
{
    public partial class LoginViewController : UIViewController//, IASAuthorizationControllerDelegate//, IASAuthorizationControllerPresentationContextProviding
    {
        public LoginViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

           // SetupProviderLoginView();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

           // PerformExistingAccountSetupFlows();
        }
        /*
        void SetupProviderLoginView()
        {
            var authorizationButton = new ASAuthorizationAppleIdButton(ASAuthorizationAppleIdButtonType.Default, ASAuthorizationAppleIdButtonStyle.White);
            authorizationButton.TouchUpInside += HandleAuthorizationAppleIDButtonPress;
         //   loginProviderStackView.AddArrangedSubview(authorizationButton);
        }

        // Prompts the user if an existing iCloud Keychain credential or Apple ID credential is found.
        void PerformExistingAccountSetupFlows()
        {
            // Prepare requests for both Apple ID and password providers.
            ASAuthorizationRequest[] requests = {
            new ASAuthorizationAppleIdProvider ().CreateRequest (),
            new ASAuthorizationPasswordProvider ().CreateRequest ()
        };

            // Create an authorization controller with the given requests.
            var authorizationController = new ASAuthorizationController(requests);
            authorizationController.Delegate = this;
            authorizationController.PresentationContextProvider = this;
            authorizationController.PerformRequests();
        }

        private void HandleAuthorizationAppleIDButtonPress(object sender, EventArgs e)
        {
            var appleIdProvider = new ASAuthorizationAppleIdProvider();
            var request = appleIdProvider.CreateRequest();
            request.RequestedScopes = new[] { ASAuthorizationScope.Email, ASAuthorizationScope.FullName };

            var authorizationController = new ASAuthorizationController(new[] { request });
            authorizationController.Delegate = this;
            authorizationController.PresentationContextProvider = this;
            authorizationController.PerformRequests();
        }*/
        /*
        public UIWindow GetPresentationAnchor(ASAuthorizationController controller)
        {
          //  throw new NotImplementedException();
        }
        */
    }
}
