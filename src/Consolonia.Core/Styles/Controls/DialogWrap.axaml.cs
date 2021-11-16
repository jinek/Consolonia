using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using Consolonia.Core.Infrastructure;
using DynamicData.Binding;

namespace Consolonia.Core.Styles.Controls
{
    internal class DialogWrap : UserControl
    {
        private Window _parentWindow;
        private IDisposable _disposable;
        private IDisposable _disposable2;

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

        /*
        protected override Size MeasureOverride(Size availableSize)
        {
            _parentWindow = this.FindAncestorOfType<Window>();
            /*
            
            

            Width = ContentPresenter.DesiredSize.Width;
            Height = ContentPresenter.DesiredSize.Height;#1#
            /*ContentPresenter.Measure(_parentWindow.ClientSize);
            return ContentPresenter.DesiredSize;#1#
            ContentPresenter.Measure(_parentWindow.ClientSize);
            
            Size measureOverride = base.MeasureOverride(ContentPresenter.DesiredSize);
            return measureOverride;
            return _parentWindow.ClientSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _parentWindow = this.FindAncestorOfType<Window>();
            ContentPresenter.Arrange(new Rect(_parentWindow.ClientSize)));
            
            Size arrangeOverride = base.ArrangeOverride(_parentWindow.ClientSize);
            return arrangeOverride;
            /*
            //return ContentPresenter.Arrange();
            base.ArrangeOverride(_parentWindow.ClientSize);
            return _parentWindow.ClientSize;#1#
        }*/

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void SetContent(IControl control)
        {
            _disposable2?.Dispose();
            _disposable2 = control.GetPropertyChangedObservable(BoundsProperty).Subscribe(args =>
            {
                var rect = (Rect)args.NewValue;
                Border.Width = rect.Width + 2;
                Border.Height = rect.Height + 2;
            });
            ContentPresenter.Content = control;
        }

        private ContentPresenter ContentPresenter => this.Get<ContentPresenter>("ContentPresenter");
        private Control Border => this.Get<Control>("Border");

        private void Close_Clicked(object? sender, RoutedEventArgs e)
        {
            CloseDialog();
        }

        private void CloseDialog()
        {
            ((Control)Content).CloseDialog();
        }

        private void InputElement_OnKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key is Key.Cancel or Key.Escape)
            {
                CloseDialog();
                e.Handled = true;
            }
        }
    }
}