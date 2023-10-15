using Xamarin.Forms;

namespace Munchii
{
    // EntryLineBehavior class handles text entry behavior for Entry elements.
    public class EntryLineBehavior : Behavior<Entry>
    {
        // Bindable properties for the next and previous Entry elements.
        public static readonly BindableProperty NextEntryProperty = BindableProperty.Create(nameof(NextEntry), typeof(Entry), typeof(EntryLineBehavior), null);
        public static readonly BindableProperty PreviousEntryProperty = BindableProperty.Create(nameof(PreviousEntry), typeof(Entry), typeof(EntryLineBehavior), null);

        // Properties for the next and previous Entry elements.
        public Entry NextEntry { get => (Entry)GetValue(NextEntryProperty); set => SetValue(NextEntryProperty, value); }
        public Entry PreviousEntry { get => (Entry)GetValue(PreviousEntryProperty); set => SetValue(PreviousEntryProperty, value); }

        // Attach behavior to Entry element.
        protected override void OnAttachedTo(Entry entry)
        {
            entry.TextChanged += OnEntryTextChanged;
            base.OnAttachedTo(entry);
        }

        // Handle text changes in the Entry element.
        private void OnEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            Entry entry = (Entry)sender;

            // Validate input to only allow digits.
            if (!string.IsNullOrEmpty(e.NewTextValue) && !char.IsDigit(e.NewTextValue[0]))
            {
                entry.Text = e.OldTextValue;
                return;
            }

            // Handle focus change based on text length.
            Device.BeginInvokeOnMainThread(() =>
            {
                if (!string.IsNullOrEmpty(e.NewTextValue) && e.NewTextValue.Length == entry.MaxLength)
                {
                    NextEntry?.Focus();
                }
                else if (e.NewTextValue?.Length < e.OldTextValue?.Length)
                {
                    PreviousEntry?.Focus();
                }
            });
        }

        // Detach behavior from Entry element.
        protected override void OnDetachingFrom(Entry entry)
        {
            entry.TextChanged -= OnEntryTextChanged;
            base.OnDetachingFrom(entry);
        }
    }
}
