using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Consolonia.Core.Drawing;

namespace Consolonia.Core.Infrastructure
{
    /// <summary>
    /// A readonly snapshot of rectangles.
    /// </summary>
    internal class Snapshot
    {
        private readonly IReadOnlyList<Rect> _rectangles;

        private Snapshot(IReadOnlyList<Rect> rectangles)
        {
            _rectangles = rectangles;
        }

        /// <summary>
        /// Checks if a point is contained within any rectangle.
        /// </summary>
        /// <param name="point">The point to check.</param>
        /// <param name="inclusive">If true, uses inclusive containment; if false, uses exclusive containment.</param>
        /// <returns>True if the point is contained, false otherwise.</returns>
        public bool Contains(Point point, bool inclusive)
        {
            return _rectangles.Any(rect => inclusive ? rect.Contains(point) : rect.ContainsExclusive(point));
        }


        /// <summary>
        /// A thread-safe collection of normalized rectangles.
        /// </summary>
        public class Regions
        {
            private readonly List<Rect> _rectangles = [];

            public void AddRect(Rect rect)
            {
                if (rect.IsEmpty())
                    return;

                lock (_rectangles)
                {
                    //todo: sometimes two rectangles can be replaced by one bigger rectangle if W or H matches
                    for (int i = 0; i < _rectangles.Count; i++)
                    {
                        Rect existingRect = _rectangles[i];
                        if (existingRect.Contains(rect))
                        {
                            return;
                        }

                        if (rect.Contains(existingRect))
                        {
                            _rectangles.RemoveAt(i);
                            i--;
                        }
                    }

                    _rectangles.Add(rect);
                }
            }

            public Snapshot GetSnapshotAndClear()
            {
                lock (_rectangles)
                {
                    var snapshot = new Snapshot([.. _rectangles]);
                    _rectangles.Clear();
                    return snapshot;
                }
            }
        }
    }
}