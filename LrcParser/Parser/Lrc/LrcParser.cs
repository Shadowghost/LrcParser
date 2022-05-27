// Copyright (c) karaoke.dev <contact@karaoke.dev>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Text.RegularExpressions;
using LrcParser.Model;
using LrcParser.Parser.Lrc.Lines;
using LrcParser.Parser.Lrc.Metadata;
using LrcParser.Utils;

namespace LrcParser.Parser.Lrc;

/// <summary>
/// Parser for encode and decode .lrc lyric format
/// </summary>
public class LrcParser : LyricParser
{
    public LrcParser()
    {
        Register<LrcRubyParser>();
        Register<LrcLyricParser>();
    }

    protected override Song PostProcess(List<object> values)
    {
        var lyrics = values.OfType<LrcLyric>();
        var rubies = values.OfType<LrcRuby>();

        return new Song
        {
            Lyrics = lyrics.Select(l => new Lyric
            {
                Text = l.Text,
                TimeTags = getTimeTags(l.TimeTags),
                RubyTags = getRubyTags(rubies, l).ToList()
            }).ToList()
        };

        static SortedDictionary<TextIndex, int?> getTimeTags(SortedDictionary<TextIndex, int> timeTags)
            => new(timeTags.ToDictionary(k => k.Key, v => v.Value as int?));

        static IEnumerable<RubyTag> getRubyTags(IEnumerable<LrcRuby> rubyTags, LrcLyric lyric)
        {
            var text = lyric.Text;
            var timeTags = lyric.TimeTags;
            foreach (var rubyTag in rubyTags)
            {
                if (string.IsNullOrEmpty(rubyTag.Ruby) || string.IsNullOrEmpty(rubyTag.Parent))
                    continue;

                if(rubyTag.Ruby == rubyTag.Parent)
                   continue;

                var matches = new Regex(rubyTag.Parent).Matches(text);

                foreach (var match in matches.ToArray())
                {
                    var startTextIndex = match.Index;
                    var endTextIndex = startTextIndex + match.Length;
                    var startTimeTag = timeTags.Reverse().LastOrDefault(x => TextIndexUtils.ToStringIndex(x.Key) >= startTextIndex);
                    var endTimeTag = timeTags.FirstOrDefault(x => TextIndexUtils.ToStringIndex(x.Key) >= endTextIndex);

                    if(rubyTag.StartTime.HasValue && rubyTag.StartTime > startTimeTag.Value)
                        continue;

                    if(rubyTag.EndTime.HasValue && rubyTag.EndTime < endTimeTag.Value)
                        continue;

                    yield return new RubyTag
                    {
                        Text = rubyTag.Ruby,
                        StartIndex = TextIndexUtils.ToStringIndex(startTimeTag.Key),
                        EndIndex = TextIndexUtils.ToStringIndex(endTimeTag.Key)
                    };
                }
            }
        }
    }

    protected override IEnumerable<object> PreProcess(Song song)
    {
        var lyrics = song.Lyrics;

        // first, should return the time-tag first.
        foreach (var lyric in lyrics)
        {
            yield return new LrcLyric
            {
                Text = lyric.Text,
                TimeTags = getTimeTags(lyric.TimeTags),
            };
        }

        // give it a line if contains ruby.
        if (lyrics.Any(l => l.RubyTags.Any()))
            yield return new object();

        // then, export the ruby.
        var rubiesWithSameParent = lyrics.Select(getRubyTags).SelectMany(x => x).GroupBy(x => x.Parent);

        foreach (var groupWithSameParent in rubiesWithSameParent)
        {
            // should process the value with same parent text.
            var rubiesWithSameRuby = groupWithSameParent.GroupBy(x => x.Ruby).ToList();

            foreach (var groupWithSameRuby in rubiesWithSameRuby)
            {
                // should process the value with same parent text and ruby text.
                var isFirst = rubiesWithSameRuby.IndexOf(groupWithSameRuby) == 0;
                var isLast = rubiesWithSameRuby.IndexOf(groupWithSameRuby) == rubiesWithSameRuby.Count - 1;

                var minStartTime = isFirst ? null : groupWithSameRuby.Min(x => x.StartTime);
                var maxEndTime = isLast ? null : groupWithSameRuby.Max(x => x.EndTime);

                yield return new LrcRuby
                {
                    Ruby = groupWithSameRuby.Key,
                    Parent = groupWithSameParent.Key,
                    StartTime = minStartTime,
                    EndTime = maxEndTime
                };
            }
        }

        static SortedDictionary<TextIndex, int> getTimeTags(SortedDictionary<TextIndex, int?> timeTags)
            => new(timeTags.Where(x => x.Value.HasValue).ToDictionary(k => k.Key, v => v.Value.Value));

        static IEnumerable<LrcRuby> getRubyTags(Lyric lyric)
        {
            var timeTags = lyric.TimeTags;

            foreach (var rubyTag in lyric.RubyTags)
            {
                var startIndex = rubyTag.StartIndex;
                var endIndex = rubyTag.EndIndex;

                var startTimeTag = timeTags.Reverse().LastOrDefault(x => TextIndexUtils.ToStringIndex(x.Key) >= startIndex && x.Value.HasValue).Value;
                var endTimeTag = timeTags.FirstOrDefault(x => TextIndexUtils.ToStringIndex(x.Key) >= endIndex && x.Value.HasValue).Value;

                yield return new LrcRuby
                {
                    Ruby = rubyTag.Text,
                    Parent = lyric.Text[startIndex..endIndex],
                    StartTime = startTimeTag,
                    EndTime = endTimeTag,
                };
            }
        }
    }
}
