using System;
using Avalonia;
using Avalonia.Media;
using Avalonia.Platform;
using Consolonia.Core.InternalHelpers;

namespace Consolonia.Core.Drawing
{
    internal class Line : ITransformedGeometryImpl
    {
        public Line(Point pStart, bool vertical, int length, IGeometryImpl sourceGeometry = default,
            Matrix transform = default)
        {
            PStart = pStart;
            Vertical = vertical;
            Length = length;
            SourceGeometry = sourceGeometry!; // todo: check why sometimes is goes nullable?
            Transform = transform;
        }

        public Point PStart { get; }

        private Point PEnd => Vertical ? PStart.WithY(PStart.Y + Length) : PStart.WithX(PStart.X + Length);
        public bool Vertical { get; }
        public int Length { get; }

        public Rect GetRenderBounds(IPen pen)
        {
            //todo: check restrictions
            Point p2 = PStart;
            p2 = Vertical ? p2.WithY(p2.Y + Length) : p2.WithX(p2.X + Length);
            return new Rect(PStart, p2);
        }

        public bool FillContains(Point point)
        {
            if (Vertical)
                return point.Y.IsNearlyEqual(PStart.Y)
                       && point.X >= PStart.X
                       && point.X < PStart.X + Length;

            return point.X.IsNearlyEqual(PStart.X)
                   && point.Y >= PStart.Y
                   && point.Y < PStart.Y + Length;
        }

        public IGeometryImpl Intersect(IGeometryImpl geometry)
        {
            throw new NotImplementedException();
        }

        public bool StrokeContains(IPen pen, Point point)
        {
            return false; //todo: why is this called when hit test is false?
        }

        public ITransformedGeometryImpl WithTransform(Matrix transform)
        {
            if (!transform.NoRotation())
                throw new NotSupportedException();

            Point pStart = PStart.Transform(transform);
            Point pEnd = PEnd.Transform(transform);

            (double vectorX, double vectorY) = pStart - pEnd;
            return new Line(pStart,
                Vertical,
                (int)Math.Abs(vectorX + vectorY) /*always vertical or horizontal*/,
                this,
                transform);
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

        public static Line CreateMyLine(Point p1, Point p2)
        {
            (double x, double y) = p2 - p1;
            // ReSharper disable once InvertIf
            if (p1.X > p2.X || p1.Y > p2.Y)
            {
                p1 = p2;
                (x, y) = (-x, -y); //todo: move this to constructor
            }


            return x is 0 or -0 ? new Line(p1, true, (int)y) : new Line(p1, false, (int)x);
        }
    }
}