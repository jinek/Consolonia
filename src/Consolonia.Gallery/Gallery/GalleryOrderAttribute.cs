using System;
using System.Reflection;

// ReSharper disable MemberCanBePrivate.Global

namespace Consolonia.Gallery.Gallery
{
    [AttributeUsage(AttributeTargets.Class)]
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