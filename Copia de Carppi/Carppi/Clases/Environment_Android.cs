using System;
using System.Threading.Tasks;
using Android.Content.Res;
using Android.OS;
using Plugin.CurrentActivity;
//using static Android.Content.Res.Resources;
//using Xamarin.Forms;

namespace Carppi.Clases
{
    public class Environment_Android// : IEnvironment
    {
        public Task<UiMode> GetOperatingSystemThemeAsync() =>
            Task.FromResult(GetOperatingSystemTheme());

        public UiMode GetOperatingSystemTheme()
        {
            //Ensure the device is running Android Froyo or higher because UIMode was added in Android Froyo, API 8.0
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Froyo)
            {
                var uiModeFlags = CrossCurrentActivity.Current.AppContext.Resources.Configuration.UiMode & UiMode.NightMask;

                switch (uiModeFlags)
                {
                    case UiMode.NightYes:
                        return UiMode.NightYes;

                    case UiMode.NightNo:
                        return UiMode.NightNo;

                    default:
                        return UiMode.NightNo;

                        break;//throw new NotSupportedException($"UiMode {uiModelFlags} not supported");
                }
            }
            else
            {
                return UiMode.NightNo;
            }
        }
    }
}
