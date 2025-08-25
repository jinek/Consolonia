using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;

namespace Consolonia.Controls
{

    /// <summary>
    /// This is a unicode char as an icon.
    /// </summary>
    /// <remarks>
    /// It uses a viewbox to scale the character icon to fit the available space, allowing
    /// this to be used both in console apps and in graphical apps.
    /// </remarks>
    public class CharIcon : ContentControl
    {
        public static readonly StyledProperty<IconKind?> KindProperty =
            AvaloniaProperty.Register<CharIcon, IconKind?>(nameof(Kind));

        public static readonly StyledProperty<object> IconProperty =
            AvaloniaProperty.Register<CharIcon, object>(nameof(Icon));

        private Viewbox _viewbox;

        public CharIcon()
        {
            _viewbox = new Viewbox
            {
                Stretch = Avalonia.Media.Stretch.Uniform,
            };
            this.Content = _viewbox;
        }


        public IconKind? Kind
        {
            get => GetValue(KindProperty);
            set => SetValue(KindProperty, value);
        }

        public object Icon
        {
            get => GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            if (change.Property == KindProperty)
            {
                if (Kind.HasValue)
                    Icon = GetIcon(Kind.Value);
                else
                    Icon = " ";
            }
            else if (change.Property == IconProperty)
            {
                _viewbox.Child = new ContentPresenter() { Content = Icon };
            }


            base.OnPropertyChanged(change);
        }

