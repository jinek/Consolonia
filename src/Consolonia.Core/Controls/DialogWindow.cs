using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.VisualTree;
using Consolonia.Core.Controls.Dialog;

// ReSharper disable MemberCanBeProtected.Global

namespace Consolonia.Core.Controls
{
    [TemplatePart("PART_ContentPresenter", typeof(ContentPresenter))]
    public class DialogWindow : UserControl
    {
        public static readonly DirectProperty<DialogWindow, Size> ContentSizeProperty =
            AvaloniaProperty.RegisterDirect<DialogWindow, Size>(nameof(ContentSize), window => window.ContentSize);

        public static readonly StyledProperty<string> TitleProperty = Window.TitleProperty.AddOwner<DialogWindow>();

        public static readonly StyledProperty<bool> IsCloseButtonVisibleProperty =
            AvaloniaProperty.Register<DialogWindow, bool>(nameof(IsCloseButtonVisible), true);

        public static readonly StyledProperty<WindowStartupLocation> WindowStartupLocationProperty =
            AvaloniaProperty.Register<DialogWindow, WindowStartupLocation>(nameof(WindowStartupLocation));

        public static readonly StyledProperty<bool> CanResizeProperty =
            AvaloniaProperty.Register<DialogWindow, bool>(nameof(CanResize), true);

        public static readonly StyledProperty<string> IconProperty =
            AvaloniaProperty.Register<DialogWindow, string>(nameof(Icon));

        private Size _contentSize;
        private ContentPresenter _partContentPresenter;

        private TaskCompletionSource<object> _taskCompletionSource;


        static DialogWindow()
        {
            TitleProperty.OverrideDefaultValue<DialogWindow>(string.Empty);
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

        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        // ReSharper disable once MemberCanBePrivate.Global
        public bool CancelOnEscape { get; set; } = true;


        /// <summary>
        ///     Enables or disables resizing of the window.
        /// </summary>
        public bool CanResize
        {
            get => GetValue(CanResizeProperty);
            set => SetValue(CanResizeProperty, value);
        }

        /// <summary>
        ///     Gets or sets the icon of the window.
        /// </summary>
        public string Icon
        {
            get => GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        /// <summary>
        ///     Gets or sets the startup location of the window.
        /// </summary>
        public WindowStartupLocation WindowStartupLocation
        {
            get => GetValue(WindowStartupLocationProperty);
            set => SetValue(WindowStartupLocationProperty, value);
        }

        /// <summary>
        ///     Gets or sets the window position in screen coordinates.
        /// </summary>
        //public PixelPoint Position
        //{
        //    get => PlatformImpl?.Position ?? PixelPoint.Origin;
        //    set => PlatformImpl?.Move(value);
        //}

        // ReSharper disable once UnusedMember.Global Used by template
        public void CloseClick()
        {
            if (CancelOnEscape)
                CloseDialog();
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            _partContentPresenter = e.NameScope.Find<ContentPresenter>("PART_ContentPresenter");
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Size arrangeOverride = base.ArrangeOverride(finalSize);
            Visual firstVisualChild = _partContentPresenter?.GetVisualChildren().FirstOrDefault();
            if (firstVisualChild != null)
                ContentSize = firstVisualChild.Bounds.Size;
            return arrangeOverride;
        }

        protected void ShowDialogInternal(Visual parent)
        {
            if (WindowStartupLocation == WindowStartupLocation.CenterScreen)
            {
                Width = (ushort)(parent.Bounds.Width * .9);
                Height = (ushort)(parent.Bounds.Height * .9);
                if (parent is Window window) window.SizeChanged += Window_SizeChanged;
            }

            DialogHost dialogHost = GetDialogHost(parent);
            dialogHost.OpenInternal(this);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var window = sender as Window;
            if (window != null)
            {
                Width = (ushort)(window.Bounds.Width * .9);
                Height = (ushort)(window.Bounds.Height * .9);
            }
        }

        // ReSharper disable once VirtualMemberNeverOverridden.Global overriden in other packages, why resharper suggests this?
        public virtual void CloseDialog(object result = null)
        {
            DialogHost dialogHost = GetDialogHost(this);
            dialogHost.PopInternal(this);
            if (Parent is Window window) window.SizeChanged -= Window_SizeChanged;
            _taskCompletionSource.SetResult(result);
        }

        public async Task ShowDialogAsync(Control parent)
        {
            if (_taskCompletionSource != null)
                throw new NotImplementedException();

            _taskCompletionSource = new TaskCompletionSource<object>();
            ShowDialogInternal(parent);
            await _taskCompletionSource.Task;
        }

        public async Task<T> ShowDialogAsync<T>(Control parent)
        {
            if (_taskCompletionSource != null)
                throw new NotImplementedException();

            _taskCompletionSource = new TaskCompletionSource<object>();
            ShowDialogInternal(parent);
            object result = await _taskCompletionSource.Task;
            return (T)result;
        }

        protected static DialogHost GetDialogHost(Visual parent)
        {
            var window = parent.FindAncestorOfType<Window>(true);
            DialogHost dialogHost = window!.GetValue(DialogHost.DialogHostProperty);
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