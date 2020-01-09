using UnityEngine;

public class Timer
{
    private float duration;
    private float elapsedTime;

    /// <summary>
    /// Is the timer active.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Is the timer finished.
    /// </summary>
    public bool IsFinished { get; private set; }

    /// <summary>
    /// The timer's completion ratio, from 0 to 1.
    /// </summary>
    public float Ratio { get; private set; }

    /// <summary>
    /// Creates the timer.
    /// </summary>
    /// <param name="duration">The duration</param>
    public Timer(float duration)
    {
        Init(duration);
    }

    /// <summary>
    /// Initializes the timer.
    /// </summary>
    /// <param name="duration">The duration</param>
    public void Init(float duration)
    {
        if (duration > 0f)
        {
            this.duration = duration;
        }
        else
        {
            Debug.LogError("Timer duration must be more than 0.0 seconds.");
        }
    }

    /// <summary>
    /// Starts the timer.
    /// </summary>
    public void Start()
    {
        if (duration > 0f)
        {
            ResetTimer();
            IsActive = true;
        }
    }

    /// <summary>
    /// Updates the timer.
    /// If you intend to update and check the timer, call <see cref="Check()"/> instead.
    /// </summary>
    public void UpdateTimer()
    {
        if (IsActive)
        {
            elapsedTime += Time.deltaTime;
            Ratio = elapsedTime / duration;
        }
    }

    /// <summary>
    /// Checks the timer. Updates the time if the timer has not already finished.
    /// </summary>
    /// <returns>Has the timer finished</returns>
    public bool Check()
    {
        if (IsFinished)
        {
            return true;
        }
        else if (IsActive)
        {
            UpdateTimer();

            if (Ratio >= 1f)
            {
                Finish();
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Finishes the timer.
    /// </summary>
    public void Finish()
    {
        IsActive = false;
        IsFinished = true;
        Ratio = 1f;
    }

    /// <summary>
    /// Resets the timer.
    /// </summary>
    public void ResetTimer()
    {
        IsActive = false;
        IsFinished = false;
        elapsedTime = 0f;
        Ratio = 0f;
    }
}
