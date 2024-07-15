// Copyright (c) karaoke.dev <contact@karaoke.dev>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LrcParser.Extension;
using LrcParser.Model;

namespace LrcParser.Tests.Helper;

public static partial class TestCaseTagHelper
{
    [GeneratedRegex("(?<index>[-0-9]+),(?<time>[-0-9]+|s*|),(?<state>start|end)]")]
    private static partial Regex TimeTagRegex();

    /// <summary>
    /// Process test case time tag string format into <see cref="TextIndex"/>
    /// </summary>
    /// <example>
    /// [0,1000,start]
    /// </example>
    /// <param name="str">Time tag string format</param>
    /// <returns><see cref="TextIndex"/>Time tag object</returns>
    public static TextIndex? ParseTimeTag(string str)
    {
        if (string.IsNullOrEmpty(str))
            return null;

        var regex = TimeTagRegex();
        var result = regex.Match(str);
        if (!result.Success)
            throw new RegexMatchTimeoutException(nameof(str));

        int index = result.GetGroupValue<int>("index");
        var state = result.GetGroupValue<string>("state") == "start" ? IndexState.Start : IndexState.End;
        int time = result.GetGroupValue<int?>("time") ?? 0;

        return new TextIndex(index, time, state);
    }

    public static List<TextIndex> ParseTimeTags(IEnumerable<string> strings)
    {
        var list = strings.Select(ParseTimeTag).ToList();
        list.Sort();

        return list.Where(u => u.HasValue).Select(u => u.Value).ToList();
    }
}
