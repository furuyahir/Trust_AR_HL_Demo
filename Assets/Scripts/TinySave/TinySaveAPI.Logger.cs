namespace TinySaveAPI
{
    using System.Diagnostics;

    static class Logger
    {
        [Conditional ( "UNITY_EDITOR" )]
        public static void Log ( string logMsg )
        {
            UnityEngine.Debug.Log ( logMsg );
        }

        [Conditional ( "UNITY_EDITOR" )]
        public static void LogWarning ( string logMsg )
        {
            UnityEngine.Debug.LogWarning ( logMsg );
        }
    }
}