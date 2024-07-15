// Copyright (c) karaoke.dev <contact@karaoke.dev>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Text.RegularExpressions;
using LrcParser.Model;
using LrcParser.Utils;

namespace LrcParser.Parser.Lrc.Utils;

internal static class LrcTimedTextUtils
{
    internal static Tuple<string, List<TextIndex>> TimedTextToObject(string timedText)
    {
        var timeTags = new List<TextIndex>();
        if (string.IsNullOrEmpty(timedText))
            return new Tuple<string, List<TextIndex>>("", timeTags);

        var timeTagRegex = new Regex(@"[\[<]\d\d:\d\d[:.]\d\d(\d)?[\]>]");
        var matchTimeTags = timeTagRegex.Matches(timedText);

        var lastMatch = matchTimeTags.Last();
        var endOfLastMatch = lastMatch.Index + lastMatch.Length;
        var isMultiOccurenceTag = matchTimeTags.Sum(t => t.Length - t.Index - 1) == endOfLastMatch;

        var text = "";
        if (isMultiOccurenceTag)
        {
            foreach (var match in matchTimeTags.ToArray())
            {
                timeTags.Add(new TextIndex(0, TimeTagUtils.TimeTagToMillisecond(match.Value)));
            }

            text = timedText[endOfLastMatch..];
        }
        else
        {
            foreach (var match in matchTimeTags.ToArray())
            {
                var endTextIndex = timedText.Length;
                var startIndex = 0;
                var endIndex = match.Index;

                if (startIndex < endIndex)
                {
                    // add the text.
                    text += timedText[startIndex..endIndex];
                }

                // update the new start for next time-tag calculation.
                startIndex = endIndex + match.Length;

                // add the time-tag.
                var hasText = startIndex < endTextIndex;
                var isEmptyStringNext = hasText && timedText[startIndex] == ' ';

                var state = hasText && !isEmptyStringNext ? IndexState.Start : IndexState.End;
                var textIndex = text.Length - (state == IndexState.Start ? 0 : 1);
                var time = TimeTagUtils.TimeTagToMillisecond(match.Value);

                // using try add because it might be possible with duplicated time-tag position in the lyric.
                timeTags.Add(new TextIndex(textIndex, time, state));

                // should add remaining text at the right of the end time-tag.
                text += timedText[startIndex..endTextIndex];
            }
        }

        return new Tuple<string, List<TextIndex>>(text, timeTags);
    }

    internal static string ToTimedText(string text, List<TextIndex> timeTags)
    {
        var insertIndex = 0;

        var timedText = text;

        foreach (var tag in timeTags)
        {
            var timeTagString = TimeTagUtils.MillisecondToTimeTag(tag.Time ?? 0);
            var stringIndex = TextIndexUtils.ToGapIndex(tag);
            timedText = timedText.Insert(insertIndex + stringIndex, timeTagString);

            insertIndex += timeTagString.Length;
        }

        return timedText;
    }
}