        /// <summary>
        /// Dictionary mapping IconKind enum values to Unicode characters.
        /// </summary>
        public static readonly IReadOnlyDictionary<IconKind, string> IconMap = new Dictionary<IconKind, string>
        {
            // Navigation & UI
            [IconKind.Home] = "🏡",
            [IconKind.Search] = "🔍",
            [IconKind.Settings] = "⚙",
            [IconKind.Info] = "ℹ",
            [IconKind.Warning] = "⚠",
            [IconKind.Help] = "❓",
            [IconKind.Menu] = "☰",
            [IconKind.MoreHorizontal] = "⋯",
            [IconKind.MoreVertical] = "⋮",
            [IconKind.Refresh] = "🔄",
            [IconKind.ArrowLeft] = "←",
            [IconKind.ArrowRight] = "→",
            [IconKind.ArrowUp] = "↑",
            [IconKind.ArrowDown] = "↓",
            [IconKind.ArrowUpLeft] = "↖",
            [IconKind.ArrowUpRight] = "↗",
            [IconKind.ArrowDownLeft] = "↙",
            [IconKind.ArrowDownRight] = "↘",
            [IconKind.ChevronLeft] = "◀",
            [IconKind.ChevronRight] = "▶", 
            [IconKind.ChevronUp] = "▲",
            [IconKind.ChevronDown] = "▼",
            [IconKind.Play] = "▶",
            [IconKind.Pause] = "⏸",
            [IconKind.Stop] = "⏹",
            [IconKind.FastForward] = "⏩",
            [IconKind.Rewind] = "⏪",
            [IconKind.First] = "⏮",
            [IconKind.Last] = "⏭",
            [IconKind.Back] = "⇦",
            [IconKind.Forward] = "⇨",

            // Actions
            [IconKind.Add] = "＋",
            [IconKind.Remove] = "−",
            [IconKind.Close] = "✖",
            [IconKind.Edit] = "✎",
            [IconKind.Delete] = "🗑",
            [IconKind.Attach] = "📎",
            [IconKind.Tag] = "🏷",
            [IconKind.Bookmark] = "🔖",
            [IconKind.Share] = "🔗",
            [IconKind.Download] = "📥",
            [IconKind.Upload] = "📤",
            [IconKind.Filter] = "🔍",
            [IconKind.Sort] = "⇅",
            [IconKind.Open] = "📂",
            [IconKind.New] = "📄",
            [IconKind.Undo] = "⤺",
            [IconKind.Redo] = "⤻",
            [IconKind.Refresh2] = "⟳",
            [IconKind.Reload] = "🔃",
            [IconKind.Sync] = "🔄",
            [IconKind.Cancel] = "❌",
            [IconKind.Check] = "✓",
            [IconKind.Cross] = "✕",
            [IconKind.Plus] = "+",
            [IconKind.Minus] = "−",
            [IconKind.Maximize] = "🗖",
            [IconKind.Minimize] = "🗕",
            [IconKind.Restore] = "🗗",

            // Communication
            [IconKind.Mail] = "✉",
            [IconKind.Phone] = "📞",
            [IconKind.Chat] = "💬",
            [IconKind.Comment] = "💭",
            [IconKind.Lock] = "🔒",
            [IconKind.Unlock] = "🔓",
            [IconKind.Bell] = "🔔",
            [IconKind.BellOff] = "🔕",
            [IconKind.Send] = "📤",
            [IconKind.Receive] = "📥",
            [IconKind.Reply] = "⬏",
            [IconKind.ReplyAll] = "⮆",
            [IconKind.ReplyForward] = "⭜",

            // Files & Folders
            [IconKind.Folder] = "📁",
            [IconKind.FolderOpen] = "📂",
            [IconKind.Document] = "📄",
            [IconKind.Copy] = "🗐",
            [IconKind.Cut] = "✂",
            [IconKind.Paste] = "📋",
            [IconKind.Save] = "💾",
            [IconKind.Print] = "🖨",
            [IconKind.Archive] = "🗄",
            [IconKind.File] = "📄",
            [IconKind.FileText] = "📃",
            [IconKind.FileImage] = "🖼",
            [IconKind.FileVideo] = "📹",
            [IconKind.FileAudio] = "🎵",
            [IconKind.FilePdf] = "📕",
            [IconKind.FileZip] = "🗜",
            [IconKind.FileCode] = "📝",
            [IconKind.SaveAs] = "💾",
            [IconKind.Export] = "⭸",
            [IconKind.Import] = "⭶",
            [IconKind.Trashcan] = "🗑",
            [IconKind.Recycle] = "♻",

            // Media
            [IconKind.Camera] = "📷",
            [IconKind.Video] = "📹",
            [IconKind.Image] = "🖼",
            [IconKind.Music] = "🎵",
            [IconKind.VolumeUp] = "🔊",
            [IconKind.VolumeDown] = "🔉",
            [IconKind.VolumeMute] = "🔇",
            [IconKind.Microphone] = "🎤",
            [IconKind.Record] = "⏺",
            [IconKind.LivePhoto] = "📸",
            [IconKind.SkipNext] = "⏭",
            [IconKind.SkipPrevious] = "⏮",
            [IconKind.Shuffle] = "🔀",
            [IconKind.Repeat] = "🔁",
            [IconKind.RepeatOne] = "🔂",

            // Time & Date
            [IconKind.Calendar] = "📅",
            [IconKind.Clock] = "🕒",
            [IconKind.Alarm] = "⏰",
            [IconKind.Stopwatch] = "⏱",
            [IconKind.Timer] = "⏲",
            [IconKind.Schedule] = "📋",
            [IconKind.Event] = "🗓",
            [IconKind.Today] = "📅",
            [IconKind.History] = "🕘",

            // Location & Travel
            [IconKind.Location] = "📍",
            [IconKind.Map] = "🗺",
            [IconKind.Compass] = "🧭",
            [IconKind.Pin] = "📌",
            [IconKind.PinOff] = "📍",
            [IconKind.Navigation] = "🧭",
            [IconKind.Route] = "🛤",
            [IconKind.GPS] = "🛰",

            // Commerce
            [IconKind.ShoppingCart] = "🛒",
            [IconKind.CreditCard] = "💳",
            [IconKind.Gift] = "🎁",
            [IconKind.Purchase] = "💰",
            [IconKind.Sale] = "🏷",
            [IconKind.Receipt] = "🧾",

            // Weather
            [IconKind.Sun] = "☀",
            [IconKind.Moon] = "🌙",
            [IconKind.Cloud] = "☁",
            [IconKind.Rain] = "🌧",
            [IconKind.Snow] = "❄",
            [IconKind.Thunderstorm] = "⛈",
            [IconKind.Umbrella] = "☂",
            [IconKind.Wind] = "💨",
            [IconKind.Temperature] = "🌡",

            // Technology & Development
            [IconKind.Code] = "💻",
            [IconKind.Terminal] = "⌨",
            [IconKind.Database] = "🗃",
            [IconKind.Server] = "🖥",
            [IconKind.Network] = "🌐",
            [IconKind.Wifi] = "📶",
            [IconKind.WifiOff] = "📴",
            [IconKind.Bluetooth] = "📱",
            [IconKind.Power] = "⚡",
            [IconKind.PowerButton] = "⏻",
            [IconKind.Battery] = "🔋",
            [IconKind.PluggedIn] = "🔌",

            // User & People
            [IconKind.User] = "👤",
            [IconKind.Users] = "👥",
            [IconKind.Profile] = "👤",
            [IconKind.Account] = "👤",
            [IconKind.Group] = "👥",
            [IconKind.Team] = "👥",
            [IconKind.Contact] = "👤",
            [IconKind.Person] = "👤",

            // Status & Feedback
            [IconKind.Success] = "✅",
            [IconKind.Error] = "❌",
            [IconKind.Loading] = "⏳",
            [IconKind.Progress] = "📊",
            [IconKind.Complete] = "✔",
            [IconKind.Pending] = "⏳",
            [IconKind.Active] = "🟢",
            [IconKind.Inactive] = "🟡",
            [IconKind.Online] = "🟢",
            [IconKind.Busy] = "🔴",
            [IconKind.Away] = "🟡",
            [IconKind.Offline] = "⚪",

            // Misc
            [IconKind.Star] = "★",
            [IconKind.Heart] = "♥",
            [IconKind.HeartBroken] = "💔",
            [IconKind.Key] = "🔑",
            [IconKind.Lightbulb] = "💡",
            [IconKind.Trophy] = "🏆",
            [IconKind.Flag] = "🚩",
            [IconKind.Globe] = "🌐",
            [IconKind.Eye] = "👁",
            [IconKind.Question] = "❓",
            [IconKind.Exclamation] = "❗",
            [IconKind.Favorite] = "💖",
            [IconKind.Like] = "👍",
            [IconKind.Dislike] = "👎",
            [IconKind.ThumbsUp] = "👍",
            [IconKind.ThumbsDown] = "👎",
            [IconKind.Award] = "🏅",
            [IconKind.Certificate] = "📜",
            [IconKind.Shield] = "🛡",
            [IconKind.Security] = "🔒",
            [IconKind.Private] = "🔒",
            [IconKind.Public] = "🔓"
        };

        /// <summary>
        /// Gets the Unicode character for the specified IconKind.
        /// </summary>
        /// <param name="iconKind">The IconKind to get the character for.</param>
        /// <returns>The Unicode character, or a space if the IconKind is not found.</returns>
        private string GetIcon(IconKind iconKind)
        {
            if (this.TryFindResource($"IconKind_{iconKind}", out object textObj))
                return textObj.ToString();
            return IconMap.TryGetValue(iconKind, out string? icon) ? icon : " ";
        }
    }
}
