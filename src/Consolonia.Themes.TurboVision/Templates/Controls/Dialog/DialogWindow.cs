using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace Consolonia.Themes.TurboVision.Templates.Controls.Dialog
{
    public class DialogWindow : UserControl
    {
        public static readonly DirectProperty<DialogWindow, Size> ContentSizeProperty =
            AvaloniaProperty.RegisterDirect<DialogWindow, Size>(nameof(ContentSize), window => window.ContentSize);

        public static readonly StyledProperty<string> TitleProperty = Window.TitleProperty.AddOwner<DialogWindow>();

        public static readonly StyledProperty<bool> IsCloseButtonVisibleProperty =
            AvaloniaProperty.Register<DialogWindow, bool>(nameof(IsCloseButtonVisible), true);

        private Size _contentSize = Size.Empty;
        private IDisposable _disposable2;

        private TaskCompletionSource _taskCompletionSource;

        static DialogWindow()
        {
            TitleProperty.OverrideDefaultValue<DialogWindow>(string.Empty);
            ContentProperty.Changed.AddClassHandler<DialogWindow>((x, e) => x.ContentChanged2(e));
        }

        public DialogWindow()
        {
            KeyDown += InputElement_OnKeyDown;
        }

        public Size ContentSize
        {
            get => _contentSize;
            private set => SetAndRaise(ContentSizeProperty, ref _contentSize, value);
        }

        public string Title
        {
            get => GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public bool IsCloseButtonVisible
        {
            get => GetValue(IsCloseButtonVisibleProperty);
            set => SetValue(IsCloseButtonVisibleProperty, value);
        }

        public bool CancelOnEscape { get; set; } = true;

        // ReSharper disable once UnusedMember.Global Used by template
        public void CloseClick(object _)
        {
            if (CancelOnEscape)
                CloseDialog();
        }

        private void ContentChanged2(AvaloniaPropertyChangedEventArgs args)
        {
            var control = (IControl)args.NewValue;
            _disposable2?.Dispose();
            _disposable2 = control?.GetPropertyChangedObservable(BoundsProperty).Subscribe(args2 =>
            {
                var rect = (Rect)args2.NewValue;
                ContentSize = rect.Size;
            });
        }

        private void ShowDialogInternal(IControl parent)
        {
            DialogHost dialogHost = GetDialogHost(parent);
            dialogHost.OpenInternal(this);
        }

        public virtual void CloseDialog()
        {
            DialogHost dialogHost = GetDialogHost(this);
            dialogHost.PopInternal(this);
            _taskCompletionSource.SetResult();
        }

        public Task ShowDialogAsync(IControl parent)
        {
            if (_taskCompletionSource != null)
                throw new NotImplementedException();

            _taskCompletionSource = new TaskCompletionSource();
            ShowDialogInternal(parent);
            return _taskCompletionSource.Task;
        }

        private static DialogHost GetDialogHost(IControl parent)
        {
            var window = parent.FindAncestorOfType<Window>(true);
            DialogHost dialogHost = window.GetValue(DialogHost.DialogHostProperty);
            return dialogHost;
        }

        private void InputElement_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!CancelOnEscape) return;
            if (e.Key is not (Key.Cancel or Key.Escape)) return;
            CloseDialog();
            e.Handled = true;
        }
    }
}