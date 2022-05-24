// Copyright (c) karaoke.dev <contact@karaoke.dev>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using LrcParser.Parser.Lrc.Lines;
using LrcParser.Parser.Lrc.Metadata;
using LrcParser.Tests.Parser.Lines;
using NUnit.Framework;

namespace LrcParser.Tests.Parser.Lrc.Lines;

public class LrcRubyParserTest : BaseSingleLineParserTest<LrcRubyParser, LrcRuby>
{
    [TestCase("@Ruby1=帰,かえ,[00:53:19],[01:24:77]", "帰", "かえ", 53190, 84770)]
    [TestCase("@Ruby1=帰,かえ,[01:24:77]", "帰", "かえ", 84770, null)]
    [TestCase("@Ruby1=帰,かえ,,[01:24:77]", "帰", "かえ", null, 84770)]
    [TestCase("@Ruby1=帰,かえ", "帰", "かえ", null, null)]
    [TestCase("@Ruby1=帰,か[01:24:77]え", "帰", "か[01:24:77]え", null, null)] // todo: maybe should be able to get the time in the ruby.
    public void TestDecode(string rubyTag, string parent, string ruby, int? startTime, int? endTime)
    {
        var expected = new LrcRuby
        {
            Parent = parent,
            Ruby = ruby,
            StartTime = startTime,
            EndTime = endTime
        };
        var actual = Decode(rubyTag);

        Assert.AreEqual(expected.Ruby, actual.Ruby);
        Assert.AreEqual(expected.Parent, actual.Parent);
        Assert.AreEqual(expected.StartTime, actual.StartTime);
        Assert.AreEqual(expected.EndTime, actual.EndTime);
    }


    [TestCase("帰", "かえ", 53190, 84770, "@Ruby1=帰,かえ,[00:53.19],[01:24.77]")]
    [TestCase("帰", "かえ", 84770, null, "@Ruby1=帰,かえ,[01:24.77]")]
    [TestCase("帰", "かえ", null, 84770, "@Ruby1=帰,かえ,,[01:24.77]")]
    [TestCase("帰", "かえ", null, null, "@Ruby1=帰,かえ")]
    public void TestEncode(string parent, string ruby, int? startTime, int? endTime, string expected)
    {
        var rubyTag = new LrcRuby
        {
            Parent = parent,
            Ruby = ruby,
            StartTime = startTime,
            EndTime = endTime
        };
        var actual = Encode(rubyTag);

        Assert.AreEqual(expected, actual);
    }
}
