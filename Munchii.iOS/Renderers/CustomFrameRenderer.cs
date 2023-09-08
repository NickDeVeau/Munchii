using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using CoreGraphics;
using Munchii.iOS;

[assembly: ExportRenderer(typeof(Frame), typeof(CustomFrameRenderer))]
namespace Munchii.iOS
{
    public class CustomFrameRenderer : FrameRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);
            UpdateShadow();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == "CornerRadius" ||
                e.PropertyName == "HasShadow")
            {
                UpdateShadow();
            }
        }

        void UpdateShadow()
        {
            if (Element == null || Layer == null) return;

            if (Element.HasShadow)
            {
                Layer.ShadowRadius = 5; // Adjust the radius to make shadow less intense
                Layer.ShadowColor = UIColor.Black.CGColor;
                Layer.ShadowOpacity = 0.2f; // Make it less intense
                Layer.ShadowOffset = new CGSize(3, 3); // Bottom right corner
                Layer.MasksToBounds = false;
            }
            else
            {
                Layer.ShadowOpacity = 0;
            }
        }
    }
}
