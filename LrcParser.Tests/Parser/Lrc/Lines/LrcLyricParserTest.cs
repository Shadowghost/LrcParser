// Copyright (c) karaoke.dev <contact@karaoke.dev>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using LrcParser.Parser.Lrc.Lines;
using LrcParser.Parser.Lrc.Metadata;
using LrcParser.Tests.Helper;
using LrcParser.Tests.Parser.Lines;
using NUnit.Framework;

namespace LrcParser.Tests.Parser.Lrc.Lines;

public class LrcLyricParserTest : BaseSingleLineParserTest<LrcLyricParser, LrcLyric>
{
    [TestCase("[00:17:97]帰[00:18:37]り[00:18:55]道[00:18:94]は[00:19:22]", true)]
    [TestCase("karaoke", true)]
    [TestCase("", false)]
    [TestCase(null, false)]
    [TestCase("@Ruby1=帰,かえ", true)] // will take off this if no other parser to process this line.
    public void TestCanDecode(string text, bool expected)
    {
        var actual = CanDecode(text);
        Assert.That(actual, Is.EqualTo(expected));
    }

    [TestCase("[00:17:97]帰[00:18:37]り[00:18:55]道[00:18:94]は[00:19:22]", "帰り道は", new[] { "[0,17970,start]", "[1,18370,start]", "[2,18550,start]", "[3,18940,start]", "[3,19220,end]" })]
    [TestCase("帰[00:18:37]り[00:18:55]道[00:18:94]は[00:19:22]", "帰り道は", new[] { "[1,18370,start]", "[2,18550,start]", "[3,18940,start]", "[3,19220,end]" })]
    [TestCase("[00:17:97]帰[00:18:37]り[00:18:55]道[00:18:94]は", "帰り道は", new[] { "[0,17970,start]", "[1,18370,start]", "[2,18550,start]", "[3,18940,start]" })]
    [TestCase("帰り道は", "帰り道は", new string[] { })]
    [TestCase("", "", new string[] { })]
    [TestCase(null, "", new string[] { })]
    public void TestDecode(string lyric, string text, string[] timeTags)
    {
        var expected = new LrcLyric
        {
            Text = text,
            TimeTags = TestCaseTagHelper.ParseTimeTags(timeTags),
        };
        var actual = Decode(lyric);

        Assert.That(actual.Text, Is.EqualTo(expected.Text));
        Assert.That(actual.TimeTags, Is.EqualTo(expected.TimeTags));
    }

    [TestCase("帰り道は", new[] { "[0,17970,start]", "[1,18370,start]", "[2,18550,start]", "[3,18940,start]", "[3,19220,end]" }, "[00:17.97]帰[00:18.37]り[00:18.55]道[00:18.94]は[00:19.22]")]
    [TestCase("帰り道は", new[] { "[1,18370.start]", "[2,18550,start]", "[3,18940,start]", "[3,19220,end]" }, "帰[00:18.37]り[00:18.55]道[00:18.94]は[00:19.22]")]
    [TestCase("帰り道は", new[] { "[0,17970,start]", "[1,18370,start]", "[2,18550,start]", "[3,18940,start]" }, "[00:17.97]帰[00:18.37]り[00:18.55]道[00:18.94]は")]
    [TestCase("帰り道は", new string[] { }, "帰り道は")]
    [TestCase("", new string[] { }, "")]
    public void TestEncode(string text, string[] timeTags, string expected)
    {
        var lyric = new LrcLyric
        {
            Text = text,
            TimeTags = TestCaseTagHelper.ParseTimeTags(timeTags),
        };
        var actual = Encode(lyric);

        Assert.That(actual, Is.EqualTo(expected));
    }
}
