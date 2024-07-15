// Copyright (c) karaoke.dev <contact@karaoke.dev>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using LrcParser.Model;
using NUnit.Framework;

namespace LrcParser.Tests.Parser.Lrc;

public class LrcParserTest : BaseLyricParserTest<LrcParser.Parser.Lrc.LrcParser>
{
    [Test]
    public void TestDecode()
    {
        var lrcText = new[]
        {
            "[00:17:97]帰[00:18:37]り[00:18:55]道[00:18:94]は[00:19:22]",
        };

        var song = new Song
        {
            Lyrics =
            [
                new()
                {
                    Text = "帰り道は",
                    TimeTags = new List<TextIndex>
                    {
                        { new TextIndex(0, 17970) },
                        { new TextIndex(1, 18370) },
                        { new TextIndex(2, 18550) },
                        { new TextIndex(3, 18940) },
                        { new TextIndex(3, 19220, IndexState.End) },
                    },
                },
            ],
        };

        checkDecode(lrcText, song);
    }

    [Test]
    public void TestDecodeWithRuby()
    {
        var lrcText = new[]
        {
            "[00:01:00]島[00:02:00]島[00:03:00]島[00:04:00]",
            "@Ruby1=島,しま,,[00:02:00]",
            "@Ruby2=島,じま,[00:02:00],[00:03:00]",
            "@Ruby3=島,とう,[00:03:00]",
        };

        var song = new Song
        {
            Lyrics =
            [
                new()
                {
                    Text = "島島島",
                    TimeTags = new List<TextIndex>
                    {
                        { new TextIndex(0, 1000) },
                        { new TextIndex(1, 2000) },
                        { new TextIndex(2, 3000) },
                        { new TextIndex(2, 4000, IndexState.End) },
                    },
                    RubyTags =
                    [
                        new()
                        {
                            Text = "しま",
                            StartCharIndex = 0,
                            EndCharIndex = 0,
                        },
                        new()
                        {
                            Text = "じま",
                            StartCharIndex = 1,
                            EndCharIndex = 1,
                        },
                        new()
                        {
                            Text = "とう",
                            StartCharIndex = 2,
                            EndCharIndex = 2,
                        },
                    ],
                },
            ],
        };

        checkDecode(lrcText, song);
    }

    [Test]
    public void TestDecodeWithRubyAndRubyTimeTag()
    {
        var lrcText = new[]
        {
            "[00:01:00]島[00:02:00]島[00:03:00]島[00:04:00]",
            "@Ruby1=島,し[00:00:50]ま,,[00:02:00]",
            "@Ruby2=島,じ[00:00:50]ま,[00:02:00],[00:03:00]",
            "@Ruby3=島,と[00:00:50]う,[00:03:00]",
        };

        var song = new Song
        {
            Lyrics =
            [
                new()
                {
                    Text = "島島島",
                    TimeTags = new List<TextIndex>
                    {
                        { new TextIndex(0, 1000) },
                        { new TextIndex(1, 2000) },
                        { new TextIndex(2, 3000) },
                        { new TextIndex(2, 4000, IndexState.End) },
                    },
                    RubyTags =
                    [
                        new()
                        {
                            Text = "しま",
                            TimeTags = new List<TextIndex>
                            {
                                { new TextIndex(1, 1500) },
                            },
                            StartCharIndex = 0,
                            EndCharIndex = 0,
                        },
                        new()
                        {
                            Text = "じま",
                            TimeTags = new List<TextIndex>
                            {
                                { new TextIndex(1, 2500) },
                            },
                            StartCharIndex = 1,
                            EndCharIndex = 1,
                        },
                        new()
                        {
                            Text = "とう",
                            TimeTags = new List<TextIndex>
                            {
                                { new TextIndex(1, 3500) },
                            },
                            StartCharIndex = 2,
                            EndCharIndex = 2,
                        },
                    ],
                },
            ],
        };

        checkDecode(lrcText, song);
    }

