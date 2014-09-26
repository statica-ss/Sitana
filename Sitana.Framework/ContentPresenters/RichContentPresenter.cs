using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Globalization;

namespace Sitana.Framework.Content
{
    public class RichContentPresenter
    {
        public abstract class DisplayElement
        {
            public abstract Point Size {get;}
            public Point Position { get; internal set; }

            public virtual Boolean UsesInterline
            {
                get
                {
                    return false;
                }
            }

            public virtual void Init(IFontPresenter fontPresenter, String[] arguments)
            { }

            public virtual void Draw(SpriteBatch spriteBatch, Vector2 offset, Single opacity, Single scale)
            {

            }
        }

        public class DisplayElementText : DisplayElement
        {
            IFontPresenter _textPresenter;

            public override Point Size
            {
                get
                {
                    return _textPresenter.Size.ToPoint();
                }
            }

            public override Boolean UsesInterline
            {
                get
                {
                    return true;
                }
            }

            public override void Init(IFontPresenter fontPresenter, String[] arguments)
            {
                _textPresenter = fontPresenter.Clone();
                _textPresenter.PrepareRender(arguments[1], false);
            }

            public override void Draw(SpriteBatch spriteBatch, Vector2 offset, Single opacity, Single scale)
            {
                offset += (Position.ToVector2() * scale).TrimToIntValues();
                _textPresenter.DrawText(spriteBatch, offset, Color.White * opacity, scale, Align.Left);
            }
        }

        public class DisplayElementImage : DisplayElement
        {
            Texture2D _texture;
            Rectangle _source;

            public override Point Size
            {
                get
                {
                    return new Point(_source.Width, _source.Height);
                }
            }

            public override void Init(IFontPresenter fontPresenter, String[] arguments)
            {
                String path = arguments[1];
                _texture = ContentLoader.Current.Load<Texture2D>(path);
                
                _source = new Rectangle(0, 0, _texture.Width, _texture.Height);

                if (arguments.Length > 2)
                {
                    _source = new Rectangle(
                        Int32.Parse(arguments[2]),
                        Int32.Parse(arguments[3]),
                        Int32.Parse(arguments[4]),
                        Int32.Parse(arguments[5]));
                }
            }

            public override void Draw(SpriteBatch spriteBatch, Vector2 offset, Single opacity, Single scale)
            {
                offset += (Position.ToVector2() * scale).TrimToIntValues();
                spriteBatch.Draw(_texture, offset, _source, Color.White * opacity, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
            }
        }

        String _template;
        String _current;

        IFontPresenter _currentFontPresenter;

        Int32 _skipLevel = 0;

        Align _currentAlign;
        Align _currentValign;

        Point _currentPosition;
        Int32 _lineHeight;

        Int32 _width;

        Single _interline;
        Int32 _padding;

        public Int32 Height
        {
            get;
            set;
        }

        List<List<DisplayElement>> _lines = new List<List<DisplayElement>>();

        Dictionary<String, Type> _displayElementsTypes = new Dictionary<String,Type>();

        Dictionary<String, String> _defines = new Dictionary<String, String>();

        public static RichContentPresenter New(String path)
        {
            path += ".txt";

            using (Stream stream = ContentLoader.Current.Open(path) )
            {
                using(StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    String template = reader.ReadToEnd();
                    return new RichContentPresenter(template);
                }
            }
        }

        public RichContentPresenter(String template)
        {
            template = template.Replace("\t", String.Empty);

            String[] lines = template.Split('\r', '\n');

            _template = String.Empty;

            foreach (var line in lines)
            {
                if (!String.IsNullOrWhiteSpace(line) && !line.StartsWith("#"))
                {
                    _template += line;
                }
            }

            RegisterType("text", typeof(DisplayElementText));
            RegisterType("image", typeof(DisplayElementImage));
        }

        public void RegisterType(String name, Type type)
        {
            _displayElementsTypes.Add(name, type);
        }

        public void Begin(Int32 width)
        {
            _width = width;
            _current = _template;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 offset, Single opacity, Single scale)
        {
            for ( Int32 idx = 0; idx < _lines.Count; ++idx )
            {
                var line = _lines[idx];

                for ( Int32 idx2 = 0; idx2 < line.Count; ++idx2)
                {
                    var element = line[idx2];

                    element.Draw(spriteBatch, offset, opacity, scale);
                }
            }
        }

        public void SetValue(String id, String value)
        {
            if (_current == null)
            {
                throw new InvalidOperationException("Begin must be called before SetValue.");
            }

            value = value.Replace("\n", "{endl}");

            _current = _current.Replace("[" + id + "]", value);
        }

