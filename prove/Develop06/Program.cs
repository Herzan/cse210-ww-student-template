using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

// Enum for Priority Levels
public enum Priority
{
    High,
    Medium,
    Low
}

// Base class for Goals
[JsonConverter(typeof(GoalConverter))]
public abstract class Goal
{
    protected string _name;
    protected int _points;
    protected bool _isComplete;
    protected string _category;
    protected Priority _priority; // New property for goal priority

    protected Goal(string name, int points, string category, Priority priority)
    {
        _name = name;
        _points = points;
        _isComplete = false;
        _category = category;
        _priority = priority;
    }

    public string Name => _name;
    public int Points => _points;
    public bool IsComplete => _isComplete;
    public string Category => _category;
    public Priority GoalPriority => _priority; // New property

    public abstract void RecordEvent();
    public abstract void EditGoal(string newName, int newPoints, string newCategory, Priority newPriority); // Updated method

    public double Progress => _isComplete ? 100 : 0; // Progress percentage

    public override string ToString()
    {
        return $"{Name} - {Points} points [{Category}] - Priority: {GoalPriority}" +
               (IsComplete ? " (Completed)" : $" (Progress: {Progress}%)");
    }

    // Helper method for displaying a visual progress bar
    public string DisplayProgressBar()
    {
        int completedBars = (int)(Progress / 10);
        return $"[{new string('#', completedBars)}{new string('-', 10 - completedBars)}]";
    }
}

// Simple Goal
public class SimpleGoal : Goal
{
    public SimpleGoal(string name, int points, string category, Priority priority) : base(name, points, category, priority) { }

    public override void RecordEvent()
    {
        _isComplete = true;
        Console.WriteLine($"{Name} is marked as complete! You earned {Points} points.");
        Console.WriteLine("🎉 Congratulations! Keep up the good work!");
    }

    public override void EditGoal(string newName, int newPoints, string newCategory, Priority newPriority)
    {
        _name = newName;
        _points = newPoints;
        _category = newCategory;
        _priority = newPriority;
    }
}

// Eternal Goal
public class EternalGoal : Goal
{
    public EternalGoal(string name, int points, string category, Priority priority) : base(name, points, category, priority) { }

    public override void RecordEvent()
    {
        Console.WriteLine($"{Name} recorded! You earned {Points} points this time.");
    }

    public override void EditGoal(string newName, int newPoints, string newCategory, Priority newPriority)
    {
        _name = newName;
        _points = newPoints;
        _category = newCategory;
        _priority = newPriority;
    }
}

// Checklist Goal
public class ChecklistGoal : Goal
{
    private int _timesCompleted;
    private int _requiredTimes;

    public ChecklistGoal(string name, int points, string category, Priority priority, int requiredTimes)
        : base(name, points, category, priority)
    {
        _requiredTimes = requiredTimes;
        _timesCompleted = 0;
    }

    public override void RecordEvent()
    {
        if (_timesCompleted < _requiredTimes)
        {
            _timesCompleted++;
            if (_timesCompleted == _requiredTimes)
            {
                _isComplete = true;
                Console.WriteLine($"{Name} is completed! You earned {Points} points.");
                Console.WriteLine("✨ Milestone achieved! You finished the entire checklist!");
            }
            else if (_timesCompleted == _requiredTimes / 2)
            {
                Console.WriteLine($"{Name} recorded! Halfway there! Keep it up!");
            }
            else
            {
                Console.WriteLine($"{Name} recorded! You earned {Points} points.");
            }
        }
        else
        {
            Console.WriteLine($"{Name} is already completed!");
        }
    }

    public override void EditGoal(string newName, int newPoints, string newCategory, Priority newPriority)
    {
        _name = newName;
        _points = newPoints;
        _category = newCategory;
        _priority = newPriority;
    }

    public string GetProgress()
    {
        return $"Completed {_timesCompleted}/{_requiredTimes} times.";
    }

    public double progress => (double)_timesCompleted / _requiredTimes * 100; // Progress percentage
}

