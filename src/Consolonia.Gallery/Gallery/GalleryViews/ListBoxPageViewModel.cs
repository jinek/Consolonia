#pragma warning disable CA5394 // Do not use insecure randomness
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Selection;
using ReactiveUI;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    public class ListBoxPageViewModel : ViewModelBase
    {
        private bool _alwaysSelected;
        private bool _autoScrollToSelectedItem = true;
        private int _counter;
        private bool _multiple;
        private bool _toggle;

        public ListBoxPageViewModel()
        {
            Items = new ObservableCollection<string>(Enumerable.Range(1, 10000).Select(_ => GenerateItem()));

            Selection = new SelectionModel<string>();
            Selection.Select(1);

            SelectionMode = this.WhenAnyValue(
                x => x.Multiple,
                x => x.Toggle,
                x => x.AlwaysSelected,
                (m, t, a) =>
                    (m ? Avalonia.Controls.SelectionMode.Multiple : 0) |
                    (t ? Avalonia.Controls.SelectionMode.Toggle : 0) |
                    (a ? Avalonia.Controls.SelectionMode.AlwaysSelected : 0));

            AddItemCommand = MiniCommand.Create(() => Items.Add(GenerateItem()));

            RemoveItemCommand = MiniCommand.Create(() =>
            {
                List<string> items = Selection.SelectedItems.ToList();

                foreach (string item in items) Items.Remove(item);
            });

            SelectRandomItemCommand = MiniCommand.Create(() =>
            {
                using (Selection.BatchUpdate())
                {
                    Selection.Clear();
                    Selection.Select(Random.Shared.Next(Items.Count - 1));
                }
            });
        }

        public ObservableCollection<string> Items { get; }
        public SelectionModel<string> Selection { get; }
        public IObservable<SelectionMode> SelectionMode { get; }

        public bool Multiple
        {
            get => _multiple;
            set => RaiseAndSetIfChanged(ref _multiple, value);
        }

        public bool Toggle
        {
            get => _toggle;
            set => RaiseAndSetIfChanged(ref _toggle, value);
        }

        public bool AlwaysSelected
        {
            get => _alwaysSelected;
            set => RaiseAndSetIfChanged(ref _alwaysSelected, value);
        }

        public bool AutoScrollToSelectedItem
        {
            get => _autoScrollToSelectedItem;
            set => RaiseAndSetIfChanged(ref _autoScrollToSelectedItem, value);
        }

        public MiniCommand AddItemCommand { get; }
        public MiniCommand RemoveItemCommand { get; }
        public MiniCommand SelectRandomItemCommand { get; }

        private string GenerateItem()
        {
            return $"Item {_counter++}";
        }
    }
}