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
        public event OnKeyDownDelegate OnKeyDown;
        public event OnKeyDownDelegate OnKeyUp;

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

                        if (OnKeyDown != null)
                        {
                            Keys key = GetKey(m.wparam);
                            Console.WriteLine(key.ToString());
                            OnKeyDown(key);
                        }

#if WINDOWS_XNA
                        if (form.CanFocus)
                        {
                            _TranslateMessage(ref m);
                        }
#endif
                    }

                    break;

                case Wm.KeyUp:
                    {
                        long tick = DateTime.Now.Ticks;

                        if(OnKeyUp != null)
                        {
                            Keys key = GetKey(m.wparam);
                            if(OnKeyUp != null)
                            {
                                OnKeyUp(key);
                            }
                        }
                    }
                    break;

                case Wm.Char:
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
                       
                    }
                    break;

                    
            }
        }
    }
}
