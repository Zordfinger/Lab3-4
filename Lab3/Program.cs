using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//TextTests.RunTests();

RunProgramm();

void RunProgramm()
{
    Console.OutputEncoding = Encoding.UTF8;

    string source =
        "Hello, world and people all around the world! I am learning C#. " +
        "This is a testы. Как дела? " +
        "Привет, мир! The cat is sleeping.";

    Text text = Parser.ParseText(source);

    Console.WriteLine("Исходный текст:");
    Console.WriteLine(source);

    Console.WriteLine("\nПредложения:");
    for (int i = 0; i < text.sentences.Count; i++)
    {
        Sentence s = text.sentences[i];
        Console.WriteLine(
            $"{i + 1}. {s} | Words: {s.WordCount} | Question: {s.IsQuestion}");
    }

    Console.WriteLine("\nСортировка по количеству слов:");
    foreach (Sentence s in text.SentencesByWordCount())
        Console.WriteLine($"{s.WordCount}: {s}");

    Console.WriteLine("\nСортировка по длине:");
    foreach (Sentence s in text.SentencesByLength())
        Console.WriteLine($"{s.ToString().Length}: {s}");

    Console.WriteLine("\nСлова длины 3 в вопросительных предложениях:");
    List<string> words = text.WordByLengthInQuestion(3);
    Console.WriteLine(words.Count > 0
        ? string.Join(", ", words)
        : "Совпадений нет");

    Text deletedText = Parser.ParseText(source);
    deletedText.DeleteWordsSWConLetterByLength(3);
    Console.WriteLine("\nПосле удаления слов длины 3, начинающихся с согласной:");
    Console.WriteLine(deletedText);

    Text replacedText = Parser.ParseText(source);
    bool replaced = replacedText.SwapWordByLengthWithSubstring(1, 5, "REPLACED");
    Console.WriteLine("\nПосле замены слов длины 5 в первом предложении:");
    Console.WriteLine(replacedText);

    Text filteredText = Parser.ParseText(source);
    filteredText.RemoveStopwords(new List<string>
        {
            "is", "am", "the", "this", "как"
        });
    Console.WriteLine("\nПосле удаления стоп-слов:");
    Console.WriteLine(filteredText);

    try
    {
        text.XMlExport("result.xml");
        Console.WriteLine("\nXML сохранён в файл:");
        Console.WriteLine(System.IO.Path.GetFullPath("result.xml"));
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\nОшибка XML-экспорта: {ex.Message}");
    }

    Console.WriteLine("\nНажми любую клавишу для выхода...");
    Console.ReadKey();
}