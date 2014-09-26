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
using Microsoft.Xna.Framework;
using Ebatianos;
using Ebatianos.Content;
using Ebatianos.Gui;
using System.Collections.Generic;

namespace Ebatianos.Content.Generators
{
    public abstract class ContentGenerator : GuiElement
    {
        protected ParametersCollection _parameters;
        protected Screen _owner;

        /// <summary>
        /// Initialize resource loader from parameters.
        /// </summary>
        /// <param name="parameters">Parameters from xml node.</param>
        /// <param name="contentLoader">Content loader.</param>
        /// <param name="owner">Owner screen.</param>
        /// <returns>True when succeeded.</returns>
        protected override Boolean Initialize(InitializeParams initParams)
        {
            ParametersCollection parameters = initParams.Parameters;
            Screen owner = initParams.Owner;

            _parameters = parameters;
            _owner = owner;

            if (_parameters.AsBoolean("InstantGeneration"))
            {
                Generate();
            }
            else if (owner is ScreenPreloader)
            {
                (owner as ScreenPreloader).AddGenerator(this, parameters.AsString("Path"));
            }

            return false;
        }

        public abstract void Generate();

        protected void OnGenerated(Type type, Object contentObj)
        {
            if (contentObj != null)
            {
                ContentLoader.Current.AddContent(_parameters.AsString("Path"), type, contentObj);
            }
        }
    }
}

