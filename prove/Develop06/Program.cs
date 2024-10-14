using System;
using System.Collections.Generic;
using System.IO;

// Base class for Goals
public abstract class Goal
{
    protected string _name;
    protected int _points;
    protected bool _isComplete;
    protected string _category; // New property for category

    protected Goal(string name, int points, string category)
    {
        _name = name;
        _points = points;
        _isComplete = false;
        _category = category;
    }

    public string Name => _name;
    public int Points => _points;
    public bool IsComplete => _isComplete;
    public string Category => _category; // New property

    public abstract void RecordEvent();
    public abstract void EditGoal(string newName, int newPoints, string newCategory); // Updated method

    public double Progress => _isComplete ? 100 : 0; // Progress percentage

    public override string ToString()
    {
        return $"{Name} - {Points} points [{Category}]" + (IsComplete ? " (Completed)" : $" (Progress: {Progress}%)");
    }
}

// Simple Goal
public class SimpleGoal : Goal
{
    public SimpleGoal(string name, int points, string category) : base(name, points, category) { }

    public override void RecordEvent()
    {
        _isComplete = true; // Mark as complete
        Console.WriteLine($"{Name} is marked as complete! You earned {Points} points.");
    }

    public override void EditGoal(string newName, int newPoints, string newCategory)
    {
        _name = newName;
        _points = newPoints;
        _category = newCategory; // Update category
    }
}

// Eternal Goal
public class EternalGoal : Goal
{
    public EternalGoal(string name, int points, string category) : base(name, points, category) { }

    public override void RecordEvent()
    {
        Console.WriteLine($"{Name} recorded! You earned {Points} points this time.");
    }

    public override void EditGoal(string newName, int newPoints, string newCategory)
    {
        _name = newName;
        _points = newPoints;
        _category = newCategory; // Update category
    }
}

// Checklist Goal
public class ChecklistGoal : Goal
{
    private int _timesCompleted;
    private int _requiredTimes;

    public ChecklistGoal(string name, int points, string category, int requiredTimes) : base(name, points, category)
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
                _isComplete = true; // Mark as complete
                Console.WriteLine($"{Name} is completed! You earned {Points} points.");
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

    public override void EditGoal(string newName, int newPoints, string newCategory)
    {
        _name = newName;
        _points = newPoints;
        _category = newCategory; // Update category
    }

    public string GetProgress()
    {
        return $"Completed {_timesCompleted}/{_requiredTimes} times.";
    }

    public double progress => (double)_timesCompleted / _requiredTimes * 100; // Progress percentage
}

// Goal Tracker Class
public class GoalTracker
{
    private List<Goal> _goals;
    private int _totalPoints;
    private List<string> _history; // New history for completed goals

    public GoalTracker()
    {
        _goals = new List<Goal>();
        _totalPoints = 0;
        _history = new List<string>(); // Initialize history
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
        _history.Add($"{DateTime.Now}: Completed '{_goals[index].Name}' and earned {_goals[index].Points} points."); // Add to history
        Console.WriteLine($"Total Points: {_totalPoints}");
    }

    public void EditGoal(int index)
    {
        if (index < 0 || index >= _goals.Count)
        {
            Console.WriteLine("Invalid goal index.");
            return;
        }

        Console.Write("Enter new name for the goal: ");
        string newName = Console.ReadLine();
        int newPoints = GetValidPoints("Enter new points for the goal: ");
        Console.Write("Enter new category for the goal: "); // New input for category
        string newCategory = Console.ReadLine();
        _goals[index].EditGoal(newName, newPoints, newCategory);
        Console.WriteLine("Goal updated successfully.");
    }

    public void DeleteGoal(int index)
    {
        if (index < 0 || index >= _goals.Count)
        {
            Console.WriteLine("Invalid goal index.");
            return;
        }

        Console.WriteLine($"{_goals[index].Name} has been deleted from your goals.");
        _goals.RemoveAt(index);
    }

    public void DisplayGoals()
    {
        Console.WriteLine("Your Goals:");
        for (int i = 0; i < _goals.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {_goals[i]}");
            if (_goals[i] is ChecklistGoal checklistGoal)
            {
                Console.WriteLine(checklistGoal.GetProgress());
            }
        }
    }

    public void DisplayHistory() // New method to display goal history
    {
        Console.WriteLine("Goal History:");
        foreach (var record in _history)
        {
            Console.WriteLine(record);
        }
    }

