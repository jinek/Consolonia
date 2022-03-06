using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using Consolonia.Themes.TurboVision.Templates.Controls.Dialog;

namespace Consolonia.Themes.TurboVision.Templates.Controls
{
    internal class DialogWrap : UserControl
    {
        private Window _parentWindow;
        private IDisposable _disposable;
        private IDisposable _disposable2;
        private DialogWindow _dialogWindow;

        public DialogWrap()
        {
            InitializeComponent();
            AttachedToVisualTree += (sender, args) =>
            {
                _parentWindow = this.FindAncestorOfType<Window>();
                _disposable = _parentWindow.GetPropertyChangedObservable(Window.ClientSizeProperty).Subscribe(args =>
                {
                    var newSize = (Size)args.NewValue;

                    SetNewSize(newSize);
                });
                SetNewSize(_parentWindow.ClientSize);
            };
            DetachedFromLogicalTree += (sender, args) => { _disposable.Dispose(); };
        }

        private void SetNewSize(Size newSize)
        {
            Width = newSize.Width;
            Height = newSize.Height;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void SetContent(DialogWindow dialogWindow)
        {
            /*_disposable2?.Dispose();
            _disposable2 = dialogWindow.GetPropertyChangedObservable(BoundsProperty).Subscribe(args =>
            {
                var rect = (Rect)args.NewValue;
                DialogPanelBorder.Width = rect.Width + 2;
                DialogPanelBorder.Height = rect.Height + 2;
            });*/
            _dialogWindow = dialogWindow;
            ContentPresenter.Content = dialogWindow;
        }

        internal ContentPresenter ContentPresenter => this.Get<ContentPresenter>("ContentPresenter");

        /// <summary>
        ///     Focused element when new dialog shown
        ///     This is focus to restore when dialog closed 
        /// </summary>
        internal IInputElement? HadFocusOn { get; set; }

        private void CloseDialog()
        {
            ((DialogWindow)ContentPresenter.Content).CloseDialog();
        }
    }
}