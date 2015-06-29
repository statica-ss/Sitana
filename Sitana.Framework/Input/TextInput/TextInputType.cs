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
		FirstLetterUppercase = 3,
		Uppercase = 4,
		PasswordClass = 5,
        Digits = 6,
		Numeric = 7,
        Email = 8,

		TypeFilter = 0xff,

		NoSuggestions = 256,
		Password = NoSuggestions | PasswordClass
    }
}
