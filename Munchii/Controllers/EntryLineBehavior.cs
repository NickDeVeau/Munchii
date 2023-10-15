using Xamarin.Forms;

namespace Munchii
{
    public class EntryLineBehavior : Behavior<Entry>
    {
        public static readonly BindableProperty NextEntryProperty =
            BindableProperty.Create(nameof(NextEntry), typeof(Entry), typeof(EntryLineBehavior), null);

        public static readonly BindableProperty PreviousEntryProperty =
            BindableProperty.Create(nameof(PreviousEntry), typeof(Entry), typeof(EntryLineBehavior), null);

        public Entry NextEntry
        {
            get { return (Entry)GetValue(NextEntryProperty); }
            set { SetValue(NextEntryProperty, value); }
        }

        public Entry PreviousEntry
        {
            get { return (Entry)GetValue(PreviousEntryProperty); }
            set { SetValue(PreviousEntryProperty, value); }
        }

        protected override void OnAttachedTo(Entry entry)
        {
            entry.TextChanged += OnEntryTextChanged;
            base.OnAttachedTo(entry);
        }

        private void OnEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            Entry entry = (Entry)sender;

            // Check if the new text is a digit
            if (!string.IsNullOrEmpty(e.NewTextValue) && !char.IsDigit(e.NewTextValue[0]))
            {
                // If not a digit, revert to the old value
                entry.Text = e.OldTextValue;
                return;
            }

            Device.BeginInvokeOnMainThread(() =>
            {
                // If a character was added, and the entry is at max length, move to the next entry.
                if (!string.IsNullOrEmpty(e.NewTextValue) && e.NewTextValue.Length == entry.MaxLength)
                {
                    NextEntry?.Focus();
                }
                // If a character was deleted, move to the previous entry (if the current entry has one character left).
                else if (e.NewTextValue?.Length < e.OldTextValue?.Length)
                {
                    PreviousEntry?.Focus();
                }
            });
        }

        protected override void OnDetachingFrom(Entry entry)
        {
            entry.TextChanged -= OnEntryTextChanged;
            base.OnDetachingFrom(entry);
        }
    }
}
