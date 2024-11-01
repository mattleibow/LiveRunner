using Microsoft.Maui.Animations;

namespace LiveRunnerApp.GameComponents;

public class AnimatedValue<T>
    where T : notnull
{
    private readonly Lerp.LerpDelegate _lerp;

    private T _desired;
    private double _progress;

    public AnimatedValue(T value)
    {
        var lerp = Lerp.GetLerp(typeof(T))?.Calculate;
        if (lerp == null)
            throw new InvalidOperationException($"No lerp found for type {typeof(T)}");
        _lerp = lerp;

        Initial = value;
        Current = value;
        _desired = value;
        _progress = 1.0;
    }

    public double Progress
    {
        get => _progress;
        private set
        {
            _progress = Math.Clamp(value, 0, 1);
            if (_progress >= 1)
            {
                Initial = Desired;
                Current = Desired;
            }
            else
            {
                var prog = Easing.Ease(Progress);
                Current = (T)_lerp(Initial, Desired, prog);
            }
        }
    }

    public T Initial { get; set; }

    public T Current { get; private set; }

    public T Desired
    {
        get => _desired;
        set
        {
            _desired = value;
            Progress = 0;
        }
    }

    public double Duration { get; set; } = 1.0;

    public Easing Easing { get; set; } = Easing.Linear;

    public void Update(double deltaTime)
    {
        if (Progress >= 1)
            return;

        var laneProgress = deltaTime / Duration;
        Progress += laneProgress;
    }
}