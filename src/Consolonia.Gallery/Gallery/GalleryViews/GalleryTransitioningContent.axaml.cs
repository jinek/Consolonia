#pragma warning disable CA1002 // Do not expose generic lists
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.VisualTree;

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    public partial class GalleryTransitioningContent : UserControl
    {
        public GalleryTransitioningContent()
        {
            InitializeComponent();
        }
    }

    public class TransitioningContentControlPageViewModel : ViewModelBase
    {
        private bool _clipToBounds;


        private int _duration = 500;
        private bool _reversed;


        private string _selectedParagraph;


        private PageTransition _selectedTransition;

        public TransitioningContentControlPageViewModel()
        {
            string[] loremIpsum =
            {
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
                "Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
                "Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo.",
                "Nemo enim ipsam voluptatem quia voluptas sit aspernatur aut odit aut fugit, sed quia consequuntur magni dolores eos qui ratione voluptatem sequi nesciunt. Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit.",
                "At vero eos et accusamus et iusto odio dignissimos ducimus qui blanditiis praesentium voluptatum deleniti atque corrupti quos dolores et quas molestias excepturi sint occaecati cupiditate non provident, similique sunt in culpa qui officia deserunt mollitia animi, id est laborum et dolorum fuga.",
                "Et harum quidem rerum facilis est et expedita distinctio. Nam libero tempore, cum soluta nobis est eligendi optio cumque nihil impedit quo minus id quod maxime placeat facere possimus, omnis voluptas assumenda est, omnis dolor repellendus.",
                "Temporibus autem quibusdam et aut officiis debitis aut rerum necessitatibus saepe eveniet ut et voluptates repudiandae sint et molestiae non recusandae. Itaque earum rerum hic tenetur a sapiente delectus, ut aut reiciendis voluptatibus maiores alias consequatur aut perferendis doloribus asperiores repellat.",
                "Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit, sed quia non numquam eius modi tempora incidunt ut labore et dolore magnam aliquam quaerat voluptatem. Ut enim ad minima veniam, quis nostrum exercitationem ullam corporis suscipit laboriosam, nisi ut aliquid ex ea commodi consequatur?",
                "Quis autem vel eum iure reprehenderit qui in ea voluptate velit esse quam nihil molestiae consequatur, vel illum qui dolorem eum fugiat quo voluptas nulla pariatur? At vero eos et accusamus et iusto odio dignissimos ducimus qui blanditiis praesentium voluptatum deleniti atque corrupti quos dolores et quas molestias excepturi sint occaecati cupiditate non provident.",
                "Ut enim ad minima veniam, quis nostrum exercitationem ullam corporis suscipit laboriosam, nisi ut aliquid ex ea commodi consequatur? Quis autem vel eum iure reprehenderit qui in ea voluptate velit esse quam nihil molestiae consequatur, vel illum qui dolorem eum fugiat quo voluptas nulla pariatur?"
            };

            Paragraphs.AddRange(loremIpsum);

            SetupTransitions();

            _selectedTransition = PageTransitions[1];
            _selectedParagraph = Paragraphs[0];
        }

        public List<PageTransition> PageTransitions { get; } = new();

        public List<string> Paragraphs { get; } = new();

        /// <summary>
        ///     Gets or Sets the selected image
        /// </summary>
        public string SelectedParagraph
        {
            get => _selectedParagraph;
            set => RaiseAndSetIfChanged(ref _selectedParagraph, value);
        }

        /// <summary>
        ///     Gets or sets the transition to play
        /// </summary>
        public PageTransition SelectedTransition
        {
            get => _selectedTransition;
            set => RaiseAndSetIfChanged(ref _selectedTransition, value);
        }

        /// <summary>
        ///     Gets or sets if the content should be clipped to bounds
        /// </summary>
        public bool ClipToBounds
        {
            get => _clipToBounds;
            set => RaiseAndSetIfChanged(ref _clipToBounds, value);
        }

        /// <summary>
        ///     Gets or Sets the duration
        /// </summary>
        public int Duration
        {
            get => _duration;
            set
            {
                RaiseAndSetIfChanged(ref _duration, value);
                SetupTransitions();
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the animation is reversed.
        /// </summary>
        public bool Reversed
        {
            get => _reversed;
            set => RaiseAndSetIfChanged(ref _reversed, value);
        }

        private void SetupTransitions()
        {
            PageTransitions.Clear();
            PageTransitions.AddRange(new[]
            {
                new PageTransition("None"),
                // Cross fade is not supported, we don't have opacity.
                // new PageTransition("CrossFade") { Transition = new CrossFade(TimeSpan.FromMilliseconds(Duration)) },
                new PageTransition("Slide horizontally")
                {
                    Transition = new PageSlide(TimeSpan.FromMilliseconds(Duration))
                },
                new PageTransition("Slide vertically")
                {
                    Transition = new PageSlide(TimeSpan.FromMilliseconds(Duration), PageSlide.SlideAxis.Vertical)
                },
                new PageTransition("Composite")
                {
                    Transition = new CompositePageTransition
                    {
                        PageTransitions = new List<IPageTransition>
                        {
                            new PageSlide(TimeSpan.FromMilliseconds(Duration)),
                            new PageSlide(TimeSpan.FromMilliseconds(Duration), PageSlide.SlideAxis.Vertical)
                        }
                    }
                },

                new PageTransition("Custom")
                {
                    Transition = new CustomTransition(TimeSpan.FromMilliseconds(Duration))
                }
            });

            if (SelectedTransition != null)
                SelectedTransition = PageTransitions.Single(x => x.DisplayTitle == SelectedTransition.DisplayTitle);
        }

        public void NextParagraph()
        {
            Reversed = false;
            int index = Paragraphs.IndexOf(SelectedParagraph) + 1;

            if (index >= Paragraphs.Count) index = 0;

            SelectedParagraph = Paragraphs[index];
        }

        public void PrevParagraph()
        {
            Reversed = true;
            int index = Paragraphs.IndexOf(SelectedParagraph) - 1;

            if (index < 0) index = Paragraphs.Count - 1;

            SelectedParagraph = Paragraphs[index];
        }
    }

    public class PageTransition : ViewModelBase
    {
        private IPageTransition _transition;

        public PageTransition(string displayTitle)
        {
            DisplayTitle = displayTitle;
        }

        public string DisplayTitle { get; }

        /// <summary>
        ///     Gets or sets the transition
        /// </summary>
        public IPageTransition Transition
        {
            get => _transition;
            set => RaiseAndSetIfChanged(ref _transition, value);
        }

        public override string ToString()
        {
            return DisplayTitle;
        }
    }

    public class CustomTransition : IPageTransition
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CustomTransition" /> class.
        /// </summary>
        public CustomTransition()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CustomTransition" /> class.
        /// </summary>
        /// <param name="duration">The duration of the animation.</param>
        public CustomTransition(TimeSpan duration)
        {
            Duration = duration;
        }

        /// <summary>
        ///     Gets the duration of the animation.
        /// </summary>
        public TimeSpan Duration { get; set; }

        public async Task Start(Visual from, Visual to, bool forward, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) return;

            var tasks = new List<Task>();
            Visual parent = GetVisualParent(from, to);
            StyledProperty<double> scaleProperty = ScaleTransform.ScaleYProperty;

            if (from != null)
            {
                Animation animation = CreateAnimation(scaleProperty, 1d, 0d);
                tasks.Add(animation.RunAsync(from, cancellationToken));
            }

            if (to != null)
            {
                to.IsVisible = true;
                Animation animation = CreateAnimation(scaleProperty, 0d, 1d);
                tasks.Add(animation.RunAsync(to, cancellationToken));
            }

            await Task.WhenAll(tasks);

            if (from != null && !cancellationToken.IsCancellationRequested) from.IsVisible = false;
        }

        private Animation CreateAnimation(AvaloniaProperty scaleProperty, double firstFrameValue, double lastFrameValue)
        {
            return new Animation
            {
                Children =
                {
                    new KeyFrame
                    {
                        Setters = { new Setter { Property = scaleProperty, Value = firstFrameValue } },
                        Cue = new Cue(0d)
                    },
                    new KeyFrame
                    {
                        Setters = { new Setter { Property = scaleProperty, Value = lastFrameValue } },
                        Cue = new Cue(1d)
                    }
                },
                Duration = Duration
            };
        }

        /// <summary>
        ///     Gets the common visual parent of the two control.
        /// </summary>
        /// <param name="from">The from control.</param>
        /// <param name="to">The to control.</param>
        /// <returns>The common parent.</returns>
        /// <exception cref="ArgumentException">
        ///     The two controls do not share a common parent.
        /// </exception>
        /// <remarks>
        ///     Any one of the parameters may be null, but not both.
        /// </remarks>
        private static Visual GetVisualParent(Visual from, Visual to)
        {
            Visual p1 = (from ?? to)!.GetVisualParent();
            Visual p2 = (to ?? from)!.GetVisualParent();

            if (p1 != null && p2 != null && p1 != p2)
                throw new ArgumentException("Controls for PageSlide must have same parent.");

            return p1 ?? throw new InvalidOperationException("Cannot determine visual parent.");
        }
    }
}