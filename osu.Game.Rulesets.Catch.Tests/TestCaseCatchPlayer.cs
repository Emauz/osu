﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using NUnit.Framework;

namespace osu.Game.Rulesets.Catch.Tests
{
    [TestFixture]
    public class TestCaseCatchPlayer : Game.Tests.Visual.TestCasePlayer
    {
        public TestCaseCatchPlayer() : base(new CatchRuleset())
        {
        }
    }
}
