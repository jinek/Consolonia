using System;
using System.Reflection;

namespace Consolonia.Gallery.Gallery
{
    public class GalleryOrderAttribute : Attribute
    {
        public GalleryOrderAttribute(int order)
        {
            Order = order;
        }

        public int Order { get; }

        public static int GetOrder(Type type)
        {
            return type.GetCustomAttribute<GalleryOrderAttribute>()?.Order ?? 1000;
        }
    }
}