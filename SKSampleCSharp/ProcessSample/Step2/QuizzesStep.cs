using Microsoft.SemanticKernel;

namespace ProcessLibSample.Step2;

public class QuizzesStep : KernelProcessStep<QuizzesState>
{
    private QuizzesState _state = new QuizzesState();

    public override ValueTask ActivateAsync(KernelProcessStepState<QuizzesState> state)
    {
        _state = state.State ?? new QuizzesState();

        var q1 = new QuestionAnswer("What is the capital of France?", "Paris");
        var q2 = new QuestionAnswer("What is the capital of Germany?", "Berlin");
        var q3 = new QuestionAnswer("What is the capital of Italy?", "Rome");

        _state.Questions.Add(q1.ToString());
        _state.Questions.Add(q2.ToString());
        _state.Questions.Add(q3.ToString());
        _state.Questions.Add("exit");

        return ValueTask.CompletedTask;
    }

    [KernelFunction]
    public async Task GetQuizzes(KernelProcessStepContext context)
    {
        Console.ForegroundColor = ConsoleColor.Green;

        var input = _state.Questions[_state.CurrentIdx];
        _state.CurrentIdx++;

        Console.WriteLine(input);

        //_state.QuizCount++;

        //var result = $"Quizzes are a great way to test your knowledge! : {_state.QuizCount}";

        //Console.WriteLine(result);

        //if (_state.QuizCount > 5)
        //{
        //    await context.EmitEventAsync(new KernelProcessEvent()
        //    {
        //        Id = StepEvent.StepEvents.Exit
        //    });
        //    return;
        //}


        if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
        {
            await context.EmitEventAsync(new KernelProcessEvent()
            {
                Id = StepEvent.StepEvents.Exit
            });
            return;
        }

        await context.EmitEventAsync(new KernelProcessEvent()
        {
            Id = StepEvent.StepEvents.GetQuizzes,
            Data = input

        });

    }
}