using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Consolonia.Core.Controls
{
    public partial class MessageBox : DialogWindow
    {
        public static readonly DirectProperty<MessageBox, object> OkProperty =
            AvaloniaProperty.RegisterDirect<MessageBox, object>(nameof(Ok), mb => mb.Ok, (mb, value) => mb.Ok = value);

        public static readonly DirectProperty<MessageBox, object> CancelProperty =
            AvaloniaProperty.RegisterDirect<MessageBox, object>(nameof(Cancel), mb => mb.Cancel, (mb, value) => mb.Cancel = value);

        public static readonly DirectProperty<MessageBox, object> YesProperty =
            AvaloniaProperty.RegisterDirect<MessageBox, object>(nameof(Yes), mb => mb.Yes, (mb, value) => mb.Yes = value);

        public static readonly DirectProperty<MessageBox, object> NoProperty =
            AvaloniaProperty.RegisterDirect<MessageBox, object>(nameof(No), mb => mb.No, (mb, value) => mb.No = value);

        public static readonly DirectProperty<MessageBox, Mode> ModeProperty =
            AvaloniaProperty.RegisterDirect<MessageBox, Mode>(nameof(Mode), mb => mb.Mode, (mb, value) => mb.Mode = value);

        public MessageBox()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            Loaded += (_, _) =>
            {
                (
                    this.FindControl<Button>(nameof(OkButton)) ??
                    this.FindControl<Button>(nameof(CancelButton)) ??
                    this.FindControl<Button>(nameof(YesButton)) ??
                    this.FindControl<Button>(nameof(NoButton))
                )?.Focus();
            };

            InitializeComponent();
        }

        private Mode _mode = Mode.Ok;
        public Mode Mode
        {
            get => _mode;
            set => SetAndRaise(ModeProperty, ref _mode, value);
        }


        private object _ok = "OK";
        public object Ok
        {
            get => _ok;
            set => SetAndRaise(OkProperty, ref _ok, value);
        }

        private object _cancel = "Cancel";
        public object Cancel
        {
            get => _cancel;
            set => SetAndRaise(CancelProperty, ref _cancel, value);
        }

        private object _yes = "Yes";
        public object Yes
        {
            get => _yes;
            set => SetAndRaise(YesProperty, ref _yes, value);
        }

        private object _no = "No";
        public object No
        {
            get => _no;
            set => SetAndRaise(NoProperty, ref _no, value);
        }

        public async Task<MessageBoxResult> ShowDialogAsync(Control parent, string text, string title = null)
        {
            DataContext = new MessageBoxViewModel(Mode, Ok, Cancel, Yes, No, text, title ?? this.Title);
#nullable enable
            var result = await ShowDialogAsync<MessageBoxResult?>(parent);
            if (result.HasValue)
            {
                return result.Value;
            }
            return MessageBoxResult.Cancel;
#nullable disable
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }


        private void OnOk(object sender, RoutedEventArgs e)
            => CloseDialog(MessageBoxResult.Ok);

        private void OnCancel(object sender, RoutedEventArgs e)
            => CloseDialog(MessageBoxResult.Cancel);

        private void OnYes(object sender, RoutedEventArgs e)
            => CloseDialog(MessageBoxResult.Yes);

        private void OnNo(object sender, RoutedEventArgs e)
            => CloseDialog(MessageBoxResult.No);
    }

    public enum Mode
    {
        /// <summary>
        ///  Specifies that the message box contains an OK button.
        /// </summary>
        Ok,

        /// <summary>
        ///  Specifies that the message box contains OK and Cancel buttons.
        /// </summary>
        OkCancel,

        /// <summary>
        ///  Specifies that the message box contains Yes and No buttons.
        /// </summary>
        YesNo,

        /// <summary>
        ///  Specifies that the message box contains Yes, No, and Cancel buttons.
        /// </summary>
        YesNoCancel,
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
        internal MessageBoxViewModel(Mode mode, object okContent, object cancelContent, object yesContent, object noContent, string text, string title)
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

        [ObservableProperty]
        private Mode _mode;

        [ObservableProperty]
        private string _title;

        [ObservableProperty]
        private bool _showCancelButton;

        [ObservableProperty]
        private bool _showOkButton;

        [ObservableProperty]
        private bool _showYesButton;

        [ObservableProperty]
        private bool _showNoButton;

        [ObservableProperty]
        private object _okContent;

        [ObservableProperty]
        private object _cancelContent;

        [ObservableProperty]
        private object _yesContent;

        [ObservableProperty]
        private object _noContent;

        [ObservableProperty]
        private string _text;
    }
}