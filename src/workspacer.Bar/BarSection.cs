using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace workspacer.Bar
{
    public class BarSection
    {
        public int WidthInPixels;
        private FlowLayoutPanel _panel;
        private IBarWidget[] _widgets;
        private IMonitor _monitor;
        private IConfigContext _configContext;
        private string _fontName;
        private int _fontSize;
        
        private Color _defaultFore;
        private Color _defaultBack;

        private bool _reverse;
        private IBarWidgetContext _context;

        private IDictionary<Label, Action> _clickedHandlers;

        public BarSection(bool reverse, FlowLayoutPanel panel, IBarWidget[] widgets, IMonitor monitor, IConfigContext context,
            Color defaultFore, Color defaultBack, string fontName, int fontSize)
        {
            _panel = panel;
            _widgets = widgets;
            _monitor = monitor;
            _configContext = context;
            _fontName = fontName;
            _fontSize = fontSize;
            _reverse = reverse;
            _defaultFore = defaultFore;
            _defaultBack = defaultBack;

            _clickedHandlers = new Dictionary<Label, Action>();

            _context = new BarWidgetContext(this, _monitor, _configContext);
            while (_panel.Controls.Count != _widgets.Count())
            {
                _panel.Controls.Add(CreateWidgetPanel());
            }

            InitializeWidgets(widgets, _context);
            GetWidthInPixels(widgets);
        }

        private void GetWidthInPixels(IEnumerable<IBarWidget> widgets)
        {
            foreach (var w in widgets)
            {
                var parts = w.GetParts();
                foreach (var part in parts)
                {
                    WidthInPixels += TextRenderer.MeasureText(part.Text, CreateFont(_fontName, _fontSize)).Width;
                }
            }
        }

        private FlowLayoutPanel CreateWidgetPanel()
        {
            return new FlowLayoutPanel
            {
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom,
                BackColor = ColorToColor(_defaultBack),
                Location = new Point(0, 0),
                Margin = new Padding(0),
                Size = new Size(50, 50),
                WrapContents = false
            };
        }

        public void Draw()
        {
            if (!_widgets.Any(w => w.IsDirty()))
            {
                return;
            }

            var widgets = _reverse ? _widgets.Reverse().ToArray() : _widgets;
            for (int wIndex = 0; wIndex < widgets.Length; wIndex++)
            {
                if (!widgets[wIndex].IsDirty())
                {
                    continue;
                }

                var widgetPanel = _panel.Controls[wIndex];
                var parts = widgets[wIndex].GetParts();

                EqualizeControls((FlowLayoutPanel)widgetPanel, parts.Count());

                for (var pIndex = 0; pIndex < parts.Length; pIndex++)
                {
                    SetLabel((Label)widgetPanel.Controls[pIndex], parts[pIndex]);
                }

                widgets[wIndex].MarkClean();
            }
        }

        private void EqualizeControls(FlowLayoutPanel panel, int partCount)
        {
            if (panel.Controls.Count != partCount)
            {
                while (panel.Controls.Count < partCount)
                {
                    AddLabel(panel);
                }

                while (panel.Controls.Count > partCount)
                {
                    panel.Controls.RemoveAt(panel.Controls.Count - 1);
                }
            }
        }

        private void SetLabel(Label label, IBarWidgetPart part)
        {
            label.Text = part.Text;

            var foregroundColor = ColorToColor(part.ForegroundColor ?? _defaultFore);
            if (label.ForeColor != foregroundColor)
            {
                label.ForeColor = foregroundColor;
            }

            var backgroundColor = ColorToColor(part.BackgroundColor ?? _defaultBack);
            if (label.BackColor != backgroundColor)
            {
                label.BackColor = backgroundColor;
            }

            label.Font = CreateFont(string.IsNullOrEmpty(part.FontName) ? _fontName : part.FontName, _fontSize);

            if (part.PartClicked != null)
            {
                _clickedHandlers[label] = part.PartClicked;
            }
            else
            {
                _clickedHandlers.Remove(label);
            }
        }

        private System.Drawing.Color ColorToColor(Color color)
        {
            return System.Drawing.Color.FromArgb(color.R, color.G, color.B);
        }

        private Font CreateFont(string name, float size)
        {
            return new Font(name, size, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
        }

        private Label AddLabel(FlowLayoutPanel panel)
        {
            var label = new Label
            {
                Font = CreateFont(_fontName, _fontSize),
                Padding = new Padding(0),
                Margin = new Padding(0),
                AutoSize = true
            };

            panel.Controls.Add(label);
            label.Click += (s, e) =>
            {
                if (_clickedHandlers.ContainsKey(label))
                {
                    _clickedHandlers[label]();
                }
            };

            return label;
        }

        private void InitializeWidgets(IEnumerable<IBarWidget> widgets, IBarWidgetContext context)
        {
            foreach (var w in widgets)
            {
                w.Initialize(context);
            }
        }
    }
}
