using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Avalonia.Controls;
using Consolonia.Controls;

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    public partial class GalleryIcons: UserControl
    {
        List<IconKind> _kinds = Enum.GetValues<IconKind>().ToList();

        public GalleryIcons()
        {
            InitializeComponent();

            this.DataContext = this;
        }

        public IEnumerable<IconKind> Kinds => _kinds;
    }
}