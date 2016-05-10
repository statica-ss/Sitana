// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Text;

namespace Sitana.Framework.Input
{
	[Flags]
    public enum TextInputType: int
    {
        NormalText = 1,
		MultilineText = 2,
		FirstLetterUppercase = 4,
		Uppercase = 8,
		PasswordClass = 16,
        Digits = 32,
		Numeric = 64,
        Email = 128,

		TypeFilter = 1023,

		NoSuggestions = 1024,
        Wrap = 2048,

		Password = NoSuggestions | PasswordClass
    }
}
