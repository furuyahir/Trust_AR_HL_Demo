using System;

namespace TinySaveAPI.Test
{
    // ---------------------------------------------------------------------------- 
    /// <summary>
    /// A sample class that we serialise, and then deserialise in our examples.
    /// </summary>
    /// <remarks>
    /// See https://docs.unity3d.com/Manual/script-Serialization.html for more information on Unity Serialization.
    /// See https://docs.microsoft.com/en-us/dotnet/standard/serialization/binary-serialization for more information on Binary Serialization.
    /// </remarks>
    [Serializable]
    public class PlayerData
    {
        public string Name;
        public int Lives;
        public int Health;
    }
}
