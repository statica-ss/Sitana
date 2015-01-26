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

namespace Sitana.Framework.GamerApi
{
    public class AchievementInfo
    {
        public readonly string Id;
        public int Completion = 0;

        public AchievementInfo(string id)
        {
            Id = id;
        }
    }

    public class LeaderboardInfo
    {
        public readonly string Id;
        public int Score = 0;

        public LeaderboardInfo(string id)
        {
            Id = id;
        }
    }

    public delegate void AchievementInfoDelegate(AchievementInfo[] achievements);
    public delegate void LeaderboardDelegate(LeaderboardInfo[] leaderboard);

    public delegate void AchievementCompletedDelegate(Achievement achievement);

	public delegate void GamerErrorDelegate(GamerError error);

	public enum GamerError
	{
		ErrorSignIn
	}
}

