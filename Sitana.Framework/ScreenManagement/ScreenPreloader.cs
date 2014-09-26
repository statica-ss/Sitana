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
using System.Reflection;
using System.Threading;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Sitana.Framework.Content;
using Sitana.Framework.Content.Generators;


namespace Sitana.Framework.Gui
{
    public class ScreenPreloader : Screen
    {
        private static Dictionary<Type, MethodInfo> _loaderMethod = new Dictionary<Type, MethodInfo>();

        private List<LoadResourceInfo> _elementsToLoad = new List<LoadResourceInfo>();

        public Single Progress { get; private set; }

        protected Boolean Completed { get; private set; }

        private Action _completedAction = null;

        private Double _wait = 1;
        private Int32 _initialCount = 0;

        public ScreenPreloader()
        {
            Progress = 0;
            Completed = false;
        }

        public override void LoadContent(Object[] arguments)
        {
            _completedAction = Parameters().AsAction("OnCompleted", null);
            _wait = (Double)Parameters().AsInt32("MinimalLoadTime") / 1000.0;
        }

        public void AddGenerator(ContentGenerator generator, String path)
        {
            LoadResourceInfo info = new LoadResourceInfo() { Generator = generator, Path = path };

            _elementsToLoad.Add(info);

            _initialCount = _elementsToLoad.Count;
        }

        public static LoadResourceInfo GetInfoForElement(Type type, String path)
        {
            MethodInfo genericMethodInstance = null;

            if (!_loaderMethod.TryGetValue(type, out genericMethodInstance))
            {
                MethodInfo methodInfo = typeof(ContentLoader).GetMethod("Load");

                if (methodInfo == null)
                {
                    return null;
                }

                genericMethodInstance = methodInfo.MakeGenericMethod(type);

                if (genericMethodInstance == null)
                {
                    return null;
                }

                _loaderMethod.Add(type, genericMethodInstance);
            }

            LoadResourceInfo info = new LoadResourceInfo() { Load = genericMethodInstance, Path = path, ContentType = type };
            return info;
        }

        public void AddElement(Type type, String path)
        {
            var info = GetInfoForElement(type, path);

            _elementsToLoad.Add(info);
            _initialCount = _elementsToLoad.Count;
        }

        public override void Update(TimeSpan gameTime)
        {
            base.Update(gameTime);

            if (State == ScreenState.Visible)
            {
                if (_elementsToLoad.Count > 0)
                {
                    LoadResourceInfo info = _elementsToLoad[0];
                    _elementsToLoad.RemoveAt(0);

                    try
                    {
                        if (info.Generator != null)
                        {
                            info.Generator.Generate();
                        }
                        else if (info.Load != null)
                        {
                            Object contentObj = info.Load.Invoke(ContentLoader.Current, new Object[] { info.Path });
                            ContentLoader.Current.AddContent(info.Path, info.ContentType, contentObj);
                        }
                    }
                    catch (Exception) { }

                    Progress = (Single)(_initialCount - _elementsToLoad.Count) / (Single)_initialCount;
                    Completed = _elementsToLoad.Count == 0;

                    _wait = Math.Max(_wait, 0.5);
                }

                if (_wait > 0)
                {
                    _wait -= gameTime.TotalSeconds;
                }

                if (Completed && _wait <= 0)
                {
                    if (_completedAction != null)
                    {
                        _completedAction.Invoke();
                        _completedAction = null;
                    }
                }
            }
        }
    }
}