// JsonConverter implementation for handling polymorphism
public class GoalConverter : JsonConverter<Goal>
{
    public override Goal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
        {
            var root = doc.RootElement;
            string type = root.GetProperty("Type").GetString();

            switch (type)
            {
                case "SimpleGoal":
                    return JsonSerializer.Deserialize<SimpleGoal>(root.GetRawText(), options);
                case "EternalGoal":
                    return JsonSerializer.Deserialize<EternalGoal>(root.GetRawText(), options);
                case "ChecklistGoal":
                    return JsonSerializer.Deserialize<ChecklistGoal>(root.GetRawText(), options);
                default:
                    throw new NotSupportedException($"Goal type '{type}' is not supported");
            }
        }
    }

    public override void Write(Utf8JsonWriter writer, Goal value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("Type", value.GetType().Name);
        JsonSerializer.Serialize(writer, (object)value, value.GetType(), options);
        writer.WriteEndObject();
    }
}

// Goal Tracker Class
public class GoalTracker
{
    private List<Goal> _goals;
    private int _totalPoints;
    private List<string> _history;

    public GoalTracker()
    {
        _goals = new List<Goal>();
        _totalPoints = 0;
        _history = new List<string>();
    }

    public void AddGoal(Goal goal)
    {
        _goals.Add(goal);
        Console.WriteLine($"{goal.Name} has been added to your goals.");
    }

    public void RecordGoalEvent(int index)
    {
        if (index < 0 || index >= _goals.Count)
        {
            Console.WriteLine("Invalid goal index.");
            return;
        }

        _goals[index].RecordEvent();
        _totalPoints += _goals[index].Points;
        _history.Add($"{DateTime.Now}: Completed '{_goals[index].Name}' and earned {_goals[index].Points} points.");
        Console.WriteLine($"Total Points: {_totalPoints}");
    }

    public void DisplayGoals()
    {
        Console.WriteLine("Your Goals (Grouped by Category):");
        foreach (var categoryGroup in _goals.GroupBy(g => g.Category))
        {
            Console.WriteLine($"\nCategory: {categoryGroup.Key}");
            foreach (var goal in categoryGroup)
            {
                Console.WriteLine($"{goal} | {goal.DisplayProgressBar()}");
            }
        }
    }

