using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

// ReSharper disable UnusedMember.Global

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    public partial class GalleryScrollViewer : UserControl
    {
        public GalleryScrollViewer()
        {
            InitializeComponent();

            DataContext = new ScrollViewerPageViewModel();
        }
    }

    public class ScrollViewerPageViewModel : ViewModelBase
    {
        private readonly bool _allowAutoHide;

        private readonly string[] _cows = Array.Empty<string>();
        private readonly ScrollBarVisibility _horizontalScrollVisibility;
        private readonly ScrollBarVisibility _verticalScrollVisibility;

        public ScrollViewerPageViewModel()
        {
            AvailableVisibility =
            [
                ScrollBarVisibility.Auto,
                ScrollBarVisibility.Visible,
                ScrollBarVisibility.Hidden,
                ScrollBarVisibility.Disabled
            ];

            HorizontalScrollVisibility = ScrollBarVisibility.Auto;
            VerticalScrollVisibility = ScrollBarVisibility.Visible;
            AllowAutoHide = true;
            var text = new List<string>();
            for (int i = 0; i < 30; i++)
                text.AddRange("""
                                                                     /;    ;\
                                                                 __  \\____//
                                                                /{_\_/   `'\____
                                                                \___   (o)  (o  }
                                     _____________________________/          :--'  
                                 ,-,'`@@@@@@@@       @@@@@@         \_    `__\
                                ;:(  @@@@@@@@@        @@@             \___(o'o)
                                :: )  @@@@          @@@@@@        ,'@@(  `===='       
                                :: : @@@@@:          @@@@         `@@@:
                                :: \  @@@@@:       @@@@@@@)    (  '@@@'
                                ;; /\      /`,    @@@@@@@@@\   :@@@@@)
                                ::/  )    {_----------------:  :~`,~~;
                               ;;'`; :   )                  :  / `; ;
                              ;;;; : :   ;                  :  ;  ; :              
                              `'`' / :  :                   :  :  : :
                                  )_ \__;      ";"          :_ ;  \_\       `,','
                                  :__\  \    * `,'*         \  \  :  \   *  8`;'*  *
                                      `^'     \ :/           `^'  `-^-'   \v/ :  \/ 
                              Bill Ames

                              """.Split((char[]) ['\r', '\n'], StringSplitOptions.RemoveEmptyEntries));
            int lineNumber = 1;
            Cows = [.. text.Select(l => $"{lineNumber++,3} {l}")];
        }

        public bool AllowAutoHide
        {
            get => _allowAutoHide;
            init => RaiseAndSetIfChanged(ref _allowAutoHide, value);
        }

        public ScrollBarVisibility HorizontalScrollVisibility
        {
            get => _horizontalScrollVisibility;
            init => RaiseAndSetIfChanged(ref _horizontalScrollVisibility, value);
        }

        public ScrollBarVisibility VerticalScrollVisibility
        {
            get => _verticalScrollVisibility;
            init => RaiseAndSetIfChanged(ref _verticalScrollVisibility, value);
        }

        public IReadOnlyList<ScrollBarVisibility> AvailableVisibility { get; init; }

#pragma warning disable CA1819 // Properties should not return arrays
        public string[] Cows
#pragma warning restore CA1819 // Properties should not return arrays
        {
            get => _cows;
            init => RaiseAndSetIfChanged(ref _cows, value);
        }
    }

    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // ReSharper disable once UnusedMethodReturnValue.Global
        protected bool RaiseAndSetIfChanged<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                RaisePropertyChanged(propertyName);
                return true;
            }

            return false;
        }


        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}