    [Test]
    public void TestDecodeWithSameRubyWithDifferentRubyTimeTag()
    {
        var lrcText = new[]
        {
            "[00:01:00]島[00:02:00]島[00:03:00]島[00:04:00]",
            "@Ruby1=島,し[00:00:40]ま,,[00:02:00]",
            "@Ruby2=島,し[00:00:50]ま,[00:02:00],[00:03:00]",
            "@Ruby3=島,し[00:00:60]ま,[00:03:00]",
        };

        var song = new Song
        {
            Lyrics =
            [
                new()
                {
                    Text = "島島島",
                    TimeTags = new List<TextIndex>
                    {
                        { new TextIndex(0, 1000) },
                        { new TextIndex(1, 2000) },
                        { new TextIndex(2, 3000) },
                        { new TextIndex(2, 4000, IndexState.End) },
                    },
                    RubyTags =
                    [
                        new()
                        {
                            Text = "しま",
                            TimeTags = new List<TextIndex>
                            {
                                { new TextIndex(1, 1400) },
                            },
                            StartCharIndex = 0,
                            EndCharIndex = 0,
                        },
                        new()
                        {
                            Text = "しま",
                            TimeTags = new List<TextIndex>
                            {
                                { new TextIndex(1, 2500) },
                            },
                            StartCharIndex = 1,
                            EndCharIndex = 1,
                        },
                        new()
                        {
                            Text = "しま",
                            TimeTags = new List<TextIndex>
                            {
                                { new TextIndex(1, 3600) },
                            },
                            StartCharIndex = 2,
                            EndCharIndex = 2,
                        },
                    ],
                },
            ],
        };

        checkDecode(lrcText, song);
    }

    [Test]
    public void TestDecodeWithNoTimeRangeRuby()
    {
        var lrcText = new[]
        {
            "カラオケ",
            "@Ruby1=カ,か",
            "@Ruby2=ラ,ら",
            "@Ruby3=オ,お",
            "@Ruby4=ケ,け",
        };

        var song = new Song
        {
            Lyrics =
            [
                new()
                {
                    Text = "カラオケ",
                    RubyTags =
                    [
                        new()
                        {
                            Text = "か",
                            StartCharIndex = 0,
                            EndCharIndex = 0,
                        },
                        new()
                        {
                            Text = "ら",
                            StartCharIndex = 1,
                            EndCharIndex = 1,
                        },
                        new()
                        {
                            Text = "お",
                            StartCharIndex = 2,
                            EndCharIndex = 2,
                        },
                        new()
                        {
                            Text = "け",
                            StartCharIndex = 3,
                            EndCharIndex = 3,
                        },
                    ],
                },
            ],
        };

        checkDecode(lrcText, song);
    }

    [Test]
    public void TestDecodeWithRubyInDifferentLine()
    {
        var lrcText = new[]
        {
            "[00:01:00]島[00:02:00]",
            "[00:03:00]島[00:04:00]",
            "[00:05:00]島[00:06:00]",
            "@Ruby1=島,しま,,[00:02:00]",
            "@Ruby2=島,じま,[00:03:00],[00:04:00]",
            "@Ruby3=島,とう,[00:05:00]",
        };

        var song = new Song
        {
            Lyrics =
            [
                new()
                {
                    Text = "島",
                    TimeTags = new List<TextIndex>
                    {
                        { new TextIndex(0, 1000) },
                        { new TextIndex(0, 2000, IndexState.End) },
                    },
                    RubyTags =
                    [
                        new()
                        {
                            Text = "しま",
                            StartCharIndex = 0,
                            EndCharIndex = 0,
                        },
                    ],
                },
                new()
                {
                    Text = "島",
                    TimeTags = new List<TextIndex>
                    {
                        { new TextIndex(0, 3000) },
                        { new TextIndex(0, 4000, IndexState.End) },
                    },
                    RubyTags =
                    [
                        new()
                        {
                            Text = "じま",
                            StartCharIndex = 0,
                            EndCharIndex = 0,
                        },
                    ],
                },
                new()
                {
                    Text = "島",
                    TimeTags = new List<TextIndex>
                    {
                        { new TextIndex(0, 5000) },
                        { new TextIndex(0, 6000, IndexState.End) },
                    },
                    RubyTags =
                    [
                        new()
                        {
                            Text = "とう",
                            StartCharIndex = 0,
                            EndCharIndex = 0,
                        },
                    ],
                },
            ],
        };

        checkDecode(lrcText, song);
    }

    [Test]
    public void TestDecodeWithInvalid()
    {
        // should not generate the ruby if ruby text is same as parent text.
        var lrcText = new[]
        {
            "[00:01:00]島[00:02:00]",
            "@Ruby1=島,島",
        };

        var song = new Song
        {
            Lyrics =
            [
                new()
                {
                    Text = "島",
                    TimeTags = new List<TextIndex>
                    {
                        { new TextIndex(0, 1000) },
                        { new TextIndex(0, 2000, IndexState.End) },
                    },
                },
            ],
        };

        checkDecode(lrcText, song);
    }

