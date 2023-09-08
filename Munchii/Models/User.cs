using System.Collections.Generic;
using Munchii.Models;

public class User
{
    public string Id { get; set; }
    public string Username { get; set; }
    public bool QuizSubmitted { get; set; }
    public QuizData QuizData { get; set; }
}

public class QuizData
{
    public List<Ranking> Rankings { get; set; }
    public List<RestaurantType> DealBreakers { get; set; }
}

public class Ranking
{
    public string Name { get; set; }
    public double Rating { get; set; }
}



