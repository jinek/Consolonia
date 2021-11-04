using System;
using Avalonia;
using Avalonia.Media;
using Avalonia.Platform;

namespace Consolonia.Core.Drawing
{
    internal class Rectangle : IGeometryImpl
    {
        private readonly Rect _rect;

        public Rectangle(Rect rect)
        {
            _rect = rect;
        }

        public Rect GetRenderBounds(IPen pen)
        {
            return Bounds;//todo: should be just Bounds?
        }

        public bool FillContains(Point point)
        {
            return _rect.Contains(point);
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
            throw new NotImplementedException();
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

        public Rect Bounds => _rect;
        public double ContourLength => (_rect.Width + _rect.Height) * 2;
        public Rect Rect => _rect;
    }
}