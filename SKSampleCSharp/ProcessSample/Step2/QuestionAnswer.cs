namespace ProcessLibSample.Step2;

public class QuestionAnswer(string question, string answer)
{
    private string _question = question;
    private string _answer = answer;

    public override string ToString()
    {
        return $"Question {_question} , Answer : {_answer}";
    }
}