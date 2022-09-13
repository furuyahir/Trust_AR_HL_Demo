using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public class TestDataClassAdvanced
    {
        public int TestInteger = 111;
        public float TestFloat = 222.2f;
        public string TestString = "TestDataClass String";

        // Properties can be serialised if using SerializationType.Binary. JsonUtility unfortunately ignores properties.
        public string TestStringProperty { get; set; }

        //JsonUtility unfortunately ignores Dictionaries.
        public Dictionary<string, string> dictionary = new Dictionary<string, string>
        {
            [ "a" ] = "aaa",
            [ "b" ] = "bbb"
        };

        // Both Binary and JsonUtility serialisation works with List<T>.
        public List<string> TestList = new List<string> { "a", "b", "c" };
    }
}
