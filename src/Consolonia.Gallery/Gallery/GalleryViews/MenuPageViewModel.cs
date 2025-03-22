#pragma warning disable CA1822 // Mark members as static

using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia.Controls;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    public class MenuPageViewModel
    {
        public MenuPageViewModel()
        {
            OpenCommand = MiniCommand.CreateFromTask(Open);
            SaveCommand = MiniCommand.Create(Save);
            OpenRecentCommand = MiniCommand.Create<string>(OpenRecent);

            var recentItems = new[]
            {
                new MenuItemViewModel
                {
                    Header = "File1.txt",
                    Command = OpenRecentCommand,
                    CommandParameter = @"c:\foo\File1.txt"
                },
                new MenuItemViewModel
                {
                    Header = "File2.txt",
                    Command = OpenRecentCommand,
                    CommandParameter = @"c:\foo\File2.txt"
                }
            };

            RecentItems = recentItems;
            MenuItems = new[]
            {
                new MenuItemViewModel
                {
                    Header = "_File",
                    Items = new[]
                    {
                        new MenuItemViewModel { Header = "O_pen...", Command = OpenCommand },
                        new MenuItemViewModel { Header = "Save", Command = SaveCommand },
                        new MenuItemViewModel { Header = "-" },
                        new MenuItemViewModel
                        {
                            Header = "Recent",
                            Items = recentItems
                        }
                    }
                },
                new MenuItemViewModel
                {
                    Header = "_Edit",
                    Items = new[]
                    {
                        new MenuItemViewModel { Header = "_Copy" },
                        new MenuItemViewModel { Header = "_Paste" }
                    }
                }
            };
        }

        public Control View { get; set; }

        public IReadOnlyList<MenuItemViewModel> MenuItems { get; }
        public IReadOnlyList<MenuItemViewModel> RecentItems { get; }
        public MiniCommand OpenCommand { get; }
        public MiniCommand SaveCommand { get; }
        public MiniCommand OpenRecentCommand { get; }

        public Task Open()
        {
            return Task.CompletedTask;
            /*todo: dialog does not work
             var window = View?.GetVisualRoot() as Window;
            if (window == null)
                return;
            var dialog = new OpenFileDialog();
            var result = await dialog.ShowAsync(window);

            if (result != null)
            {
                foreach (var path in result)
                {
                    System.Diagnostics.Debug.WriteLine($"Opened: {path}");
                }
            }*/
        }

        public void Save()
        {
            Debug.WriteLine("Save");
        }

        public void OpenRecent(string path)
        {
            Debug.WriteLine($"Open recent: {path}");
        }
    }
}