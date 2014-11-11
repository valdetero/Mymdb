using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Mymdb.Droid.Views
{
    [Activity(Label = "Mymdb", MainLauncher=true)]
    public class MainActivity : Xamarin.Forms.Platform.Android.AndroidActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Xamarin.Insights.Initialize(Core.Constants.Insights.ApiKey, this);
            Xamarin.Insights.ForceDataTransmission = true;
                
            Xamarin.Forms.Forms.Init(this, bundle);

            IoC.ServiceContainer.Register<Xamarin.Forms.Platform.Android.AndroidActivity>(() => this);
            ServiceRegistrar.Init();

            SetPage(Mymdb.UI.App.GetMainPage());
        }
    }
}