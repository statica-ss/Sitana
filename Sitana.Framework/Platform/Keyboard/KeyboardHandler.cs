using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Xna.Framework.Input
{
    public class KeyboardHandler : MessageHook
    {
        public delegate void OnCharacterDelegate(char c);
        public delegate void OnKeyDownDelegate(Keys key);

        public event OnCharacterDelegate OnCharacter;
        public event OnKeyDownDelegate OnKey;

        long _wmCharLastTick = 0;
        long _wmKeyDownLastTick = 0;

        public KeyboardHandler(IntPtr window)
            : base(window)
        {

        }

        Keys GetKey(IntPtr wparam)
        {
            return (Keys)(int)wparam;
        }

        protected override void Hook(ref Message m)
        {
            switch (m.msg)
            {
                case Wm.KeyDown:
                    {
                        long tick = DateTime.Now.Ticks;

                        if (tick > _wmKeyDownLastTick + 100000)
                        {

                            if (OnKey != null)
                            {
                                Keys key = GetKey(m.wparam);
                                OnKey(key);
                            }

                            if (form.CanFocus)
                            {
                                _TranslateMessage(ref m);
                            }
                        }
                        _wmKeyDownLastTick = tick;
                    }

                    break;

                case Wm.Char:
                    {
                        long tick = DateTime.Now.Ticks;

                        if (tick > _wmCharLastTick + 100000)
                        {
                            char c = (char)m.wparam;

                            if (c < (char)0x20
                                && c != '\n'
                                && c != '\r'
                                //&& c != '\t'//tab //uncomment to accept tab
                                && c != '\b')//backspace
                                break;

                            if (OnCharacter != null)
                            {
                                OnCharacter(c);
                            }
                            _wmCharLastTick = tick;
                        }
                    }
                    break;

                    
            }
        }
    }
}
