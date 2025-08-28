using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    public partial class GalleryNotifications : UserControl
    {
        private readonly NotificationViewModel _viewModel;

        public GalleryNotifications()
        {
            InitializeComponent();

            _viewModel = new NotificationViewModel();

            DataContext = _viewModel;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            _viewModel.NotificationManager = new WindowNotificationManager(TopLevel.GetTopLevel(this)!);
        }

        public void NotificationOnClick()
        {
            this.Get<WindowNotificationManager>("ControlNotifications").Show("Notification clicked");
        }
    }

    public class NotificationViewModel
    {
        public NotificationViewModel()
        {
            ShowTextManagedNotificationCommand = MiniCommand.Create(() =>
            {
                NotificationManager?.Show($"It's {DateTime.Now.TimeOfDay}!", NotificationType.Information);
            });

            ShowCustomManagedNotificationCommand = MiniCommand.Create(() =>
            {
                var viewModel = new NotificationViewModel
                {
                    Title = "Hey There!",
                    Message = "Did you know that Avalonia now supports Custom In-Window Notifications?",
                    NotificationManager = NotificationManager
                };
                NotificationManager?.Show(
                    new CustomNotificationView { Foreground = Brushes.White, DataContext = viewModel },
                    NotificationType.Warning);
            });

            ShowManagedNotificationCommand = MiniCommand.Create(() =>
            {
                NotificationManager?.Show(new Notification("Welcome", "Avalonia now supports Notifications!"));
            });

            YesCommand = MiniCommand.Create(() =>
            {
                NotificationManager?.Show(new Notification("Avalonia Notifications",
                    "Start adding notifications to your app today."));
            });

            NoCommand = MiniCommand.Create(() =>
            {
                NotificationManager?.Show(new Notification("Avalonia Notifications",
                    "Start adding notifications to your app today. To find out more visit..."));
            });
        }

        public WindowNotificationManager NotificationManager { get; set; }

        public string Title { get; set; }
        public string Message { get; set; }

        public MiniCommand YesCommand { get; }

        public MiniCommand NoCommand { get; }

        public MiniCommand ShowCustomManagedNotificationCommand { get; }

        public MiniCommand ShowManagedNotificationCommand { get; }
        public MiniCommand ShowTextManagedNotificationCommand { get; }
    }
}