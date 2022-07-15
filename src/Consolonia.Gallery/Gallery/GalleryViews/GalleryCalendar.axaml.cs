﻿using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using JetBrains.Annotations;

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    [UsedImplicitly]
    [GalleryOrder(30)]
    public class GalleryCalendar : UserControl
    {
        public GalleryCalendar()
        {
            InitializeComponent();
            DateTime today = DateTime.Today; 
            var cal1 = this.Get<Calendar>("DisplayDatesCalendar");
            cal1.DisplayDateStart = today.AddDays(-25);
            cal1.DisplayDateEnd = today.AddDays(25);

            var cal2 = this.Get<Calendar>("BlackoutDatesCalendar");
            cal2.BlackoutDates.AddDatesInPast();
            cal2.BlackoutDates.Add(new CalendarDateRange(today.AddDays(6)));
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}