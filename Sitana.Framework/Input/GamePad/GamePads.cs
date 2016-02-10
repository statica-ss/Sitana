using Microsoft.Xna.Framework;
using Sitana.Framework.Cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using XnaGamePad = Microsoft.Xna.Framework.Input.GamePad;

namespace Sitana.Framework.Input.GamePad
{
    public class GamePads: Singleton<GamePads>
    {
		GamePad[] _gamePads;

        public GamePads()
        {
#if ANDROID
			_gamePads = new GamePad[XnaGamePad.MaximumGamePadCount];
#elif WINDOWS_PHONE
			_gamePads = new GamePad[1];
#else
			_gamePads = new GamePad[4];
#endif

			for (int idx = 0; idx < _gamePads.Length; ++idx)
			{
				_gamePads[idx] = new GamePad((PlayerIndex)idx);
			}
        }

        public GamePad this[int index]
        {
            get
            {
                return _gamePads[index];
            }
        }

        internal void Update()
        {
            foreach(var gp in _gamePads)
            {
                gp.Update();
            }
        }
    }
}
