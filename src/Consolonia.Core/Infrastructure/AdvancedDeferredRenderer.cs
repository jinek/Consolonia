using Avalonia;
using Avalonia.Rendering;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace Consolonia.Core.Infrastructure
{
    internal class AdvancedDeferredRenderer : DeferredRenderer, IRenderer
    {
        private readonly IVisual _root;
        public ConsoleWindow RenderRoot;

        public AdvancedDeferredRenderer(IRenderRoot root, IRenderLoop renderLoop, ISceneBuilder sceneBuilder = null,
            IDispatcher dispatcher = null, IDeferredRendererLock rendererLock = null) : base(root, renderLoop,
            sceneBuilder, dispatcher, rendererLock)
        {
            _root = root;
        }

        void IRenderer.AddDirty(IVisual visual)
        {
            base.AddDirty(visual);

            // this is from immediateRenderer, keeping original formatting
            // todo: consider how to call the original method (get rid of copypaste here)
            // ReSharper disable InvertIf
            // ReSharper disable SuggestVarOrType_SimpleTypes
            if (visual.Bounds != Rect.Empty)
            {
                var m = visual.TransformToVisual(_root);

                if (m.HasValue)
                {
                    var bounds = new Rect(visual.Bounds.Size).TransformToAABB(m.Value);

                    //use transformedbounds as previous render state of the visual bounds
                    //so we can invalidate old and new bounds of a control in case it moved/shrinked
                    if (visual.TransformedBounds.HasValue)
                    {
                        var trb = visual.TransformedBounds.Value;
                        var trBounds = trb.Bounds.TransformToAABB(trb.Transform);

                        if (trBounds != bounds) RenderRoot?.Invalidate(trBounds);
                    }

                    RenderRoot?.Invalidate(bounds);
                }
            }
            // ReSharper restore SuggestVarOrType_SimpleTypes
            // ReSharper restore InvertIf
        }
    }
}