    [Test]
    public void TestEncode()
    {
        var song = new Song
        {
            Lyrics =
            [
                new()
                {
                    Text = "帰り道は",
                    TimeTags = new List<TextIndex>
                    {
                        { new TextIndex(0, 17970) },
                        { new TextIndex(1, 18370) },
                        { new TextIndex(2, 18550) },
                        { new TextIndex(3, 18940) },
                        { new TextIndex(3, 19220, IndexState.End) },
                    },
                },
            ],
        };

        var lrcText = new[]
        {
            "[00:17.97]帰[00:18.37]り[00:18.55]道[00:18.94]は[00:19.22]",
        };

        checkEncode(song, lrcText);
    }

    [Test]
    public void TestEncodeWithRuby()
    {
        var song = new Song
        {
            Lyrics =
            [
                new()
                {
                    Text = "島島島",
                    TimeTags = new List<TextIndex>
                    {
                        { new TextIndex(0, 1000) },
                        { new TextIndex(1, 2000) },
                        { new TextIndex(2, 3000) },
                        { new TextIndex(2, 4000, IndexState.End) },
                    },
                    RubyTags =
                    [
                        new()
                        {
                            Text = "しま",
                            StartCharIndex = 0,
                            EndCharIndex = 0,
                        },
                        new()
                        {
                            Text = "じま",
                            StartCharIndex = 1,
                            EndCharIndex = 1,
                        },
                        new()
                        {
                            Text = "とう",
                            StartCharIndex = 2,
                            EndCharIndex = 2,
                        },
                    ],
                },
            ],
        };

        var lrcText = new[]
        {
            "[00:01.00]島[00:02.00]島[00:03.00]島[00:04.00]",
            "",
            "@Ruby1=島,しま,,[00:02.00]",
            "@Ruby2=島,じま,[00:02.00],[00:03.00]",
            "@Ruby3=島,とう,[00:03.00]",
        };

        checkEncode(song, lrcText);
    }

    [Test]
    public void TestEncodeWithRubyAndRubyTimeTag()
    {
        var song = new Song
        {
            Lyrics =
            [
                new()
                {
                    Text = "島島島",
                    TimeTags = new List<TextIndex>
                    {
                        { new TextIndex(0, 1000) },
                        { new TextIndex(1, 2000) },
                        { new TextIndex(2, 3000) },
                        { new TextIndex(2, 4000, IndexState.End) },
                    },
                    RubyTags =
                    [
                        new()
                        {
                            Text = "しま",
                            TimeTags = new List<TextIndex>
                            {
                                { new TextIndex(1, 1500) },
                            },
                            StartCharIndex = 0,
                            EndCharIndex = 0,
                        },
                        new()
                        {
                            Text = "じま",
                            TimeTags = new List<TextIndex>
                            {
                                { new TextIndex(1, 2500) },
                            },
                            StartCharIndex = 1,
                            EndCharIndex = 1,
                        },
                        new()
                        {
                            Text = "とう",
                            TimeTags = new List<TextIndex>
                            {
                                { new TextIndex(1, 3500) },
                            },
                            StartCharIndex = 2,
                            EndCharIndex = 2,
                        },
                    ],
                },
            ],
        };

        var lrcText = new[]
        {
            "[00:01.00]島[00:02.00]島[00:03.00]島[00:04.00]",
            "",
            "@Ruby1=島,し[00:00.50]ま,,[00:02.00]",
            "@Ruby2=島,じ[00:00.50]ま,[00:02.00],[00:03.00]",
            "@Ruby3=島,と[00:00.50]う,[00:03.00]",
        };

        checkEncode(song, lrcText);
    }

