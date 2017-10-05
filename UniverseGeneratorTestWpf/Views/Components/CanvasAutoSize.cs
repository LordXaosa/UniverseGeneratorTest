using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace UniverseGeneratorTestWpf.Views.Components
{
    public class CanvasAutoSize : Canvas
    {
        protected override System.Windows.Size MeasureOverride(System.Windows.Size constraint)
        {
            base.MeasureOverride(constraint);
            IEnumerable<UIElement> children = InternalChildren.OfType<UIElement>();
            if (children != null && children.Count() > 0)
            {
                double width = children.Max(i => {
                    double d = (double)i.GetValue(Canvas.LeftProperty);
                    if (double.IsNaN(d))
                        d = 0;
                    return i.DesiredSize.Width + d;
                    });
                double height = children.Max(i => {
                    double d = (double)i.GetValue(Canvas.TopProperty);
                    if (double.IsNaN(d))
                        d = 0;
                    return i.DesiredSize.Height + d;
                });
                if (width < 0 || double.IsNaN(width) || double.IsInfinity(width))
                    width = 0;
                if (height < 0 || double.IsNaN(height) || double.IsInfinity(height))
                    height = 0;
                return new Size(width, height);
            }
            return new Size(0, 0);
        }
    }
}
