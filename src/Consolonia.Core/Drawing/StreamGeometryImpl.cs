using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Avalonia;
using Avalonia.Media;
using Avalonia.Platform;

namespace Consolonia.Core.Drawing
{
    internal class StreamGeometryImpl : IStreamGeometryImpl
    {
        private readonly List<Rectangle> _fills;
        private readonly List<Line> _strokes;

        public StreamGeometryImpl()
        {
            _strokes = new List<Line>();
            _fills = new List<Rectangle>();
        }

        public IReadOnlyList<Line> Strokes => _strokes;

        public IReadOnlyList<Rectangle> Fills => _fills;
        // private SKPath _path;

        public Rect Bounds { get; private set; }

        public double ContourLength => _strokes.Sum(l => l.ContourLength);


        public IStreamGeometryImpl Clone()
        {
            var cloneGeometry = new StreamGeometryImpl();
            foreach (Line cloneLine in _strokes.Select(l => new Line(l.PStart, l.PEnd, this, l.Transform)))
                cloneGeometry._strokes.Add(cloneLine);
            foreach (Rectangle cloneRect in _fills.Select(r => new Rectangle(r.Rect, r, r.Transform)))
                cloneGeometry._fills.Add(cloneRect);
            cloneGeometry.Bounds = Bounds;
            return cloneGeometry;
        }

        public bool FillContains(Point point)
        {
            return _fills.Any(rect => rect.FillContains(point));
        }

        public Rect GetRenderBounds(IPen pen)
        {
            Rect strokeBounds = _strokes.Aggregate(new Rect(), (rect, line) => rect.Union(line.GetRenderBounds(pen)));
            return _fills.Aggregate(strokeBounds, (rect, r) => rect.Union(r.GetRenderBounds(pen)));
        }

        public IGeometryImpl GetWidenedGeometry(IPen pen)
        {
            // TODO
            throw new NotImplementedException();
        }

        public IGeometryImpl Intersect(IGeometryImpl geometry)
        {
            return geometry.Intersect(this);
        }

        public IStreamGeometryContextImpl Open()
        {
            return new StreamGeometryContextImpl(this);
        }

        public bool StrokeContains(IPen pen, Point point)
        {
            return _strokes.Any(line => line.StrokeContains(pen, point));
        }

        public bool TryGetPointAndTangentAtDistance(double distance, out Point point, out Point tangent)
        {
            throw new NotImplementedException();
        }

        public bool TryGetPointAtDistance(double distance, out Point point)
        {
            throw new NotImplementedException();
        }

        public bool TryGetSegment(double startDistance, double stopDistance, bool startOnBeginFigure,
            [NotNullWhen(true)] out IGeometryImpl segmentGeometry)
        {
            throw new NotImplementedException();
        }

        public ITransformedGeometryImpl WithTransform(Matrix transform)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     A Conolonia implementation of a <see cref="IStreamGeometryContextImpl" />.
        /// </summary>
        private class StreamGeometryContextImpl : IStreamGeometryContextImpl, IGeometryContext2
        {
            private readonly StreamGeometryImpl _geometryImpl;
            private bool _isFilled;
            private Point _lastPoint;

            /// <summary>
            ///     Initializes a new instance of the StreamGeometryContextImpl class.
            ///     <param name="geometryImpl">Geometry to operate on.</param>
            /// </summary>
            public StreamGeometryContextImpl(StreamGeometryImpl geometryImpl)
            {
                _geometryImpl = geometryImpl;
            }

            /// <inheritdoc />
            public void LineTo(Point point, bool isStroked)
            {
                LineTo(point);
            }

            /// <inheritdoc />
            public void ArcTo(Point point, Size size, double rotationAngle, bool isLargeArc,
                SweepDirection sweepDirection, bool isStroked)
            {
                _lastPoint = point;
            }

            /// <inheritdoc />
            public void CubicBezierTo(Point point1, Point point2, Point point3, bool isStroked)
            {
                throw new NotSupportedException();
            }

            /// <inheritdoc />
            public void QuadraticBezierTo(Point point1, Point point2, bool isStroked)
            {
                throw new NotSupportedException();
            }

            /// <inheritdoc />
            public void ArcTo(Point point, Size size, double rotationAngle, bool isLargeArc,
                SweepDirection sweepDirection)
            {
                // ignore arc instructions. It's attempt to draw rounded corners, we don't do that.
                //_lastPoint = point;
            }

            /// <inheritdoc />
            public void BeginFigure(Point startPoint, bool isFilled)
            {
                _isFilled = isFilled;
                _lastPoint = startPoint;
            }

            /// <inheritdoc />
            public void CubicBezierTo(Point point1, Point point2, Point point3)
            {
                throw new NotSupportedException();
            }

            /// <inheritdoc />
            public void QuadraticBezierTo(Point point1, Point point2)
            {
                throw new NotSupportedException();
            }

            /// <inheritdoc />
            public void LineTo(Point point)
            {
                // our strokes are oriented from UpperLeft corner to Right or Down
                if (_lastPoint.X > point.X || _lastPoint.Y > point.Y)
                    _geometryImpl._strokes.Add(new Line(point, _lastPoint));
                else
                    _geometryImpl._strokes.Add(new Line(_lastPoint, point));
                _lastPoint = point;
            }

            /// <inheritdoc />
            public void EndFigure(bool isClosed)
            {
                Rect bound = _geometryImpl._strokes.Aggregate(new Rect(), (rect, line) => rect.Union(line.Bounds));
                _geometryImpl.Bounds = bound;
                if (_isFilled) _geometryImpl._fills.Add(new Rectangle(bound));
            }

            /// <inheritdoc />
            public void SetFillRule(FillRule fillRule)
            {
            }

            public void Dispose()
            {
            }
        }
    }
}