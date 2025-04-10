namespace DDOOCP_Assignment.Models
{
    public enum ActivityType
    {
        Running,
        Swimming,
        Cycling
    }

    public class Activity
    {
        public string Name { get; set; }
        public int Duration { get; set; }

        public Activity(string name, int duration)
        {
            Name = name;
            Duration = duration;
        }

        public int CalculateCaloriesBurned()
        {
            return Duration * 5; // example
        }
    }
}
