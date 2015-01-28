// /// This file is a part of the EBATIANOS.ESSENTIALS class library.
// /// (c)2013-2014 EBATIANO'S a.k.a. Sebastian Sejud. All rights reserved.
// ///
// /// THIS SOURCE FILE IS THE PROPERTY OF EBATIANO'S A.K.A. SEBASTIAN SEJUD 
// /// AND IS NOT TO BE RE-DISTRIBUTED BY ANY MEANS WHATSOEVER WITHOUT 
// /// THE EXPRESSED WRITTEN CONSENT OF EBATIANO'S A.K.A. SEBASTIAN SEJUD.
// ///
// /// THIS SOURCE CODE CAN ONLY BE USED UNDER THE TERMS AND CONDITIONS OUTLINED
// /// IN THE EBATIANOS.ESSENTIALS LICENSE AGREEMENT. 
// /// EBATIANO'S A.K.A. SEBASTIAN SEJUD GRANTS TO YOU (ONE SOFTWARE DEVELOPER) 
// /// THE LIMITED RIGHT TO USE THIS SOFTWARE ON A SINGLE COMPUTER.
// ///
// /// CONTACT INFORMATION:
// /// contact@ebatianos.com
// /// www.ebatianos.com/essentials-library
// /// 
// ///---------------------------------------------------------------------------
//
using System;
using Android.Widget;
using Android.Content;

namespace Sitana.Framework.Input
{
    public class EditTextEx: EditText
    {
        public delegate bool EditorActionDelegate(Android.Views.InputMethods.ImeAction actionCode);

        public new event EditorActionDelegate EditorAction;

        public EditTextEx(Context context): base(context)
        {
        }

        public override void OnEditorAction(Android.Views.InputMethods.ImeAction actionCode)
        {
            if (EditorAction != null && EditorAction(actionCode))
            { 
                return;
            }

            base.OnEditorAction(actionCode);
        }
    }
}

