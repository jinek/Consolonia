using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Data.Converters;

// ReSharper disable MemberCanBePrivate.Global

namespace Consolonia.Gallery.Gallery
{
    internal class GalleryItem
    {
        public GalleryItem(string name, Type type)
        {
            Type = type;
            Name = name;
        }

        public Type Type { get; }

        public string Name { get; }

        public static IEnumerable<GalleryItem> Enumerated
        {
            get
            {
                const string galleryPrefix = "Gallery";
                return Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(type =>
                        type.Namespace == "Consolonia.Gallery.Gallery.GalleryViews" &&
                        type.Name.StartsWith(galleryPrefix) &&
                        type.IsAssignableTo(typeof(UserControl)))
                    .OrderBy(GalleryOrderAttribute.GetOrder)
                    .Select(type => new GalleryItem(type.Name[galleryPrefix.Length..], type));
            }
        }
    }

    public class GalleryItemConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            return Activator.CreateInstance(((GalleryItem)value).Type);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}