        public void End()
        {
            _lineHeight = 0;
            _currentPosition = Point.Zero;
            _currentFontPresenter = null;
            _currentAlign = Align.Left;
            _currentValign = Align.Top;
            _defines.Clear();
            _interline = 1.5f;
            _padding = 0;

            _lines.Clear();
            _lines.Add(new List<DisplayElement>());

            Int32 begin = 0;

            for (Int32 idx = 0; idx < _current.Length; ++idx)
            {
                Char character = _current[idx];

                if (character == '{')
                {
                    Parse(begin, idx);
                    begin = idx;
                }

                if (character == '}')
                {
                    Parse(begin, idx);
                    begin = idx + 1;
                }
            }

            MoveToNextLine();
            Height = _currentPosition.Y;

            _current = null;
        }

        private void Parse(Int32 begin, Int32 end)
        {
            if (_current[begin] == '{')
            {
                if (end == begin)
                {
                    return;
                }

                String[] value = _current.Substring(begin+1, end - begin - 1).Split(':');

                for (Int32 idx = 1; idx < value.Length; ++idx)
                {
                    var val = value[idx];

                    if (val.Contains('['))
                    {
                        foreach (var defs in _defines)
                        {
                            val = val.Replace(defs.Key, defs.Value);
                        }
                    }

                    value[idx] = val;
                }

                AddCommand(value);
            }
            else
            {
                String value = _current.Substring(begin, end - begin);

                if (value.Contains('['))
                {
                    foreach (var defs in _defines)
                    {
                        value = value.Replace(defs.Key, defs.Value);
                    }
                }

                String[] values = new String[2];
                values[0] = "text";
                values[1] = value;

                AddCommand(values);
            }
        }

        private void AddCommand(String[] arguments)
        {
            if (_skipLevel > 0)
            {
                if (arguments[0] == "if")
                {
                    _skipLevel++;
                }

                if (arguments[0] == "endif")
                {
                    _skipLevel--;
                }

                return;
            }

            switch (arguments[0])
            {
                case "define":
                    _defines.Add("[" + arguments[1] + "]", arguments[2]);
                    break;

                case "font":
                    _currentFontPresenter = FontLoader.Load(arguments[1]);
                    break;

                case "if":
                    if (String.IsNullOrWhiteSpace(arguments[1]))
                    {
                        _skipLevel = 1;
                    }
                    break;

                case "align":
                    if (!Enum.TryParse<Align>(arguments[1], true, out _currentAlign))
                    {
                        _currentAlign = Align.Left;
                    }
                    break;

                case "valign":
                    if (!Enum.TryParse<Align>(arguments[1], true, out _currentValign))
                    {
                        _currentValign = Align.Top;
                    }
                    break;

                case "endl":
                    MoveToNextLine();
                    break;

                case "endif":
                    break;

                case "interline":
                    _interline = Single.Parse(arguments[1], CultureInfo.InvariantCulture);
                    break;

                case "padding":
                    _padding = Int32.Parse(arguments[1]);
                    break;

                case "empty":
                    _lineHeight = Math.Max(_lineHeight, Int32.Parse(arguments[1]));
                    break;

                default:
                    AddElement(arguments);
                    break;
            }
        }

        private void MoveToNextLine()
        {
            Int32 move = 0;
            switch (_currentAlign)
            {
                case Align.Right:
                    move = _width - _currentPosition.X;
                    break;

                case Align.Center:
                    move = (_width - _currentPosition.X) / 2;
                    break;
            }

            var list = _lines.Last();

            foreach (var element in list)
            {
                Int32 moveY = 0;

                if (_currentValign == Align.Bottom)
                {
                    moveY = _lineHeight - element.Size.Y;
                }
                else if (_currentValign == Align.Middle)
                {
                    moveY = (_lineHeight - element.Size.Y)/2;
                }

                element.Position = new Point(element.Position.X + move, element.Position.Y + moveY);
            }

            _lines.Add(new List<DisplayElement>());

            _currentPosition.Y += (Int32)(_lineHeight);
            _currentPosition.X = 0;
            _lineHeight = 0;
        }

        private void AddElement(String[] arguments)
        {
            DisplayElement element = CreateElement(arguments);

            if (_currentPosition.X + element.Size.X > _width)
            {
                MoveToNextLine();
            }

            if (_currentPosition.X > 0)
            {
                _currentPosition.X += _padding;
            }

            element.Position = _currentPosition;

            Single interline = element.UsesInterline ? _interline : 1;

            _currentPosition.X += element.Size.X;
            _lineHeight = Math.Max(_lineHeight, (Int32)((Single)element.Size.Y * interline));

            _lines.Last().Add(element);
        }

        protected virtual DisplayElement CreateElement(String[] arguments)
        {
            Type type;
            if (_displayElementsTypes.TryGetValue(arguments[0], out type))
            {
                DisplayElement element = Activator.CreateInstance(type) as DisplayElement;

                if (element == null)
                {
                    throw new Exception("The DisplayElement could not be created.");
                }

                element.Init(_currentFontPresenter, arguments);

                return element;
            }
            
            throw new Exception("Unknown DiplayElement type.");
        }
    }
}
