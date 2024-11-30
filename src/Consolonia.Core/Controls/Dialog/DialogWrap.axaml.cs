using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.Reactive;
using Avalonia.VisualTree;

namespace Consolonia.Core.Controls.Dialog
{
    [SuppressMessage("Usage", "PartialTypeWithSinglePart",
        Justification = "Partial class required for XAML code generation.")]
    // ReSharper disable once PartialTypeWithSinglePart
    internal partial class DialogWrap : UserControl
    {
        public readonly ContentPresenter FoundContentPresenter;
        private IDisposable _disposable;

        public DialogWrap()
        {
            InitializeComponent();
            FoundContentPresenter = this.FindNameScope()?.Find<ContentPresenter>("ContentPresenter");

            AttachedToVisualTree += (_, _) =>
            {
                var parentWindow = this.FindAncestorOfType<Window>();
                _disposable = parentWindow!.GetPropertyChangedObservable(TopLevel.ClientSizeProperty).Subscribe(
                    new AnonymousObserver<AvaloniaPropertyChangedEventArgs>(
                        args =>
                        {
                            var newSize = (Size)args.NewValue!;

                            SetNewSize(newSize);
                        }));
                SetNewSize(parentWindow!.ClientSize);
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

        public void SetContent(DialogWindow dialogWindow)
        {
            FoundContentPresenter.Content = dialogWindow;
        }

        // ReSharper disable once UnusedMember.Local Example of usage for further (when mouse support introduced for example)
#pragma warning disable IDE0051
        private void CloseDialog()
#pragma warning restore IDE0051
        {
            ((DialogWindow)FoundContentPresenter!.Content!)!.CloseDialog();
        }
    }
}