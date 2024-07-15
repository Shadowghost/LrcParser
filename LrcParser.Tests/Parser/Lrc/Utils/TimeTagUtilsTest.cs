// Copyright (c) karaoke.dev <contact@karaoke.dev>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using LrcParser.Parser.Lrc.Utils;
using NUnit.Framework;

namespace LrcParser.Tests.Parser.Lrc.Utils;

public class TimeTagUtilsTest
{
    [TestCase("[00:01:00]", 1000)]
    [TestCase("[00:01.50]", 1500)]
    public void TestTimeTagToMillisecond(string timeTag, int milliseconds)
    {
        var actual = TimeTagUtils.TimeTagToMillisecond(timeTag);

        Assert.That(actual, Is.EqualTo(milliseconds));
    }

    [TestCase(1000, "[00:01.00]")]
    [TestCase(1500, "[00:01.50]")]
    public void TestTimeTagToMillisecond(int milliseconds, string timeTag)
    {
        var actual = TimeTagUtils.MillisecondToTimeTag(milliseconds);

        Assert.That(actual, Is.EqualTo(timeTag));
    }
}
