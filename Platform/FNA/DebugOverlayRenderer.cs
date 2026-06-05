using CanopyEngine.Profiling;
using Microsoft.Xna.Framework.Graphics;
using Platform.Abstractions;
using Platform.FNA;
using System.Text;

namespace CanopyEngine.Debugging
{
    public sealed class DebugOverlayRenderer
    {
        private readonly IInput _input;
        private readonly ITime _time;
        private readonly FnaRenderer _renderer;
        private readonly SpriteFont _font;
        private bool _visible;
        private readonly Microsoft.Xna.Framework.Color _bg = new(0, 0, 0, 200);
        private readonly Microsoft.Xna.Framework.Color _textColor = Microsoft.Xna.Framework.Color.Yellow;
        private readonly int _padding = 6;

        public DebugOverlayRenderer(IInput input, ITime time, SpriteFont font, FnaRenderer renderer)
        {
            _input = input;
            _time = time;
            _font = font;
            _renderer = renderer;
        }

        public void UpdateToggle()
        {
            _visible = !_visible;
        }

        public void Draw(GraphicsDevice gd, int entityCount)
        {
            if (!_visible) return;

            var sb = new StringBuilder();
            double fps = _time.DeltaTime > 0 ? 1.0 / _time.DeltaTime : 0.0;
            sb.AppendLine($"FPS: {fps:F1}  Frame: {_time.FrameCount}");
            sb.AppendLine($"TotalTime: {_time.TotalTime:F2}s  Delta: {_time.DeltaTime * 1000.0:F2} ms  Unscaled: {_time.UnscaledDeltaTime * 1000.0:F2} ms");
            sb.AppendLine($"Entities: {entityCount}");
            sb.AppendLine("");
            sb.AppendLine("System timings (ms)  last / avg");
            foreach (var s in SystemProfiler.Stats.OrderByDescending(x => x.EmaMs).Take(40))
                sb.AppendLine($"{s.Name,-30} {s.LastMs,6:F2} / {s.EmaMs,6:F2}");

            var text = sb.ToString();
            var size = _font.MeasureString(text);
            var rectX = 8;
            var rectY = 8;
            var rectW = (int)size.X + _padding * 2;
            var rectH = (int)size.Y + _padding * 2;

            // draw background using FnaRenderer's DrawQuad (no nested SpriteBatch Begin/End)
            uint bgColor = ((uint)_bg.A << 24) | ((uint)_bg.R << 16) | ((uint)_bg.G << 8) | _bg.B;
            _renderer.DrawQuad(new System.Numerics.Vector2(rectX, rectY), rectW, rectH, bgColor);

            // draw text using FnaRenderer's DrawString (assumes FnaRenderer.Begin() already called)
            _renderer.DrawString(_font, text, new Microsoft.Xna.Framework.Vector2(rectX + _padding, rectY + _padding), _textColor);
        }
    }
}
