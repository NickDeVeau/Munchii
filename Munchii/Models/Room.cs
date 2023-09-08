using System.Collections.Generic;

public class Room
{
    public string RoomCode { get; set; }
    public Dictionary<string, User> Users { get; set; } = new Dictionary<string, User>();
    public long KeepAlive { get; set; }
    public bool QuizStarted { get; set; }
    public string HostId { get; set; }
}