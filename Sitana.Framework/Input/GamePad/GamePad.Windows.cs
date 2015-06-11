using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Sitana.Framework.Cs;
using System;
using System.Collections.Generic;
using System.Text;

using XnaGamePad = Microsoft.Xna.Framework.Input.GamePad;

namespace Sitana.Framework.Input.GamePad
{
    public class GamePad
    {
        PlayerIndex _playerIndex;

        Dictionary<Buttons, GamePadButtonState> _states = new Dictionary<Buttons, GamePadButtonState>();
        Buttons[] _keys;

        internal GamePad(PlayerIndex playerIndex)
        {
            _playerIndex = playerIndex;

            Array keys = Enum.GetValues(typeof(Buttons));

            _keys = new Buttons[keys.Length];

            for(int idx = 0; idx < keys.Length; ++idx)
            {
                _states.Add((Buttons)(keys.GetValue(idx)), GamePadButtonState.Released);
                _keys[idx] = (Buttons)(keys.GetValue(idx));
            }
        }

        public bool IsConnected
        {
            get
            {
                return XnaGamePad.GetCapabilities(_playerIndex).IsConnected;
            }
        }

        internal void Update()
        {
        }

        internal void ProcessKey(Keys key)
        {
            if(key != Keys.Escape)
            {
                return;
            }

            KeyboardState state = Keyboard.GetState();

            if(state.IsKeyDown(Keys.Escape))
            {
                _states[Buttons.Back] = GamePadButtonState.Pressed;
            }
            else if(state.IsKeyUp(Keys.Escape))
            {
                _states[Buttons.Back] = GamePadButtonState.Released;
            }
        }

        public bool IsButtonDown(Buttons button)
        {
            return _states[button] != GamePadButtonState.Released;
        }

        public bool IsButtonUp(Buttons button)
        {
            return _states[button] == GamePadButtonState.Released;
        }

        public GamePadButtonState ButtonState(Buttons button)
        {
            return _states[button];
        }
    }
}
