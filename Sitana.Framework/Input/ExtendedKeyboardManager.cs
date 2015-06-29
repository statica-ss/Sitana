using Microsoft.Xna.Framework.Input;
using Sitana.Framework.Cs;
using Sitana.Framework.Input.GamePad;
using Sitana.Framework.Ui.Controllers;
using Sitana.Framework.Input.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitana.Framework.Misc
{
    public class ExtendedKeyboardManager : Singleton<ExtendedKeyboardManager>
    {
        List<IBackable> _backables = new List<IBackable>();
		List<IMenuButtonListener> _menuListeners = new List<IMenuButtonListener>();

        public void Add(IBackable backable)
        {
            if(backable == null)
            {
                throw new ArgumentNullException("Backable can't be null");
            }

            var position = _backables.FindIndex(item => item == backable);
            if(position >= 0)
            {
                if(position < (_backables.Count -1))
                {
                    _backables.RemoveAt(position);
                    _backables.Add(backable);
                }
            }
            else
            {
                _backables.Add(backable);
            }
        }

		public void Add(IMenuButtonListener menuListener)
		{
			if (menuListener == null)
			{
				throw new ArgumentNullException("Menu listener can't be null");
			}

			var position = _menuListeners.FindIndex(item => item == menuListener);
			if(position >= 0)
			{
				if(position < (_menuListeners.Count -1))
				{
					_menuListeners.RemoveAt(position);
					_menuListeners.Add(menuListener);
				}
			}
			else
			{
				_menuListeners.Add(menuListener);
			}
		}

        public void Remove(IBackable obj)
        {
            _backables.Remove(obj);
        }

		public void Remove(IMenuButtonListener obj)
		{
			_menuListeners.Remove(obj);
		}

		public bool OnBackPressed()
        {
            for(int index = _backables.Count - 1; index >= 0; --index)
            {
				Type typeOfBackable = _backables[index].GetType();

                if(_backables[index].OnBack())
                {
					Console.WriteLine("Back processed by {0}.", typeOfBackable);
					return true;
                }
            }

			return false;
        }

		public bool OnMenuPressed()
		{
			for(int index = _menuListeners.Count - 1; index >= 0; --index)
			{
				if(_menuListeners[index].OnMenu())
				{
					return true;
				}
			}

			return false;
		}
    }
}
