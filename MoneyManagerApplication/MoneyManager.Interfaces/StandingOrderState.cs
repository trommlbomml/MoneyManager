namespace MoneyManager.Interfaces
{
    /// <summary>
    /// Zustand des Dauerauftrages.
    /// </summary>
    public enum StandingOrderState
    {
        /// <summary>
        /// Dauerauftrag liegt noch in der Zukunft und ist nicht aktiv.
        /// </summary>
        InActive,

        /// <summary>
        /// Dauerauftrag läuft derzeit und ist noch nicht abgeschlossen.
        /// </summary>
        Active,

        /// <summary>
        /// Dauerauftrag abgeschlossen.
        /// </summary>
        Finished
    }
}