using System;

namespace TinySaveAPI
{
    /// <summary>
    /// The enum returned by the API methods indicating the success or failuer of the operation.
    /// </summary>
    [Flags]
    public enum Response
    {
        None = 0,
        Success = 1 << 0,
        Failure = 1 << 2,

        Empty = Failure | 1 << 10,
        FileNotFound = Failure | 1 << 11,
        InvalidParameter = Failure | 1 << 12,
        Exception = Failure | 1 << 13,
        UnableToSerialise = Failure | 1 << 14,
        UnableToDeserialise = Failure | 1 << 15
    }
}