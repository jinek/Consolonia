using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Consolonia.PreviewHost.ViewModels;

public partial class XamlFileViewModel : ObservableObject
{
    public XamlFileViewModel(string xamlPath, Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        ArgumentNullException.ThrowIfNull(xamlPath);

        FullName = xamlPath;
        Name = Path.GetFileName(xamlPath);
        Assembly = assembly;
    }

    [ObservableProperty]
    private string? _name = null;

    [ObservableProperty]
    private string? _fullName = null;

    [ObservableProperty]
    private Assembly _assembly = null;

    private Control _content = null;
    public Control Content
    {
        get
        {
            if (_content == null)
            {
                string xaml = null!;
                int nTries = 0;
                while (xaml == null)
                {
                    try
                    {
                        xaml = File.ReadAllText(FullName!);
                    }
                    catch (IOException)
                    {
                        if (nTries++ < 3)
                        {

                            Thread.Sleep(100);
                            continue;
                        }
                        else
                            throw;
                    }
                }


                var control = (Control)AvaloniaRuntimeXamlLoader.Load(xaml, Assembly, designMode: false);

                var stackPanel = new StackPanel();
                stackPanel.HorizontalAlignment = HorizontalAlignment.Left;
                stackPanel.VerticalAlignment = VerticalAlignment.Top;
                stackPanel.Children.Add(control);
                Design.ApplyDesignModeProperties(stackPanel, control);

                _content = stackPanel;
            }
            return _content;
        }
    }
}

