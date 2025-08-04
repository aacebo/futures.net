namespace Futures;

/// <summary>
/// describes the state of a future
/// </summary>
public enum State
{
    /// <summary>
    /// the future has not started
    /// receiving data
    /// </summary>
    NotStarted,

    /// <summary>
    /// the future has received at
    /// least one call to Next
    /// </summary>
    Started,

    /// <summary>
    /// the future has completed
    /// successfully
    /// </summary>
    Success,

    /// <summary>
    /// the future has completed
    /// with an exception
    /// </summary>
    Error,

    /// <summary>
    /// the future has completed as
    /// a result of a cancellation
    /// </summary>
    Cancelled,
}