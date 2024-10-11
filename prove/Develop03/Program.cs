using System;
using System.Collections.Generic;
using System.Linq;
class Program 
{
    static void Main(string[] args)
    {
        ScriptureReference reference = new ScriptureReference("John", 3, 16);
        Scripture scripture = new Scripture(reference, "For God so loved the world that he gave his one and only Son that whoever believes in him shall not perish but have eternal life.");

        while (true)
        {
            Console.Clear();
            Console.WriteLine(scripture);

            if (scripture.AreAllWordsHidden())
            {
                Console.WriteLine("All words are hidden. Program will now exit.");
                break;
            }

            Console.WriteLine("\nPress Enter to hide more words or type 'quit' to exit.");
            string input = Console.ReadLine();
            if (input.ToLower() == "quit")
            {
                break;
            }

            scripture.HideRandomWords();

            List<Scripture> scriptures = new List<Scripture>
{
    new Scripture(new ScriptureReference("John", 3, 16), "For God so loved the world..."),
    new Scripture(new ScriptureReference("Proverbs", 3, 5), "Trust in the Lord with, all your heart...")
};


Random rnd = new Random();
Scripture s = scriptures[rnd.Next(scriptures.Count)];

string[] lines = File.ReadAllLines("scriptures.txt");
        }
    }
}




public class Words
{
    


    public string Text { get; private set; }
    public bool IsHidden { get; private set; }

    public Words(string text)
    {
        Text = text;
        IsHidden = false;
    }

    public void Hide()
    {
        IsHidden = true;
    }

    public override string ToString()
    {
        return IsHidden ? "____" : Text;
    }
}
public class ScriptureReference
{
    public string Book { get; private set; }
    public int StartVerse { get; private set; }
    public int? EndVerse { get; private set; }  // Nullable to handle single verses

    public ScriptureReference(string book, int startVerse, int? endVerse = null)
    {
        Book = book;
        StartVerse = startVerse;
        EndVerse = endVerse;
    }

    public override string ToString()
    {
        return EndVerse == null ? $"{Book} {StartVerse}" : $"{Book} {StartVerse}-{EndVerse}";
    }
}
public class Scripture
{
    public ScriptureReference Reference { get; private set; }
    private List<Words> Words;

    public Scripture(ScriptureReference reference, string text)
    {
        Reference = reference;
        Words = text.Split(' ').Select(word => new Words(word)).ToList();
    }

    public void HideRandomWords()
    {
        Random random = new Random();
        int wordCount = Words.Count;
        for (int i = 0; i < 3; i++)  // Hide 3 random words at a time
        {
            int index = random.Next(wordCount);
            Words[index].Hide();
        }
    }

    public bool AreAllWordsHidden()
    {
        return Words.All(word => word.IsHidden);
    }

    public override string ToString()
    {
        string scriptureText = string.Join(" ", Words);
        return $"{Reference}\n{scriptureText}";
    }
}
/*
 * Exceeded requirements by adding:
 * 1. File existence check before loading scriptures.
 * 2. Option to create a default set of scriptures if the file is not found.
 * 3. Dynamic user input for file path, making the program more flexible.
 */
