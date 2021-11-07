using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml.Styling;
using Consolonia.Gallery.View;

namespace Consolonia.Gallery
{
    internal class App : Application
    {
        public App()
        {
            Styles.Add(new StyleInclude(new Uri("avares://Consolonia.Gallery"))
            {
                Source = new Uri(@"avares://Consolonia.Core/Styles/DarkStyles.axaml")
            });
        }

        public override void OnFrameworkInitializationCompleted()
        {
            ((IClassicDesktopStyleApplicationLifetime)ApplicationLifetime).MainWindow = new ControlsListView();
            base.OnFrameworkInitializationCompleted();
        }
    }
}