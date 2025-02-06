public static class ChaseManager
{
    #region Variables
    public static bool canChase = true;
    #endregion

    #region Properties
    public static bool IsChasing { get; private set; }
    #endregion

    #region Events
    public delegate void OnChaseStart();
    public static OnChaseStart OnChaseStartEvent;
    public delegate void OnChaseEnd();
    public static OnChaseEnd OnChaseEndEvent;
    #endregion

    #region Code
    public static void StartChase()
    {
        if (IsChasing || !canChase)
            return;

        IsChasing = true;

        OnChaseStartEvent?.Invoke();
    }

    public static void EndChase()
    {
        if (!IsChasing || !canChase)
            return;

        IsChasing = false;

        OnChaseEndEvent?.Invoke();
    }
    #endregion
}