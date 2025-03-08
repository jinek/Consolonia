using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Chrome;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;
using Iciclecreek.Avalonia.WindowManager;

// ReSharper disable CheckNamespace

namespace Consolonia.Controls
{

    public enum MessageBoxStyle
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
        Cancel,
        Ok,
        Yes,
        No
    }

    public class MessageBox : Consolonia.Controls.Window
    {
        private Button _yesButton;
        private Button _noButton;
        private Button _cancelButton;
        private Button _okButton;

        /// <summary>
        /// Defines the <see cref="SizeToContent"/> property.
        /// </summary>
        public static readonly StyledProperty<object?> YesProperty =
            AvaloniaProperty.Register<ManagedWindow, object>(nameof(Yes));

        public static readonly StyledProperty<object?> NoProperty =
            AvaloniaProperty.Register<ManagedWindow, object>(nameof(No));

        public static readonly StyledProperty<object?> OkProperty =
            AvaloniaProperty.Register<ManagedWindow, object>(nameof(Ok));

        public static readonly StyledProperty<object?> CancelProperty =
            AvaloniaProperty.Register<ManagedWindow, object>(nameof(Cancel));

        public static readonly StyledProperty<object?> MessageProperty =
            AvaloniaProperty.Register<ManagedWindow, object>(nameof(Message));

        public static readonly StyledProperty<MessageBoxStyle> MessageBoxStyleProperty =
            AvaloniaProperty.Register<ManagedWindow, MessageBoxStyle>(nameof(MessageBoxStyle));

        public MessageBox()
        {
            base.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            base.SizeToContent = SizeToContent.WidthAndHeight;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            _yesButton = new Button()
            {
                Content = this.Yes ?? "_Yes",
                IsTabStop = true
            };
            _yesButton.Click += OnYes;

            _noButton = new Button()
            {
                Content = this.No ?? "_No",
                IsTabStop = true
            };
            _noButton.Click += OnNo;

            _cancelButton = new Button()
            {
                Content = this.Cancel ?? "_Cancel",
                IsCancel = true,
                IsTabStop = true
            };
            _cancelButton.Click += OnCancel;

            _okButton = new Button()
            {
                Content = this.Ok ?? "O_k",
                IsDefault = true,
                IsTabStop = true
            };
            _okButton.Click += OnOk;

            var buttonsStackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
            };

            switch (MessageBoxStyle)
            {
                case MessageBoxStyle.Ok:
                    buttonsStackPanel.Children.Add(_okButton);
                    break;
                case MessageBoxStyle.OkCancel:
                    buttonsStackPanel.Children.Add(_okButton);
                    buttonsStackPanel.Children.Add(_cancelButton);
                    break;
                case MessageBoxStyle.YesNo:
                    buttonsStackPanel.Children.Add(_yesButton);
                    buttonsStackPanel.Children.Add(_noButton);
                    break;
                case MessageBoxStyle.YesNoCancel:
                    buttonsStackPanel.Children.Add(_yesButton);
                    buttonsStackPanel.Children.Add(_noButton);
                    buttonsStackPanel.Children.Add(_cancelButton);
                    break;
            }
            buttonsStackPanel.Children[0].Focus();

            var message = (this.Message is string str) ? new TextBlock()
            {
                Text = str,
                TextWrapping = TextWrapping.Wrap
            } : this.Message as Control;

            this.Content = new StackPanel
            {
                Children =
                {
                    message,
                    buttonsStackPanel
                }
            };
        }


        public object Ok { get => GetValue(OkProperty); set => SetValue(OkProperty, value); }

        public object Yes { get => GetValue(YesProperty); set => SetValue(YesProperty, value); }

        public object No { get => GetValue(NoProperty); set => SetValue(NoProperty, value); }

        public object Cancel { get => GetValue(CancelProperty); set => SetValue(CancelProperty, value); }

        public object Message { get => GetValue(MessageProperty); set => SetValue(MessageProperty, value); }

        public MessageBoxStyle MessageBoxStyle { get => GetValue(MessageBoxStyleProperty); set => SetValue(MessageBoxStyleProperty, value); }

        public static Task<MessageBoxResult> ShowDialog(Visual visual, string title, string message, MessageBoxStyle style = MessageBoxStyle.Ok)
        {
            var mb = new MessageBox
            {
                MessageBoxStyle = style,
                Title = title,
                Message = message
            };
            return visual.ShowDialog<MessageBoxResult>(mb);
        }

        public async Task<MessageBoxResult> ShowDialog(Visual visual)
        {
            return await visual.ShowDialog<MessageBoxResult>(this);
        }

        private void OnOk(object sender, RoutedEventArgs e)
        {
            Close(MessageBoxResult.Ok);
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            Close(MessageBoxResult.Cancel);
        }

        private void OnYes(object sender, RoutedEventArgs e)
        {
            Close(MessageBoxResult.Yes);
        }

        private void OnNo(object sender, RoutedEventArgs e)
        {
            Close(MessageBoxResult.No);
        }
    }

    //public static class MessageBoxExtensions
    //{
    //    public static async Task<MessageBoxResult> ShowMessageBox(this Visual visual, string title, string message, MessageBoxStyle style = MessageBoxStyle.Ok)
    //    {
    //        var mb = new MessageBox
    //        {
    //            MessageBoxStyle = style,
    //            Title = title,
    //            Content = message
    //        };
    //        return await visual.ShowDialog<MessageBoxResult>(mb);
    //    }
    //}
}