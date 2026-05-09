namespace Fighter;

/// <summary>
/// Per-turn state tracking for card keyword triggers.
/// Reset via TurnEnd patches or explicit reset calls.
/// </summary>
public static class TurnState
{
    /// <summary>
    /// Whether a card with the [起手] (Starter) keyword was played this turn.
    /// </summary>
    public static bool StarterPlayedThisTurn { get; set; }

    /// <summary>
    /// Whether a card with the [投技] (Throw) keyword was played this turn.
    /// </summary>
    public static bool ThrowPlayedThisTurn { get; set; }

    public static void Reset()
    {
        StarterPlayedThisTurn = false;
        ThrowPlayedThisTurn = false;
    }
}
