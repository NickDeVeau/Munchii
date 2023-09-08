using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Munchii;
using Munchii.iOS;
using CoreGraphics;
using System.ComponentModel;
using System;
using Munchii.Controllers;

[assembly: ExportRenderer(typeof(CustomSlider), typeof(CustomSliderRenderer))]

namespace Munchii.iOS
{
    public class CustomSliderRenderer : SliderRenderer
    {
        UIView trackView;
        UIView borderTrackView;
        UIView customMaxTrack;
        UIColor customRedColor = UIColor.FromRGB(210, 53, 53);

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            UpdateTrackView();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == Slider.MinimumProperty.PropertyName ||
                e.PropertyName == Slider.MaximumProperty.PropertyName ||
                e.PropertyName == Slider.ValueProperty.PropertyName)
            {
                UpdateTrackView();
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Slider> e)
        {
            base.OnElementChanged(e);

            if (Control == null) return;

            if (e.OldElement != null)
            {
                Control.ValueChanged -= OnControlValueChanged;

                // Cleanup old custom views
                trackView?.RemoveFromSuperview();
                borderTrackView?.RemoveFromSuperview();
                customMaxTrack?.RemoveFromSuperview();
            }

            if (Control != null && e.NewElement != null)
            {
                var thumbImageSize = new CGSize(30, 30);
                var thumbImage = GenerateTargetImage(thumbImageSize, UIColor.White, customRedColor, 5);

                Control.SetThumbImage(thumbImage, UIControlState.Normal);

                Control.BackgroundColor = UIColor.White;
                Control.MaximumTrackTintColor = UIColor.Clear;  // Set this to Clear as we are using a custom view
                Control.MinimumTrackTintColor = UIColor.Clear;  // Set this to Clear as well to ensure our custom track shows

                // Border view for the minimum track
                borderTrackView = new UIView
                {
                    BackgroundColor = customRedColor,
                    UserInteractionEnabled = false
                };
                Control.InsertSubview(borderTrackView, 0);

                // Actual minimum track
                trackView = new UIView(new CGRect(0, Control.Frame.Height / 2 - thumbImageSize.Height / 2, Control.Frame.Width, thumbImageSize.Height))
                {
                    BackgroundColor = customRedColor,
                    UserInteractionEnabled = false
                };
                trackView.Layer.CornerRadius = 10;
                Control.InsertSubview(trackView, 1);

                // Maximum track with white color and a red border
                customMaxTrack = new UIView
                {
                    BackgroundColor = UIColor.White,
                    Layer = {
                        BorderColor = customRedColor.CGColor,
                        BorderWidth = 2,
                        CornerRadius = 7.5f
                    },
                    UserInteractionEnabled = false
                };
                Control.InsertSubview(customMaxTrack, 0);  // Insert it behind other tracks

                foreach (var view in Control.Subviews)
                {
                    if (view != trackView && view != borderTrackView && view != customMaxTrack)
                    {
                        Control.BringSubviewToFront(view);
                    }
                }

                Control.ValueChanged += OnControlValueChanged;

                // Ensure visual properties are updated
                UpdateTrackView();
            }
        }

        private void OnControlValueChanged(object sender, EventArgs e)
        {
            UpdateTrackView();
        }

        private void UpdateTrackView()
        {
            if (Control != null && trackView != null && borderTrackView != null && customMaxTrack != null)
            {
                var thumbWidth = Control.CurrentThumbImage.Size.Width;
                var sliderWidth = Control.Bounds.Size.Width;
                var availableWidth = sliderWidth - thumbWidth;
                var trackWidth = availableWidth * (float)Control.Value / (float)Control.MaxValue;
                var trackThickness = 15;
                var borderThickness = trackThickness + 2;

                trackView.Frame = new CGRect(thumbWidth / 2, (Control.Bounds.Height - trackThickness) / 2, trackWidth, trackThickness);
                trackView.Layer.CornerRadius = trackThickness / 2;

                borderTrackView.Frame = new CGRect((thumbWidth - 1) / 2, (Control.Bounds.Height - borderThickness) / 2, trackWidth + 2, borderThickness);
                borderTrackView.Layer.CornerRadius = borderThickness / 2;

                var customMaxTrackWidth = sliderWidth - trackWidth - thumbWidth;
                customMaxTrack.Frame = new CGRect(trackWidth + thumbWidth / 2, (Control.Bounds.Height - trackThickness) / 2, customMaxTrackWidth, trackThickness);
            }
        }

        private UIImage GenerateTargetImage(CGSize size, UIColor backgroundColor, UIColor circleColor, nfloat borderWidth)
        {
            var canvasSize = new CGSize(size.Width + borderWidth * 2, size.Height + borderWidth * 2);
            UIGraphics.BeginImageContextWithOptions(canvasSize, false, 0);
            using (var context = UIGraphics.GetCurrentContext())
            {
                context.SetFillColor(backgroundColor.CGColor);
                context.FillEllipseInRect(new CGRect(borderWidth, borderWidth, size.Width, size.Height));

                var innerCircleSize = size.Width - 2 * borderWidth;
                context.SetFillColor(circleColor.CGColor);
                context.FillEllipseInRect(new CGRect(2 * borderWidth, 2 * borderWidth, innerCircleSize, innerCircleSize));

                context.SetStrokeColor(circleColor.CGColor);
                context.SetLineWidth(2);
                context.StrokeEllipseInRect(new CGRect(borderWidth, borderWidth, size.Width, size.Height));
            }

            var image = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return image;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Control != null)
                {
                    Control.ValueChanged -= OnControlValueChanged;
                }
            }
            base.Dispose(disposing);
        }

    }
}
