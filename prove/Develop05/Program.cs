using System;
using System.Threading;

class MindfulnessActivity
{
    // Private member variables with encapsulated access via properties
    private string _activityName;
    private string _description;
    private int _duration; // Duration in seconds

    // Public properties for encapsulation
    public string ActivityName
    {
        get { return _activityName; }
        set { _activityName = value; }
    }

    public string Description
    {
        get { return _description; }
        set { _description = value; }
    }

    public int Duration
    {
        get { return _duration; }
        set { _duration = value; }
    }

    // Method to start an activity
    public void StartActivity(string activityName, string description)
    {
        ActivityName = activityName;
        Description = description;
        Console.WriteLine($"Starting {ActivityName}...\n{Description}");
        Console.Write("Enter the duration in seconds: ");
        Duration = Convert.ToInt32(Console.ReadLine());
        Console.WriteLine("Prepare to begin...");
        Thread.Sleep(3000); // Pause for 3 seconds
    }

    // Method to end an activity
    public void EndActivity()
    {
        Console.WriteLine($"Good job! You have completed the {ActivityName} for {Duration} seconds.");
        Thread.Sleep(3000); // Pause for 3 seconds
    }

    // Spinner animation
    public void ShowSpinner(int durationInSeconds)
    {
        DateTime endTime = DateTime.Now.AddSeconds(durationInSeconds);
        while (DateTime.Now < endTime)
        {
            Console.Write("/");
            Thread.Sleep(100);
            Console.Write("\b \b");
            Console.Write("-");
            Thread.Sleep(100);
            Console.Write("\b \b");
            Console.Write("\\");
            Thread.Sleep(100);
            Console.Write("\b \b");
            Console.Write("|");
            Thread.Sleep(100);
            Console.Write("\b \b");
        }
    }

    // Countdown timer for more flexibility in activities
    public void ShowCountdown(int seconds)
    {
        for (int i = seconds; i > 0; i--)
        {
            Console.WriteLine(i);
            Thread.Sleep(10);
        }
    }
}

class BreathingActivity : MindfulnessActivity
{
    // Option for challenge mode
    private bool _isChallengeMode;

    // Constructor with challenge mode option
    public BreathingActivity(bool isChallengeMode = false)
    {
        _isChallengeMode = isChallengeMode;
    }

    // Breathing activity implementation
    public void StartBreathingActivity()
    {
        StartActivity("Breathing", "This activity will help you relax by focusing on your breathing. Clear your mind and breathe deeply.");

        if (_isChallengeMode)
        {
            Console.WriteLine("Challenge Mode Activated! The breathing intervals will change randomly.");
        }

        DateTime endTime = DateTime.Now.AddSeconds(Duration);
        Random random = new Random();

        while (DateTime.Now < endTime)
        {
            int breatheInDuration = _isChallengeMode ? random.Next(3, 7) : 4;
            int breatheOutDuration = _isChallengeMode ? random.Next(3, 7) : 4;

            Console.WriteLine("Breathe in...");
            ShowCountdown(breatheInDuration); // Dynamic timing for breathing in
            Console.WriteLine("Breathe out...");
            ShowCountdown(breatheOutDuration); // Dynamic timing for breathing out
        }

        EndActivity();
    }
}

class ReflectionActivity : MindfulnessActivity
{
    // List of reflection prompts and questions
    private static readonly string[] _prompts = {
        "Think of a time when you stood up for someone else.",
        "Think of a time when you did something really difficult.",
        "Think of a time when you helped someone in need.",
        "Think of a time when you did something truly selfless."
    };

    private static readonly string[] _questions = {
        "Why was this experience meaningful to you?",
        "Have you ever done anything like this before?",
        "How did you get started?",
        "How did you feel when it was complete?",
        "What made this time different than other times when you were not as successful?",
        "What is your favorite thing about this experience?",
        "What could you learn from this experience that applies to other situations?",
        "What did you learn about yourself through this experience?",
        "How can you keep this experience in mind in the future?"
    };

    // Reflection activity implementation
    public void StartReflectionActivity()
    {
        StartActivity("Reflection", "This activity helps you reflect on times when you showed strength and resilience.");

        Random random = new Random();
        Console.WriteLine(_prompts[random.Next(_prompts.Length)]);

        DateTime endTime = DateTime.Now.AddSeconds(Duration);
        int ratingSum = 0;
        int questionCount = 0;

        while (DateTime.Now < endTime)
        {
            Console.WriteLine(_questions[random.Next(_questions.Length)]);
            ShowSpinner(5); // Spinner to give time to reflect on each question
            Console.Write("On a scale of 1-5, how did reflecting on this question make you feel? ");
            int rating = Convert.ToInt32(Console.ReadLine());
            ratingSum += rating;
            questionCount++;
        }

        double averageRating = (double)ratingSum / questionCount;
        Console.WriteLine($"Your average reflection rating was: {averageRating:F1}");
        EndActivity();
    }
}

class ListingActivity : MindfulnessActivity
{
    // List of listing prompts
    private static readonly string[] _listingPrompts = {
        "Who are people that you appreciate?",
        "What are your personal strengths?",
        "Who are people that you have helped this week?",
        "When have you felt the Holy Ghost this month?",
        "Who are some of your personal heroes?"
    };

    // Listing activity implementation
    public void StartListingActivity()
    {
        StartActivity("Listing", "This activity will help you reflect on the good things in your life.");

        Random random = new Random();
        Console.WriteLine(_listingPrompts[random.Next(_listingPrompts.Length)]);
        Console.WriteLine("Start listing items...");

        int itemCount = 0;
        DateTime endTime = DateTime.Now.AddSeconds(Duration);
        while (DateTime.Now < endTime)
        {
            Console.Write("List an item: ");
            Console.ReadLine(); // User lists the item
            itemCount++;
        }

        Console.WriteLine($"You listed {itemCount} items!");
        EndActivity();
    }
}

class Program
{
    static void Main(string[] args)
    {
        // Create instances of each activity
        BreathingActivity breathing = new BreathingActivity();
        ReflectionActivity reflection = new ReflectionActivity();
        ListingActivity listing = new ListingActivity();

        bool running = true;

        // Main menu loop
        while (running)
        {
            Console.WriteLine("\nChoose an activity:");
            Console.WriteLine("1. Breathing");
            Console.WriteLine("2. Reflection");
            Console.WriteLine("3. Listing");
            Console.WriteLine("4. Exit");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    breathing.StartBreathingActivity();
                    break;
                case "2":
                    reflection.StartReflectionActivity();
                    break;
                case "3":
                    listing.StartListingActivity();
                    break;
                case "4":
                    running = false;
                    Console.WriteLine("Exiting the program.");
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please select a valid option.");
                    break;
            }
        }
    }
}
