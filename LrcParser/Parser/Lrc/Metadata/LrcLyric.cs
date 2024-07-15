// Copyright (c) karaoke.dev <contact@karaoke.dev>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using LrcParser.Model;

namespace LrcParser.Parser.Lrc.Metadata;

public class LrcLyric
{
    /// <summary>
    /// Text
    /// </summary>
    public string Text { get; set; } = "";

    /// <summary>
    /// Time tags
    /// </summary>
    public List<TextIndex> TimeTags { get; set; } = new();
}
