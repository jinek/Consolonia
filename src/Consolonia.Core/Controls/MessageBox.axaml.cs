using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Consolonia.Core.Controls
{
    public partial class MessageBox : DialogWindow
    {
        public static readonly DirectProperty<MessageBox, object> OkProperty =
            AvaloniaProperty.RegisterDirect<MessageBox, object>(nameof(Ok), mb => mb.Ok, (mb, value) => mb.Ok = value);

        public static readonly DirectProperty<MessageBox, object> CancelProperty =
            AvaloniaProperty.RegisterDirect<MessageBox, object>(nameof(Cancel), mb => mb.Cancel,
                (mb, value) => mb.Cancel = value);

        public static readonly DirectProperty<MessageBox, object> YesProperty =
            AvaloniaProperty.RegisterDirect<MessageBox, object>(nameof(Yes), mb => mb.Yes,
                (mb, value) => mb.Yes = value);

        public static readonly DirectProperty<MessageBox, object> NoProperty =
            AvaloniaProperty.RegisterDirect<MessageBox, object>(nameof(No), mb => mb.No, (mb, value) => mb.No = value);

        public static readonly DirectProperty<MessageBox, Mode> ModeProperty =
            AvaloniaProperty.RegisterDirect<MessageBox, Mode>(nameof(Mode), mb => mb.Mode,
                (mb, value) => mb.Mode = value);

        private object _cancel = "Cancel";

        private Mode _mode = Mode.Ok;

        private object _no = "No";


        private object _ok = "OK";

        private object _yes = "Yes";

        public MessageBox()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            InitializeComponent();

            this.AttachedToVisualTree += (s, e) =>
            {
                if (OkButton.IsVisible)
                    OkButton.AttachedToVisualTree += (s2, e2) => OkButton.Focus();
                else if (YesButton.IsVisible)
                    YesButton.AttachedToVisualTree += (s2, e2) => YesButton.Focus();
                else if (CancelButton.IsVisible)
                    CancelButton.AttachedToVisualTree += (s2, e2) => CancelButton.Focus();
                else if (NoButton.IsVisible)
                    NoButton.AttachedToVisualTree += (s2, e2) => NoButton.Focus();
            };
        }

        public Mode Mode
        {
            get => _mode;
            set => SetAndRaise(ModeProperty, ref _mode, value);
        }

        public object Ok
        {
            get => _ok;
            set => SetAndRaise(OkProperty, ref _ok, value);
        }

        public object Cancel
        {
            get => _cancel;
            set => SetAndRaise(CancelProperty, ref _cancel, value);
        }

        public object Yes
        {
            get => _yes;
            set => SetAndRaise(YesProperty, ref _yes, value);
        }

        public object No
        {
            get => _no;
            set => SetAndRaise(NoProperty, ref _no, value);
        }

        public async Task<MessageBoxResult> ShowDialogAsync(Control parent, string text, string title = null)
        {
            DataContext = new MessageBoxViewModel(Mode, Ok, Cancel, Yes, No, text, title ?? Title);
#nullable enable
            var result = await ShowDialogAsync<MessageBoxResult?>(parent);
            if (result.HasValue) return result.Value;
            return MessageBoxResult.Cancel;
#nullable disable
        }

        private void OnOk(object sender, RoutedEventArgs e)
        {
            CloseDialog(MessageBoxResult.Ok);
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            CloseDialog(MessageBoxResult.Cancel);
        }

        private void OnYes(object sender, RoutedEventArgs e)
        {
            CloseDialog(MessageBoxResult.Yes);
        }

        private void OnNo(object sender, RoutedEventArgs e)
        {
            CloseDialog(MessageBoxResult.No);
        }
    }

    public enum Mode
    {
        /// <summary>
        ///     Specifies that the message box contains an OK button.
        /// </summary>
        Ok,

        /// <summary>
        ///     Specifies that the message box contains OK and Cancel buttons.
        /// </summary>
        OkCancel,

        /// <summary>
        ///     Specifies that the message box contains Yes and No buttons.
        /// </summary>
        YesNo,

        /// <summary>
        ///     Specifies that the message box contains Yes, No, and Cancel buttons.
        /// </summary>
        YesNoCancel
    }

    public enum MessageBoxResult
    {
        Ok,
        Cancel,
        Yes,
        No
    }

    internal partial class MessageBoxViewModel : ObservableObject
    {
        [ObservableProperty] private object _cancelContent;

        [ObservableProperty] private Mode _mode;

        [ObservableProperty] private object _noContent;

        [ObservableProperty] private object _okContent;

        [ObservableProperty] private bool _showCancelButton;

        [ObservableProperty] private bool _showNoButton;

        [ObservableProperty] private bool _showOkButton;

        [ObservableProperty] private bool _showYesButton;

        [ObservableProperty] private string _text;

        [ObservableProperty] private string _title;

        [ObservableProperty] private object _yesContent;

        internal MessageBoxViewModel(Mode mode, object okContent, object cancelContent, object yesContent,
            object noContent, string text, string title)
        {
            _mode = mode;
            switch (mode)
            {
                case Mode.Ok:
                    ShowOkButton = true;
                    break;
                case Mode.OkCancel:
                    ShowOkButton = true;
                    ShowCancelButton = true;
                    break;
                case Mode.YesNo:
                    ShowYesButton = true;
                    ShowNoButton = true;
                    break;
                case Mode.YesNoCancel:
                    ShowYesButton = true;
                    ShowNoButton = true;
                    ShowCancelButton = true;
                    break;
            }

            _okContent = okContent;
            _cancelContent = cancelContent;
            _yesContent = yesContent;
            _noContent = noContent;
            _text = text;
            _title = title;
        }
    }
}