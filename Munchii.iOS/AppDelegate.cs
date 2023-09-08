using System;
using System.Collections.Generic;
using System.Linq;
using Firebase.Core;
using Foundation;
using UIKit;
using Xamarin.Forms.Maps;
using Xamarin.Forms;

namespace Munchii.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Xamarin.FormsMaps.Init();
            global::Xamarin.Forms.Forms.Init();
            Firebase.Core.App.Configure();
            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }
}