    public void SaveToFile(string fileName)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(this, options);
        File.WriteAllText(fileName, json);
        Console.WriteLine("Goals successfully saved to file.");
    }

    public static GoalTracker LoadFromFile(string fileName)
    {
        if (!File.Exists(fileName))
        {
            Console.WriteLine("File not found.");
            return null;
        }

        string json = File.ReadAllText(fileName);
        GoalTracker tracker = JsonSerializer.Deserialize<GoalTracker>(json);
        Console.WriteLine("Goals successfully loaded from file.");
        return tracker;
    }

    public void DisplayMenu()
    {
        while (true)
        {
            Console.WriteLine("\nMenu:");
            Console.WriteLine("1. Add Goal");
            Console.WriteLine("2. Record Goal Event");
            Console.WriteLine("3. Edit Goal");
            Console.WriteLine("4. Delete Goal");
            Console.WriteLine("5. Display Goals");
            Console.WriteLine("6. Save Goals");
            Console.WriteLine("7. Load Goals");
            Console.WriteLine("8. Exit");
            Console.Write("Select an option: ");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    AddGoalPrompt();
                    break;
                case "2":
                    RecordGoalEventPrompt();
                    break;
                case "3":
                    EditGoalPrompt();
                    break;
                case "4":
                    DeleteGoalPrompt();
                    break;
                case "5":
                    DisplayGoals();
                    break;
                case "6":
                    SaveToFile("goals.json");
                    break;
                case "7":
                    LoadFromFile("goals.json");
                    break;
                case "8":
                    Console.WriteLine("Exiting program.");
                    return;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    private void AddGoalPrompt()
    {
        Console.Write("Enter the name of the goal: ");
        string name = Console.ReadLine();
        int points = GetValidPoints("Enter the points for the goal: ");
        Console.Write("Enter the category for the goal: ");
        string category = Console.ReadLine();
        Console.WriteLine("Enter priority (High/Medium/Low): ");
        Priority priority = (Priority)Enum.Parse(typeof(Priority), Console.ReadLine(), true);

        Console.WriteLine("Choose goal type (1: Simple, 2: Eternal, 3: Checklist): ");
        string typeChoice = Console.ReadLine();
        switch (typeChoice)
        {
            case "1":
                AddGoal(new SimpleGoal(name, points, category, priority));
                break;
            case "2":
                AddGoal(new EternalGoal(name, points, category, priority));
                break;
            case "3":
                Console.Write("Enter the required number of times to complete the checklist: ");
                int requiredTimes = int.Parse(Console.ReadLine());
                AddGoal(new ChecklistGoal(name, points, category, priority, requiredTimes));
                break;
            default:
                Console.WriteLine("Invalid goal type selected.");
                break;
        }
    }

    private int GetValidPoints(string prompt)
    {
        int points;
        Console.Write(prompt);
        while (!int.TryParse(Console.ReadLine(), out points) || points < 0)
        {
            Console.Write("Invalid points. Please enter a positive integer: ");
        }
        return points;
    }

    private void RecordGoalEventPrompt()
    {
        DisplayGoals();
        Console.Write("Enter the goal index to record an event: ");
        int index = int.Parse(Console.ReadLine());
        RecordGoalEvent(index - 1);
    }

    private void EditGoalPrompt()
    {
        DisplayGoals();
        Console.Write("Enter the goal index to edit: ");
        int index = int.Parse(Console.ReadLine());
        if (index < 1 || index > _goals.Count)
        {
            Console.WriteLine("Invalid index. Please try again.");
            return;
        }

        Goal goal = _goals[index - 1];
        Console.Write($"Enter new name for '{goal.Name}' (or press Enter to keep it unchanged): ");
        string newName = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(newName)) newName = goal.Name;

        int newPoints = GetValidPoints($"Enter new points for '{goal.Name}' (or press Enter to keep {goal.Points}): ");
        if (newPoints == 0) newPoints = goal.Points;

        Console.Write($"Enter new category for '{goal.Name}' (or press Enter to keep {goal.Category}): ");
        string newCategory = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(newCategory)) newCategory = goal.Category;

        Console.WriteLine($"Enter new priority (High/Medium/Low) (or press Enter to keep {goal.GoalPriority}): ");
        Priority newPriority;
        if (!Enum.TryParse(Console.ReadLine(), true, out newPriority))
        {
            newPriority = goal.GoalPriority;
        }

        goal.EditGoal(newName, newPoints, newCategory, newPriority);
        Console.WriteLine("Goal updated successfully.");
    }

    private void DeleteGoalPrompt()
    {
        DisplayGoals();
        Console.Write("Enter the goal index to delete: ");
        int index = int.Parse(Console.ReadLine());
        if (index < 1 || index > _goals.Count)
        {
            Console.WriteLine("Invalid index. Please try again.");
            return;
        }
        _goals.RemoveAt(index - 1);
        Console.WriteLine("Goal deleted successfully.");
    }
}

// Main Program
class Program
{
    static void Main(string[] args)
    {
        GoalTracker tracker = null;

        Console.WriteLine("Do you want to load goals from a file? (yes/no)");
        if (Console.ReadLine().ToLower() == "yes")
        {
            tracker = GoalTracker.LoadFromFile("goals.json");
            if (tracker == null)
            {
                tracker = new GoalTracker();
            }
        }
        else
        {
            tracker = new GoalTracker();
        }

        tracker.DisplayMenu();

        Console.WriteLine("Do you want to save your goals to a file? (yes/no)");
        if (Console.ReadLine().ToLower() == "yes")
        {
            tracker.SaveToFile("goals.json");
        }
    }
}
