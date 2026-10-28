
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

class TextTests
{
    public static int RunTests()
    {
        int passed = 0;
        int failed = 0;

        void Check(string name, Action test)
        {
            try
            {
                test();
                passed++;
                Console.WriteLine("PASSED: " + name);
            }
            catch (Exception ex)
            {
                failed++;
                Console.WriteLine("FAIL: " + name);
                Console.WriteLine("     " + ex.Message);
            }
        }

        Check("Parser splits text into sentences",
            ParserSplitsSentences);

        Check("Parser recognizes words and punctuation",
            ParserRecognizesTokens);

        Check("Parser preserves whitespace",
            ParserPreservesWhitespace);

        Check("Sentence counts words",
            SentenceCountsWords);

        Check("Sentence identifies questions",
            SentenceIdentifiesQuestions);

        Check("Sentences sort by word count",
            SentencesSortByWordCount);

        Check("Sentences sort by length",
            SentencesSortByLength);

        Check("Finds words of given length in questions",
            FindsWordsInQuestions);

        Check("Deletes words starting with consonants",
            DeletesWordsStartingWithConsonants);

        Check("Replaces words of given length",
            ReplacesWordsOfGivenLength);

        Check("Invalid sentence number returns false",
            InvalidSentenceNumberReturnsFalse);

        Check("Removes stopwords",
            RemovesStopwords);

        Check("ToString reconstructs text",
            ToStringReconstructsText);

        Check("XML serialization works",
            XmlSerializationWorks);

        Console.WriteLine();
        Console.WriteLine($"Passed: {passed}; Failed: {failed}");

        return failed == 0 ? 0 : 1;
    }

    private static void Equal<T>(T expected, T actual)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
        {
            throw new Exception(
                $"Expected: [{expected}], but was: [{actual}]");
        }
    }

    private static void Contains(string text, string expected)
    {
        if (!text.Contains(expected))
        {
            throw new Exception(
                $"Expected text to contain: [{expected}]");
        }
    }

    private static void SequenceEqual<T>(
        IEnumerable<T> expected,
        IEnumerable<T> actual)
    {
        if (!expected.SequenceEqual(actual))
        {
            throw new Exception(
                $"Expected: [{string.Join(", ", expected)}], " +
                $"but was: [{string.Join(", ", actual)}]");
        }
    }

    private static void ParserSplitsSentences()
    {
        Text text = Parser.ParseText("Hello. How are you? Fine!");

        Equal(3, text.sentences.Count);
    }

    private static void ParserRecognizesTokens()
    {
        Text text = Parser.ParseText("Hello, world!");

        Sentence sentence = text.sentences[0];

        Equal(4, sentence.Tokens.Count);

        if (sentence.Tokens[0] is not Word)
            throw new Exception("Hello should be a Word");

        if (sentence.Tokens[1] is not Punctuation)
            throw new Exception("Comma should be Punctuation");

        if (sentence.Tokens[2] is not Word)
            throw new Exception("world should be a Word");

        if (sentence.Tokens[3] is not Punctuation)
            throw new Exception("Exclamation mark should be Punctuation");

        Equal("Hello", sentence.Tokens[0].Content);
        Equal(",", sentence.Tokens[1].Content);
        Equal("world", sentence.Tokens[2].Content);
        Equal("!", sentence.Tokens[3].Content);
    }

    private static void ParserPreservesWhitespace()
    {
        string source = "Hello,   world!";

        Text text = Parser.ParseText(source);

        Equal(source, text.ToString());
    }

    private static void SentenceCountsWords()
    {
        Text text = Parser.ParseText("One two, three!");

        Equal(3, text.sentences[0].WordCount);
    }

    private static void SentenceIdentifiesQuestions()
    {
        Text text = Parser.ParseText("Hello. How are you? Fine!");

        Equal(false, text.sentences[0].IsQuestion);
        Equal(true, text.sentences[1].IsQuestion);
        Equal(false, text.sentences[2].IsQuestion);
    }

    private static void SentencesSortByWordCount()
    {
        Text text = Parser.ParseText("One. One two three. One two.");

        int[] counts = text.SentencesByWordCount()
            .Select(s => s.WordCount)
            .ToArray();

        SequenceEqual(new[] { 1, 2, 3 }, counts);
    }

    private static void SentencesSortByLength()
    {
        Text text = Parser.ParseText("A. Longer sentence.");

        int[] lengths = text.SentencesByLength()
            .Select(s => s.ToString().Length)
            .ToArray();

        if (lengths.Length != 2)
            throw new Exception("Expected two sentences");

        if (lengths[0] > lengths[1])
            throw new Exception("Sentences are not sorted by length");
    }

    private static void FindsWordsInQuestions()
    {
        Text text = Parser.ParseText("Cat? Dog and fox.");

        List<string> words = text.WordByLengthInQuestion(3);

        SequenceEqual(new[] { "Cat" }, words);
    }

    private static void DeletesWordsStartingWithConsonants()
    {
        Text text = Parser.ParseText("cat apple dog!");

        text.DeleteWordsSWConLetterByLength(3);

        List<string> words = text.sentences[0].Tokens
            .OfType<Word>()
            .Select(w => w.Content)
            .ToList();

        SequenceEqual(new[] { "apple" }, words);
    }

    private static void ReplacesWordsOfGivenLength()
    {
        Text text = Parser.ParseText("cat dog!");

        bool result =
            text.SwapWordByLengthWithSubstring(1, 3, "fox");

        Equal(true, result);

        List<string> words = text.sentences[0].Tokens
            .OfType<Word>()
            .Select(w => w.Content)
            .ToList();

        SequenceEqual(new[] { "fox", "fox" }, words);
    }

    private static void InvalidSentenceNumberReturnsFalse()
    {
        Text text = Parser.ParseText("Hello!");

        Equal(
            false,
            text.SwapWordByLengthWithSubstring(0, 5, "World"));

        Equal(
            false,
            text.SwapWordByLengthWithSubstring(2, 5, "World"));
    }

    private static void RemovesStopwords()
    {
        Text text = Parser.ParseText("The cat is here!");

        text.RemoveStopwords(new List<string> { "the", "IS" });

        List<string> words = text.sentences[0].Tokens
            .OfType<Word>()
            .Select(w => w.Content)
            .ToList();

        SequenceEqual(new[] { "cat", "here" }, words);
    }

    private static void ToStringReconstructsText()
    {
        string source = "Hello, world!  How are you?";

        Text text = Parser.ParseText(source);

        Equal(source, text.ToString());
    }

    private static void XmlSerializationWorks()
    {
        Text text = Parser.ParseText("Hello, world!");

        var serializer = new XmlSerializer(typeof(Text));

        using var writer = new StringWriter();

        serializer.Serialize(writer, text);

        string xml = writer.ToString();

        Contains(xml, "<Text");
        Contains(xml, "Sentence");
        Contains(xml, "Word");
        Contains(xml, "Punctuation");
    }
}

