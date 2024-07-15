// Copyright (c) karaoke.dev <contact@karaoke.dev>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using LrcParser.Parser.Lrc.Utils;
using LrcParser.Tests.Helper;
using NUnit.Framework;

namespace LrcParser.Tests.Parser.Lrc.Utils;

public class LrcTimedTextUtilsTest
{
    [TestCase("[00:17:97]帰[00:18:37]り[00:18:55]道[00:18:94]は[00:19:22]", "帰り道は", new[] { "[0,17970,start]", "[1,18370,start]", "[2,18550,start]", "[3,18940,start]", "[3,19220,end]" })]
    [TestCase(" [00:17:97]帰[00:18:37]り[00:18:55]道[00:18:94]は[00:19:22]", " 帰り道は", new[] { "[1,17970,start]", "[2,18370,start]", "[3,18550,start]", "[4,18940,start]", "[4,19220,end]" })]
    [TestCase("[00:17:97]帰[00:18:37]り[00:18:55]道[00:18:94]は[00:19:22] ", "帰り道は ", new[] { "[0,17970,start]", "[1,18370,start]", "[2,18550,start]", "[3,18940,start]", "[3,19220,end]" })]
    [TestCase("帰[00:18:37]り[00:18:55]道[00:18:94]は[00:19:22]", "帰り道は", new[] { "[1,18370,start]", "[2,18550,start]", "[3,18940,start]", "[3,19220,end]" })]
    [TestCase("[00:17:97]帰[00:18:37]り[00:18:55]道[00:18:94]は", "帰り道は", new[] { "[0,17970,start]", "[1,18370,start]", "[2,18550,start]", "[3,18940,start]" })]
    [TestCase("[00:51.00][01:29.99]You gotta fight !", "You gotta fight !", new[] { "[0,51000,start]", "[0,89990,start]" })]
    [TestCase("帰り道は", "帰り道は", new string[] { })]
    [TestCase("", "", new string[] { })]
    [TestCase(null, "", new string[] { })]
    public void TestDecode(string text, string expectedText, string[] expectedTimeTags)
    {
        var (actualText, actualTimeTags) = LrcTimedTextUtils.TimedTextToObject(text);

        Assert.That(actualText, Is.EqualTo(expectedText));
        Assert.That(actualTimeTags, Is.EqualTo(TestCaseTagHelper.ParseTimeTags(expectedTimeTags)));
    }

    [TestCase("帰り道は", new[] { "[0,17970,start]", "[1,18370,start]", "[2,18550,start]", "[3,18940,start]", "[3,19220,end]" }, "[00:17.97]帰[00:18.37]り[00:18.55]道[00:18.94]は[00:19.22]")]
    [TestCase(" 帰り道は", new[] { "[1,17970,start]", "[2,18370,start]", "[3,18550,start]", "[4,18940,start]", "[4,19220,end]" }, " [00:17.97]帰[00:18.37]り[00:18.55]道[00:18.94]は[00:19.22]")]
    [TestCase("帰り道は ", new[] { "[0,17970,start]", "[1,18370,start]", "[2,18550,start]", "[3,18940,start]", "[3,19220,end]" }, "[00:17.97]帰[00:18.37]り[00:18.55]道[00:18.94]は[00:19.22] ")]
    [TestCase("帰り道は", new[] { "[1,18370,start]", "[2,18550,start]", "[3,18940,start]", "[3,19220,end]" }, "帰[00:18.37]り[00:18.55]道[00:18.94]は[00:19.22]")]
    [TestCase("帰り道は", new[] { "[0,17970,start]", "[1,18370,start]", "[2,18550,start]", "[3,18940,start]" }, "[00:17.97]帰[00:18.37]り[00:18.55]道[00:18.94]は")]
    [TestCase("帰り道は", new string[] { }, "帰り道は")]
    [TestCase("", new string[] { }, "")]
    public void TestEncode(string text, string[] timeTags, string expected)
    {
        var actual = LrcTimedTextUtils.ToTimedText(text, TestCaseTagHelper.ParseTimeTags(timeTags));

        Assert.That(actual, Is.EqualTo(expected));
    }
}
