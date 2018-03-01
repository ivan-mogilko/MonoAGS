namespace AGS.API
{
    /// <summary>
    /// Interface for the arguments of an event that may be claimed.
    /// When event get claimed it is not passed to the remaining recepients.
    /// </summary>
    public interface IClaimableEventArgs
    {
        /// <summary>
        /// Get/set whether this event is claimed. This flag is used to notify the system
        /// that this event should not be passed to remaining subscribers.
        /// </summary>
        bool Claimed { get; set; }
    }
}
