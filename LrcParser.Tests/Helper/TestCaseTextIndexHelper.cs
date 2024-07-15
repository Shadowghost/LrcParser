// Copyright (c) karaoke.dev <contact@karaoke.dev>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Text.RegularExpressions;
using LrcParser.Extension;
using LrcParser.Model;

namespace LrcParser.Tests.Helper;

public static partial class TestCaseTextIndexHelper
{
    [GeneratedRegex("(?<index>[-0-9]+),(?<state>start|end)")]
    private static partial Regex TextIndexRegex();

    public static TextIndex ParseTextIndex(string str)
    {
        var regex = TextIndexRegex();
        var result = regex.Match(str);
        if (!result.Success)
            throw new RegexMatchTimeoutException(nameof(str));

        int index = result.GetGroupValue<int>("index");
        var state = result.GetGroupValue<string>("state") == "start" ? IndexState.Start : IndexState.End;

        return new TextIndex(index, null, state);
    }
}
