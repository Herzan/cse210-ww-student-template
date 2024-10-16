using System;
using System.Collections.Generic;

public abstract class Activity
{
    private string _date;
    private int _minutes;

    // Constructor for initializing common attributes
    public Activity(string date, int minutes)
    {
        _date = date;
        _minutes = minutes;
    }

    // Getter for the date
    public string GetDate()
    {
        return _date;
    }

    // Getter for minutes
    public int GetMinutes()
    {
        return _minutes;
    }

    // Abstract methods for distance, speed, and pace
    public abstract double GetDistance(); 
    public abstract double GetSpeed();
    public abstract double GetPace();

    // General summary method, does not need to be overridden in derived classes
    public virtual string GetSummary()
    {
        return $"{_date}: {this.GetType().Name} ({_minutes} min) - Distance {GetDistance():0.0} miles, Speed {GetSpeed():0.0} mph, Pace {GetPace():0.0} min per mile";
    }
}

public class Running : Activity
{
    private double _distance; // in miles

    // Constructor initializing the unique attribute
    public Running(string date, int minutes, double distance) : base(date, minutes)
    {
        _distance = distance;
    }

    // Overriding abstract methods from the base class
    public override double GetDistance()
    {
        return _distance;
    }

    public override double GetSpeed()
    {
        return (_distance / GetMinutes()) * 60; // speed in mph
    }

    public override double GetPace()
    {
        return GetMinutes() / _distance; // pace in minutes per mile
    }
}

public class Cycling : Activity
{
    private double _speed; // in mph

    // Constructor initializing the unique attribute
    public Cycling(string date, int minutes, double speed) : base(date, minutes)
    {
        _speed = speed;
    }

    // Overriding abstract methods from the base class
    public override double GetDistance()
    {
        return (_speed * GetMinutes()) / 60; // distance in miles
    }

    public override double GetSpeed()
    {
        return _speed;
    }

    public override double GetPace()
    {
        return 60 / _speed; // pace in minutes per mile
    }
}

public class Swimming : Activity
{
    private int _laps; // in number of laps

    // Constructor initializing the unique attribute
    public Swimming(string date, int minutes, int laps) : base(date, minutes)
    {
        _laps = laps;
    }

    // Overriding abstract methods from the base class
    public override double GetDistance()
    {
        return _laps * 50 / 1000.0 * 0.62; // distance in miles
    }

    public override double GetSpeed()
    {
        return (GetDistance() / GetMinutes()) * 60; // speed in mph
    }

    public override double GetPace()
    {
        return GetMinutes() / GetDistance(); // pace in minutes per mile
    }
}

public class Program
{
    public static void Main()
    {
        // Creating instances of each type of activity
        List<Activity> activities = new List<Activity>
        {
            new Running("03 Nov 2022", 30, 3.0),
            new Cycling("03 Nov 2022", 30, 12.0),
            new Swimming("03 Nov 2022", 30, 20)
        };

        // Iterating through the list and printing summaries
        foreach (Activity activity in activities)
        {
            Console.WriteLine(activity.GetSummary());
        }
    }
}
