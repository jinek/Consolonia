using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Reactive;
using Avalonia.VisualTree;
using Consolonia.Themes.TurboVision.Templates.Controls.Dialog;

namespace Consolonia.Themes.TurboVision.Templates.Controls
{
    internal partial class DialogWrap : UserControl
    {
        private IDisposable _disposable;

        public DialogWrap()
        {
            InitializeComponent();
            AttachedToVisualTree += (_, _) =>
            {
                var parentWindow = this.FindAncestorOfType<Window>();
                _disposable = parentWindow.GetPropertyChangedObservable(TopLevel.ClientSizeProperty).Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs>(
                    args =>
                    {
                        var newSize = (Size)args.NewValue!;

                        SetNewSize(newSize);
                    }));
                SetNewSize(parentWindow.ClientSize);
            };
            DetachedFromLogicalTree += (_, _) => { _disposable.Dispose(); };
        }

        /// <summary>
        ///     Focused element when new dialog shown
        ///     This is focus to restore when dialog closed
        /// </summary>
        internal IInputElement HadFocusOn { get; set; }

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
            ContentPresenter.Content = dialogWindow;
        }

        // ReSharper disable once UnusedMember.Local Example of usage for further (when mouse support introduced for example)
#pragma warning disable IDE0051
        private void CloseDialog()
#pragma warning restore IDE0051
        {
            ((DialogWindow)ContentPresenter.Content).CloseDialog();
        }
    }
}