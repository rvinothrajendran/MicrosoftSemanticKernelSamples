namespace ProcessLibSample.Step2;

public class QuizzesState
{
    public List<string> Questions { get; set; } = new List<string>();

    public int CurrentIdx { get; set; } = 0;
    public int QuizCount { get; set; }
}