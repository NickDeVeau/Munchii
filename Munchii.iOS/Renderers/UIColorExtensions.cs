using System;
using CoreGraphics;
using UIKit;

namespace Munchii.iOS.Renderers
{
    public static class UIColorExtensions
    {

        private static UIImage GenerateTargetImage(CGSize size, UIColor backgroundColor, UIColor circleColor, nfloat borderWidth)
        {
            UIGraphics.BeginImageContextWithOptions(size, false, 0);
            using (var context = UIGraphics.GetCurrentContext())
            {
                // Draw outer (background) circle
                context.SetFillColor(backgroundColor.CGColor);
                context.FillEllipseInRect(new CGRect(0, 0, size.Width, size.Height));

                // Draw inner circle
                nfloat innerCircleSize = size.Width - 2 * borderWidth;
                context.SetFillColor(circleColor.CGColor);
                context.FillEllipseInRect(new CGRect(borderWidth, borderWidth, innerCircleSize, innerCircleSize));
            }

            UIImage image = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return image;
        }

    }



}

