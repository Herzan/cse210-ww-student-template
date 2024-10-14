using System;
using System.Collections.Generic;

public class Comment
{
    public string CommenterName { get; set; }
    public string Text { get; set; }

    public Comment(string commenterName, string text)
    {
        CommenterName = commenterName;
        Text = text;
    }
}

public class Video
{
    public string Title { get; set; }
    public string Author { get; set; }
    public int LengthInSeconds { get; set; }
    private List<Comment> comments;

    public Video(string title, string author, int lengthInSeconds)
    {
        Title = title;
        Author = author;
        LengthInSeconds = lengthInSeconds;
        comments = new List<Comment>();
    }

    public void AddComment(Comment comment)
    {
        comments.Add(comment);
    }

    public int GetCommentCount()
    {
        return comments.Count;
    }

    public List<Comment> GetComments()
    {
        return comments;
    }
}

class Program
{
    static void Main(string[] args)
    {
        // Create a list to hold the videos
        List<Video> videos = new List<Video>();

        // Create 3-4 video objects
        Video video1 = new Video("Learn C# in 30 Minutes", "Alice", 1800);
        video1.AddComment(new Comment("Bob", "Great video! Very helpful."));
        video1.AddComment(new Comment("Charlie", "Thanks for the tips!"));
        video1.AddComment(new Comment("Diana", "Well explained."));

        Video video2 = new Video("Understanding OOP Concepts", "David", 2400);
        video2.AddComment(new Comment("Eva", "Loved the examples."));
        video2.AddComment(new Comment("Frank", "This cleared up a lot of confusion."));
        
        Video video3 = new Video("Advanced C# Techniques", "Grace", 3600);
        video3.AddComment(new Comment("Hank", "Can't wait to try these out."));
        video3.AddComment(new Comment("Ivy", "Informative content."));

        // Add videos to the list
        videos.Add(video1);
        videos.Add(video2);
        videos.Add(video3);

        // Iterate through the list of videos and display their details
        foreach (var video in videos)
        {
            Console.WriteLine($"Title: {video.Title}");
            Console.WriteLine($"Author: {video.Author}");
            Console.WriteLine($"Length: {video.LengthInSeconds} seconds");
            Console.WriteLine($"Number of Comments: {video.GetCommentCount()}");
            Console.WriteLine("Comments:");
            foreach (var comment in video.GetComments())
            {
                Console.WriteLine($"- {comment.CommenterName}: {comment.Text}");
            }
            Console.WriteLine(); // Blank line for readability
        }
    }
}
