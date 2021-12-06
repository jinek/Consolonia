using System;
using Avalonia;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Skia;

namespace Consolonia.Core.Drawing
{
    internal class Line : ITransformedGeometryImpl
    {
        public Point PStart { get; }

        public Point PEnd => Vertical ? PStart.WithY(PStart.Y + Length) : PStart.WithX(PStart.X + Length);
        public bool Vertical { get; }
        public int Length { get; }

        public static Line CreateMyLine(Point p1, Point p2)
        {
            (double x, double y) = p2 - p1;
            if (p1.X > p2.X || p1.Y > p2.Y)
            {
                p1 = p2;
                (x, y) = (-x, -y); //todo: move this to constructor
            }


            return x is 0 or -0 ? new Line(p1, true, (int)y) : new Line(p1, false, (int)x);
        }

        public Line(Point pStart, bool vertical, int length, IGeometryImpl sourceGeometry = default,
            Matrix transform = default)
        {
            PStart = pStart;
            Vertical = vertical;
            Length = length;
            SourceGeometry = sourceGeometry;
            Transform = transform;
        }

        public Rect GetRenderBounds(IPen pen)
        {
            //todo: check restrictions
            var p2 = PStart;
            if (Vertical) p2 = p2.WithY(p2.Y + Length);
            else p2 = p2.WithX(p2.X + Length);
            return new Rect(PStart, p2);
        }

        public bool FillContains(Point point)
        {
            if (Vertical)
            {
                return point.Y == PStart.Y && point.X >= PStart.X && point.X < PStart.X + Length;
            }

            return point.X == PStart.X && point.Y >= PStart.Y && point.Y < PStart.Y + Length;
        }

        public IGeometryImpl Intersect(IGeometryImpl geometry)
        {
            throw new NotImplementedException();
        }

        public bool StrokeContains(IPen pen, Point point)
        {
            throw new NotImplementedException();
        }

        public ITransformedGeometryImpl WithTransform(Matrix transform)
        {
            if (!transform.NoRotation())
                throw new NotSupportedException();

            Point pStart = PStart.Transform(transform);
            Point pEnd = PEnd.Transform(transform);
            return new Line(pStart, Vertical, (int)(pStart - pEnd).ToSKPoint().Length, this, transform);
        }

        public bool TryGetPointAtDistance(double distance, out Point point)
        {
            throw new NotImplementedException();
        }

        public bool TryGetPointAndTangentAtDistance(double distance, out Point point, out Point tangent)
        {
            throw new NotImplementedException();
        }

        public bool TryGetSegment(double startDistance, double stopDistance, bool startOnBeginFigure,
            out IGeometryImpl segmentGeometry)
        {
            throw new NotImplementedException();
        }

        public Rect Bounds => GetRenderBounds(null);

        public double ContourLength => Length * 2;
        public IGeometryImpl SourceGeometry { get; }
        public Matrix Transform { get; }
    }
}