    public void SaveGoals(string filename)
    {
        try
        {
            using (StreamWriter outputFile = new StreamWriter(filename))
            {
                foreach (var goal in _goals)
                {
                    string progress = goal is ChecklistGoal checklist ? checklist.GetProgress() : "N/A";
                    outputFile.WriteLine($"{goal.GetType().Name},{goal.Name},{goal.Points},{goal.Category},{progress}");
                }
            }
            Console.WriteLine("Goals saved to file.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving goals: {ex.Message}");
        }
    }

    public void LoadGoals(string filename)
    {
        if (!File.Exists(filename))
        {
            Console.WriteLine("File not found.");
            return;
        }

        try
        {
            string[] lines = File.ReadAllLines(filename);
            foreach (var line in lines)
            {
                var parts = line.Split(",");
                if (parts.Length < 4)
                {
                    Console.WriteLine($"Invalid line format: {line}");
                    continue;
                }

                string type = parts[0];
                string name = parts[1];
                int points = int.Parse(parts[2]);
                string category = parts[3]; // Load category
                Goal goal;

                switch (type)
                {
                    case nameof(SimpleGoal):
                        goal = new SimpleGoal(name, points, category);
                        break;
                    case nameof(EternalGoal):
                        goal = new EternalGoal(name, points, category);
                        break;
                    case nameof(ChecklistGoal):
                        int requiredTimes = parts.Length > 4 ? int.Parse(parts[4]) : 1; // Default to 1 if not specified
                        goal = new ChecklistGoal(name, points, category, requiredTimes);
                        break;
                    default:
                        Console.WriteLine($"Unknown goal type: {type}");
                        continue;
                }
                AddGoal(goal);
            }
            Console.WriteLine("Goals loaded from file.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading goals: {ex.Message}");
        }
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
            Console.WriteLine("6. Display History"); // New option for displaying history
            Console.WriteLine("7. Save Goals");
            Console.WriteLine("8. Load Goals");
            Console.WriteLine("9. Exit");
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
                    DisplayHistory(); // Call to display history
                    break;
                case "7":
                    SaveGoalsPrompt();
                    break;
                case "8":
                    LoadGoalsPrompt();
                    break;
                case "9":
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
        Console.Write("Enter the category for the goal: "); // New input for category
        string category = Console.ReadLine();

        Console.WriteLine("Select Goal Type:");
        Console.WriteLine("1. Simple Goal");
        Console.WriteLine("2. Eternal Goal");
        Console.WriteLine("3. Checklist Goal");
        string goalType = Console.ReadLine();

        Goal goal;
        switch (goalType)
        {
            case "1":
                goal = new SimpleGoal(name, points, category);
                break;
            case "2":
                goal = new EternalGoal(name, points, category);
                break;
            case "3":
                Console.Write("Enter the number of times this goal must be completed: ");
                int requiredTimes = GetValidPoints("Enter the required times: ");
                goal = new ChecklistGoal(name, points, category, requiredTimes);
                break;
            default:
                Console.WriteLine("Invalid goal type. Goal not added.");
                return;
        }

        AddGoal(goal);
    }

    private void RecordGoalEventPrompt()
    {
        DisplayGoals();
        Console.Write("Enter the index of the goal to record an event: ");
        int index = int.Parse(Console.ReadLine()) - 1;
        RecordGoalEvent(index);
    }

    private void EditGoalPrompt()
    {
        DisplayGoals();
        Console.Write("Enter the index of the goal to edit: ");
        int index = int.Parse(Console.ReadLine()) - 1;
        EditGoal(index);
    }

    private void DeleteGoalPrompt()
    {
        DisplayGoals();
        Console.Write("Enter the index of the goal to delete: ");
        int index = int.Parse(Console.ReadLine()) - 1;
        DeleteGoal(index);
    }

    private void SaveGoalsPrompt()
    {
        Console.Write("Enter filename to save goals: ");
        string filename = Console.ReadLine();
        SaveGoals(filename);
    }

    private void LoadGoalsPrompt()
    {
        Console.Write("Enter filename to load goals: ");
        string filename = Console.ReadLine();
        LoadGoals(filename);
    }

    private int GetValidPoints(string prompt) // Helper method for input validation
    {
        int points;
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out points) && points >= 0)
            {
                break;
            }
            Console.WriteLine("Please enter a valid non-negative number for points.");
        }
        return points;
    }
}

// Main Program
class Program
{
    static void Main(string[] args)
    {
        GoalTracker tracker = new GoalTracker();
        tracker.DisplayMenu();
    }
}
