using System.ComponentModel;
using Munchii;
using Munchii.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomShadowFrame), typeof(CustomShadowFrameRenderer))]
namespace Munchii.iOS
{
    public class CustomShadowFrameRenderer : FrameRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);
            UpdateShadow();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)

            {
                UpdateShadow();
            }
        }

        private void UpdateShadow()
        {
            if (NativeView is UIView nativeView)
            {
                nativeView.Layer.ShadowColor = UIColor.Black.CGColor;
                nativeView.Layer.ShadowOffset = new CoreGraphics.CGSize(0, 2); // Adjust as necessary
                nativeView.Layer.ShadowOpacity = 0.3f; // Adjust as necessary
                nativeView.Layer.ShadowRadius = 4; // Adjust as necessary
            }
        }
    }
}
