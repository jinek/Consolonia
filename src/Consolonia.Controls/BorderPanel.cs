using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.LogicalTree;
using Consolonia.Controls.Brushes;

namespace Consolonia.Controls;

/// <summary>
/// BorderPanel is a control that combines a Panel and a Border.
/// </summary>
/// <remarks>
/// * It removes a lot of copy-pasta border templates making it easier for a theme to control the border appearance for popups.
/// * It is smart about LineBrushes with Edge style, applying background to non-edge borders and no background to edge borders.
/// * It automatically crops the margin of the panel when hosted in a popup to avoid gaps between the button and the popup.
/// </remarks>
[TemplatePart(PART_Panel, typeof(Panel))]
[TemplatePart(PART_Border, typeof(Border))]
public class BorderPanel : ContentControl
{
    private const string PART_Panel = "PART_Panel";
    private const string PART_Border = "PART_Border";
    private Panel _panel;
    private Border _border;
    private Popup? _popup;

    public BorderPanel()
    {
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        this._panel = e.NameScope.Find<Panel>(PART_Panel);
        this._border = e.NameScope.Find<Border>(PART_Border);
        
        if (_popup != null)
        {
            _popup.PropertyChanged -= Popup_PropertyChanged;
            _popup = null;
        }
        _popup = this.FindLogicalAncestorOfType<Popup>();
        if (_popup != null)
        {
            _popup.PropertyChanged += Popup_PropertyChanged;
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);

        if (_popup != null)
        {
            _popup.PropertyChanged -= Popup_PropertyChanged;
            _popup = null;
        }
    }

    private void Popup_PropertyChanged(object sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == Popup.IsOpenProperty)
        {
            if (HasEdgeStyle())
            {
                var popup = sender as Popup;
                if (popup != null && (bool)e.NewValue)
                {
                    if (_panel == null)
                        return;

                    // Automatically crop the margin to get rid of gap between button and popup
                    switch (popup.Placement)
                    {
                        case PlacementMode.Top:
                        case PlacementMode.TopEdgeAlignedLeft:
                        case PlacementMode.TopEdgeAlignedRight:
                            _panel.Margin = new Thickness(0, 0, 0, -1);
                            break;
                        case PlacementMode.Bottom:
                        case PlacementMode.BottomEdgeAlignedRight:
                        case PlacementMode.BottomEdgeAlignedLeft:
                            _panel.Margin = new Thickness(0, -1, 0, 0);
                            break;
                        case PlacementMode.Left:
                        case PlacementMode.LeftEdgeAlignedTop:
                        case PlacementMode.LeftEdgeAlignedBottom:
                            _panel.Margin = new Thickness(0, 0, -1, 0);
                            break;
                        case PlacementMode.Right:
                        case PlacementMode.RightEdgeAlignedTop:
                        case PlacementMode.RightEdgeAlignedBottom:
                            _panel.Margin = new Thickness(-1, 0, 0, 0);
                            break;
                        default:
                            // Do nothing for other placements
                            break;
                    }
                }
            }
        }
    }


    private bool HasEdgeStyle()
    {
        var lineBrush = _border?.BorderBrush as LineBrush;
        return (lineBrush != null &&
            (lineBrush.LineStyle.Left == LineStyle.Edge ||
             lineBrush.LineStyle.Top == LineStyle.Edge ||
             lineBrush.LineStyle.Right == LineStyle.Edge ||
             lineBrush.LineStyle.Bottom == LineStyle.Edge ||
             lineBrush.LineStyle.Left == LineStyle.EdgeWide ||
             lineBrush.LineStyle.Top == LineStyle.EdgeWide ||
             lineBrush.LineStyle.Right == LineStyle.EdgeWide ||
             lineBrush.LineStyle.Bottom == LineStyle.EdgeWide));
    }
}