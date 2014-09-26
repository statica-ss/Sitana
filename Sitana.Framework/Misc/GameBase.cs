/// This file is a part of the EBATIANOS.ESSENTIALS class library.
/// (c)2013-2014 EBATIANO'S a.k.a. Sebastian Sejud. All rights reserved.
///
/// THIS SOURCE FILE IS THE PROPERTY OF EBATIANO'S A.K.A. SEBASTIAN SEJUD 
/// AND IS NOT TO BE RE-DISTRIBUTED BY ANY MEANS WHATSOEVER WITHOUT 
/// THE EXPRESSED WRITTEN CONSENT OF EBATIANO'S A.K.A. SEBASTIAN SEJUD.
///
/// THIS SOURCE CODE CAN ONLY BE USED UNDER THE TERMS AND CONDITIONS OUTLINED
/// IN THE EBATIANOS.ESSENTIALS LICENSE AGREEMENT. 
/// EBATIANO'S A.K.A. SEBASTIAN SEJUD GRANTS TO YOU (ONE SOFTWARE DEVELOPER) 
/// THE LIMITED RIGHT TO USE THIS SOFTWARE ON A SINGLE COMPUTER.
///
/// CONTACT INFORMATION:
/// contact@ebatianos.com
/// www.ebatianos.com/essentials-library
/// 
///---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;
using Sitana.Framework.Input;
using Sitana.Framework.Content;
using Sitana.Framework;


namespace Sitana.Framework.Gui
{
    public class GameBase : Game
    {
        protected class SendErrors
        {
            public String Name;
            public String Address;
            public String Subject;
            public String MessageFormat;
        }

        protected SendErrors SendErrorsSettings { get; set; }

        protected GraphicsDeviceManager Graphics { get; private set; }
        protected ScreenManager ScreenManager { get; set; }

        private Boolean _skipManagerUpdate = false;
        protected Boolean _appDeactivated = false;

        private Boolean _initialized = false;

        protected Boolean SupressDrawIfNoChange = false;

        public GameBase(String[] contentPaths, Type screenManagerType, Boolean initializeMusicControler)
        {
            InactiveSleepTime = TimeSpan.FromMilliseconds(100);

            // Create new input handler.

            #if WINDOWS || MACOSX
                InputHandler.Current = new MouseInputHandler();
            #else
                InputHandler.Current = new TouchInputHandler();
            #endif

            ScreenManager = (ScreenManager)Activator.CreateInstance(screenManagerType, new Object[] { Services, contentPaths });

            if (initializeMusicControler)
            {
                #if IOS
                    MusicControler.Initialize(typeof(MusicControlerIos));
                #else
                    MusicControler.Initialize(typeof(MusicControler));
                #endif
            }

            Graphics = new GraphicsDeviceManager(this);
            Graphics.IsFullScreen = true;
            Graphics.SynchronizeWithVerticalRetrace = true;
        }

        public GameBase(String[] contentPaths, Type screenManagerType, Type inputHandlerType, Type musicControlerType)
        {
            InactiveSleepTime = TimeSpan.FromMilliseconds(100);

            // Create new input handler.
            InputHandler.Current = (InputHandler)Activator.CreateInstance(inputHandlerType, null);

            ScreenManager = (ScreenManager)Activator.CreateInstance(screenManagerType, new Object[] { Services, contentPaths });

            if (musicControlerType != null)
            {
                // Initialize music controler.
                MusicControler.Initialize(musicControlerType);
            }

            Graphics = new GraphicsDeviceManager(this);
            Graphics.IsFullScreen = true;
            Graphics.SynchronizeWithVerticalRetrace = true;
        }

		public void Release()
		{
			ScreenManager.Release();

			Dispose();
		}

        // Draw game screens.
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

#if !DEBUG
            try
            {
#endif
                if (!_skipManagerUpdate)
                {
                    ScreenManager.Draw(gameTime.ElapsedGameTime);
                }
#if !DEBUG
            }
            catch (System.Exception ex)
            {
                LogError(ex);
            }
#endif
        }

        // Process game screens.
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

#if !DEBUG
            try
            {
#endif
                if (!_skipManagerUpdate)
                {
                    if (!ScreenManager.Update(gameTime.ElapsedGameTime))
                    {
                        DoExit();
                    }
                }

                if (SupressDrawIfNoChange && ScreenManager.SupressRedraw)
                {
                    SuppressDraw();
                }
#if !DEBUG
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
#endif
        }

        protected virtual void DoExit()
        {
            #if !IOS
            Exit();
            #endif
        }

        // Load game content and initiaize screenManager.
        protected override void LoadContent()
        {
            base.LoadContent();

#if !DEBUG
            try
            {
#endif
                ScreenManager.Initialize(GraphicsDevice);
                ScreenManager.OnAppActivated();

                _initialized = true;

#if !DEBUG
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
#endif
        }

        // When games is activated.
        protected override void OnActivated(object sender, EventArgs args)
        {
            base.OnActivated(sender, args);

            if (_appDeactivated == true && _skipManagerUpdate)
            {
                #if !IOS
                Exit();
                return;
                #endif
            }

            _appDeactivated = false;

            if (!_skipManagerUpdate && _initialized && ScreenManager != null)
            {
#if !DEBUG
                try
                {
#endif
                    ScreenManager.OnAppActivated();

#if !DEBUG
                }
                catch (Exception ex)
                {
                    LogError(ex);
                }
#endif
            }
        }

        protected override void OnDeactivated(object sender, EventArgs args)
        {
            base.OnDeactivated(sender, args);

            _appDeactivated = true;

            if (!_skipManagerUpdate && _initialized && ScreenManager != null)
            {
#if !DEBUG
                try
                {
#endif
                    ScreenManager.OnAppDeactivated();

#if !WINDOWS_PHONE
               Draw(new GameTime(new TimeSpan(0), new TimeSpan(0)));
#endif
#if !DEBUG
                }
                catch (Exception ex)
                {
                    LogError(ex);
                }
#endif
            }
        }

        protected virtual void LogError(Exception ex)
        {
            if (SendErrorsSettings != null)
            {
                SystemWrapper.OpenMail(SendErrorsSettings.Name, SendErrorsSettings.Address, SendErrorsSettings.Subject,
                   String.Format(SendErrorsSettings.MessageFormat, SystemWrapper.CurrentVersion, ex.ToString()),
                   new Action(() =>
                           {
                               throw ex;
                           }

                           )
                   );

                _skipManagerUpdate = true;
                return;
            }

            _skipManagerUpdate = true;
            throw ex;
        }
    }
}