    [Test]
    public void TestEncodeWithSameRubyWithDifferentRubyTimeTag()
    {
        var song = new Song
        {
            Lyrics =
            [
                new()
                {
                    Text = "島島島島",
                    TimeTags = new List<TextIndex>
                    {
                        { new TextIndex(0, 1000) },
                        { new TextIndex(1, 2000) },
                        { new TextIndex(2, 3000) },
                        { new TextIndex(3, 4000) },
                        { new TextIndex(3, 5000, IndexState.End) },
                    },
                    RubyTags =
                    [
                        new()
                        {
                            Text = "しま",
                            TimeTags = new List<TextIndex>
                            {
                                { new TextIndex(1, 1400) },
                            },
                            StartCharIndex = 0,
                            EndCharIndex = 0,
                        },
                        new()
                        {
                            Text = "しま",
                            TimeTags = new List<TextIndex>
                            {
                                { new TextIndex(1, 2500) },
                            },
                            StartCharIndex = 1,
                            EndCharIndex = 1,
                        },
                        new()
                        {
                            Text = "しま",
                            TimeTags = new List<TextIndex>
                            {
                                // will merge with second time-tag
                                { new TextIndex(1, 3500) },
                            },
                            StartCharIndex = 2,
                            EndCharIndex = 2,
                        },
                        new()
                        {
                            Text = "しま",
                            TimeTags = new List<TextIndex>
                            {
                                // although the relative time is same as the first time-tag, but might not be able to merge.
                                { new TextIndex(1, 4400) },
                            },
                            StartCharIndex = 3,
                            EndCharIndex = 3,
                        },
                    ],
                },
            ],
        };

        var lrcText = new[]
        {
            "[00:01.00]島[00:02.00]島[00:03.00]島[00:04.00]島[00:05.00]",
            "",
            "@Ruby1=島,し[00:00.40]ま,,[00:02.00]",
            "@Ruby2=島,し[00:00.50]ま,[00:02.00],[00:04.00]",
            "@Ruby3=島,し[00:00.40]ま,[00:04.00]",
        };

        checkEncode(song, lrcText);
    }

    [Test]
    public void TestEncodeWithNoTimeRangeRuby()
    {
        var song = new Song
        {
            Lyrics =
            [
                new()
                {
                    Text = "カラオケ",
                    RubyTags =
                    [
                        new()
                        {
                            Text = "か",
                            StartCharIndex = 0,
                            EndCharIndex = 0,
                        },
                        new()
                        {
                            Text = "ら",
                            StartCharIndex = 1,
                            EndCharIndex = 1,
                        },
                        new()
                        {
                            Text = "お",
                            StartCharIndex = 2,
                            EndCharIndex = 2,
                        },
                        new()
                        {
                            Text = "け",
                            StartCharIndex = 3,
                            EndCharIndex = 3,
                        },
                    ],
                },
            ],
        };

        var lrcText = new[]
        {
            "カラオケ",
            "",
            "@Ruby1=カ,か",
            "@Ruby2=ラ,ら",
            "@Ruby3=オ,お",
            "@Ruby4=ケ,け",
        };

        checkEncode(song, lrcText);
    }

    [Test]
    public void TestEncodeWithRubyInDifferentLine()
    {
        var song = new Song
        {
            Lyrics =
            [
                new()
                {
                    Text = "島",
                    TimeTags = new List<TextIndex>
                    {
                        { new TextIndex(0, 1000) },
                        { new TextIndex(0, 2000, IndexState.End) },
                    },
                    RubyTags =
                    [
                        new()
                        {
                            Text = "しま",
                            StartCharIndex = 0,
                            EndCharIndex = 0,
                        },
                    ],
                },
                new()
                {
                    Text = "島",
                    TimeTags = new List<TextIndex>
                    {
                        { new TextIndex(0, 3000) },
                        { new TextIndex(0, 4000, IndexState.End) },
                    },
                    RubyTags =
                    [
                        new()
                        {
                            Text = "じま",
                            StartCharIndex = 0,
                            EndCharIndex = 0,
                        },
                    ],
                },
                new()
                {
                    Text = "島",
                    TimeTags = new List<TextIndex>
                    {
                        { new TextIndex(0, 5000) },
                        { new TextIndex(0, 6000, IndexState.End) },
                    },
                    RubyTags =
                    [
                        new()
                        {
                            Text = "とう",
                            StartCharIndex = 0,
                            EndCharIndex = 0,
                        },
                    ],
                },
            ],
        };

        var lrcText = new[]
        {
            "[00:01.00]島[00:02.00]\n[00:03.00]島[00:04.00]\n[00:05.00]島[00:06.00]",
            "",
            "@Ruby1=島,しま,,[00:02.00]",
            "@Ruby2=島,じま,[00:03.00],[00:04.00]",
            "@Ruby3=島,とう,[00:05.00]",
        };

        checkEncode(song, lrcText);
    }

    [Test]
    public void TestEncodeWithEmptyFile()
    {
        var song = new Song();

        var lrcText = new[]
        {
            "",
        };

        checkEncode(song, lrcText);
    }

    private void checkDecode(string[] lrcTexts, Song song)
    {
        var actual = Decode(string.Join('\n', lrcTexts));
        AreEqual(song, actual);
    }

    private void checkEncode(Song song, string[] lrcTexts)
    {
        var expected = string.Join('\n', lrcTexts);
        var actual = Encode(song);
        Assert.That(actual, Is.EqualTo(expected));
    }
}
