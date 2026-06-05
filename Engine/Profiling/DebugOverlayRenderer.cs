using CanopyEngine.Profiling;
using DefaultEcs;
using Microsoft.Xna.Framework.Graphics;
using Platform.Abstractions;
using System.Text;

namespace Platform.Debugging
{
    public sealed class DebugOverlayRenderer
    {
        private readonly IInput _input;
        private readonly ITime _time;
        private readonly SpriteBatch _spriteBatch;
        private readonly SpriteFont _font;
        private bool _visible;
        private readonly Microsoft.Xna.Framework.Color _bg = new(0, 0, 0, 200);
        private readonly Microsoft.Xna.Framework.Color _textColor = Microsoft.Xna.Framework.Color.Yellow;
        private readonly int _padding = 6;
        private Texture2D? _white;
        private int _entityCount = 0;

        public DebugOverlayRenderer(IInput input, ITime time, GraphicsDevice gd, SpriteFont font)
        {
            _input = input;
            _time = time;
            _font = font;
            _spriteBatch = new SpriteBatch(gd);
        }

        public void UpdateToggle()
        {
            if (_input.IsKeyPressed(KeyCode.F3))
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
            var rect = new Microsoft.Xna.Framework.Rectangle(8, 8, (int)size.X + _padding * 2, (int)size.Y + _padding * 2);

            _spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                DepthStencilState.None,
                RasterizerState.CullNone
            );
            var white = GetWhite(gd);
            _spriteBatch.Draw(white, rect, _bg);
            _spriteBatch.DrawString(_font, text, new Microsoft.Xna.Framework.Vector2(rect.X + _padding, rect.Y + _padding), _textColor);
            _spriteBatch.End();
        }

        private Texture2D GetWhite(GraphicsDevice gd)
        {
            if (_white == null)
            {
                _white = new Texture2D(gd, 1, 1);
                _white.SetData(new[] { Microsoft.Xna.Framework.Color.White });
            }
            return _white;
        }
    }
}
