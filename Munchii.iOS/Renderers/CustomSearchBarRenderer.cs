using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using Munchii.iOS;
using System;

[assembly: ExportRenderer(typeof(SearchBar), typeof(CustomSearchBarRenderer))]
namespace Munchii.iOS
{
    public class CustomSearchBarRenderer : SearchBarRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<SearchBar> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                UITextField searchField = (UITextField)Control.ValueForKey(new Foundation.NSString("searchField"));
                if (searchField != null)
                {
                    searchField.BackgroundColor = FromHexString("#F0F4FB");  // LightGray in hex
                    searchField.TextColor = UIColor.Black;  // Set text color to black
                    searchField.AttributedPlaceholder = new Foundation.NSAttributedString(
                        searchField.Placeholder,
                        null,
                        UIColor.Gray // Set placeholder text color to gray
                    );
                    // Set the color of the magnifying glass icon
                    var glassIconView = searchField.LeftView as UIImageView;
                    if (glassIconView != null)
                    {
                        glassIconView.Image = glassIconView.Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                        glassIconView.TintColor = UIColor.Gray; // Set color to gray or whatever color you wish
                    }
                }
            }
        }

        public static UIColor FromHexString(string hexValue, float alpha = 1.0f)
        {
            var colorString = hexValue.Replace("#", "");
            float red, green, blue;

            switch (colorString.Length)
            {
                case 3: // #RGB
                    {
                        red = Convert.ToInt32(string.Format("{0}{0}", colorString.Substring(0, 1)), 16) / 255f;
                        green = Convert.ToInt32(string.Format("{0}{0}", colorString.Substring(1, 1)), 16) / 255f;
                        blue = Convert.ToInt32(string.Format("{0}{0}", colorString.Substring(2, 1)), 16) / 255f;
                        return UIColor.FromRGBA(red, green, blue, alpha);
                    }
                case 6: // #RRGGBB
                    {
                        red = Convert.ToInt32(colorString.Substring(0, 2), 16) / 255f;
                        green = Convert.ToInt32(colorString.Substring(2, 2), 16) / 255f;
                        blue = Convert.ToInt32(colorString.Substring(4, 2), 16) / 255f;
                        return UIColor.FromRGBA(red, green, blue, alpha);
                    }
                default:
                    throw new InvalidOperationException("Invalid color value");
            }
        }
    }

   

}
