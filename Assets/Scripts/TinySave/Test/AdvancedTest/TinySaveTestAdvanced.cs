// An advacned example to demonstrate using the TineySaveAPI.
//
// Even though there's quite a lot of code below, for the most part, it boilds down to
// these two lines:
//
// var saveResult = await TinySave.SaveAsync ( dataName, d, SerializationType.Binary );
// and
// var loadResult = await TinySave.LoadAsync<T> ( dataName, SerializationType.Binary );
//


using System;
using System.Collections.Generic;
using UnityEngine;

// Include TinySaveAPI se we can use the api methods.
using TinySaveAPI;
using UnityEngine.UI;

namespace TinySaveAPI.Test
{
    /// <summary>
    /// A component that has a few methods demonstrating the use of TinySaveAPI.
    /// </summary>
    public class TinySaveTestAdvanced : MonoBehaviour
    {
        // ---------------------------------------------------------------------------- 

        public Text Result;

        // A prvate string marked with the attrite, demonstrating the ability to (de)serialise MonoBehaviours.
        [SerializeField] private string TestString = "TinySaveTester MonoBehaviour String";


        private const string dataName = "Dummy Data";
        private string key;


        // ---------------------------------------------------------------------------- 
        void Start ( )
        {
            key = ( ( uint ) dataName.GetHashCode ( ) ).ToString ( );
            Debug.Log ( $"Application.persistentDataPath and file : {Application.persistentDataPath}/{key}.dat\n(if saving to file)" );
        }


        /// <summary>
        /// Demonstrate saving a general class object as Json.
        /// </summary>
        public async void SaveNonUnityObject ( )
        {
            var i = 0;
            var d = new TestDataClassAdvanced ( ) { TestString = "String Data Before Save.", TestStringProperty = "123" };

            var resultString =
                $"{++i}. Save Non-Unity Object Started.\n" +
                $"{++i}. Create New DummyData Object.\n    DummyData.TestString = \"{d.TestString}\"\n" +
                $"{++i}. Call TinySave.SaveAsync ( \"{dataName}\", {d.GetType ( ).Name})\n";

            // This is the only important line of code to save an object.
            var saveResult = await TinySave.SaveAsync ( dataName, d, SerializationType.Json );

            resultString += $"{++i}. Returned Response : {saveResult}\n";

            // Check the returned result of the SaveAsync call using a specific Enum.
            if ( saveResult != Response.Success )
            {
                Result.text = resultString;
                return;
            }
            d.TestString = "String Data AFTER Save.";
            d.TestStringProperty = "456";
            resultString += $"{++i}. Modify DummyData.TestString = \"{d.TestString}\"\n";
            Result.text = resultString;
        }



        /// <summary>
        /// Demonstrate loading a previously saved general class object from Json.
        /// </summary>
        public async void LoadNonUnityObject ( )
        {
            var i = 0;
            var resultString =
                $"{++i}. Load Non-Unity Object Started.\n" +
                $"{++i}. TinySave.LoadAsync<DummyData> ( \"{dataName}\" )\n";

            // This is the only important line of code to load an object.
            (Response response, TestDataClassAdvanced testData) loadResult = await TinySave.LoadAsync<TestDataClassAdvanced> ( dataName, SerializationType.Json );

            resultString += $"{++i}. Returned Response : {loadResult.response}\n";

            // Check if the saveResult is a specific enum.
            // Check for failure using Enum.HasFlag.
            if ( loadResult.response.HasFlag ( Response.Failure ) )
            {
                Result.text = resultString;
                return;
            }

            resultString += $"{++i}. Loaded Object String : \"{loadResult.testData.TestString}\"\n";
            Result.text = resultString;
        }

        /// <summary>
        /// Demonstrate saving a dictionary, modifying the local dicitionary item, then reloading that dictionary data again.
        /// </summary>
        public async void DictionaryExample ( )
        {
            var i = 0;
            Dictionary<string, string> dictionary = new Dictionary<string, string>
            {
                [ "a" ] = "aaa",
                [ "b" ] = "bbb"
            };

            var resultString =
                $"{++i}. Save Dictionary<string, string> Started.\n" +
                $"{++i}. Two dictionary items. dictionary[\"a\"] = {dictionary [ "a" ]}. dictionary[\"b\"] = {dictionary [ "b" ]}\n" +
                $"{++i}. Call TinySave.SaveAsync ( \"{dataName}\", dictionary, SerializationType.Binary)\n";

            // This is the only important line of code to save an object.
            var saveResult = await TinySave.SaveAsync ( dataName, dictionary, SerializationType.Binary );

            resultString += $"{++i}. Returned Response : {saveResult}\n";

            // Check the returned result of the SaveAsync call using Enum.HasFlag.
            if ( !saveResult.HasFlag ( Response.Success ) )
            {
                Result.text = resultString;
                return;
            }

            dictionary [ "a" ] = "xxx";
            dictionary [ "b" ] = "yyy";
            resultString += $"{++i}. Modify dictionary items. dictionary[\"a\"] = {dictionary [ "a" ]}. dictionary[\"b\"] = {dictionary [ "b" ]}\n";

            resultString += $"{++i}. Load Dictionary<string, string> Started.\n" +
                $"{++i}. Call TinySave.LoadAsync<Dictionary<string, string>> ( \"{dataName}\", SerializationType.Binary )\n";

            // This is the only important line of code to load an object.
            var load = await TinySave.LoadAsync<Dictionary<string, string>> ( dataName, SerializationType.Binary );

            resultString += $"{++i}. Returned Response : {load.Item1}\n";

            // Check the returned result of the LoadAsync call using Enum.HasFlag.
            if ( !load.Item1.HasFlag ( Response.Success ) )
            {
                Result.text = resultString;
                return;
            }

            dictionary = load.Item2;
            if ( load.Item2 == null )
                resultString += $"{++i}. Item returned is null!\n";
            else
                resultString += $"{++i}. Reread dictionary items. dictionary[\"a\"] = {dictionary [ "a" ]}. dictionary[\"b\"] = {dictionary [ "b" ]}";
            Result.text = resultString;
        }



        /// <summary>
        /// Demonstrate saving a MonoBehaviour object as Json.
        /// </summary>
        public async void SaveMonoBehaviour ( )
        {
            var i = 0;
            var resultString =
                $"{++i}. Save MonoBehaviour Started.\n" +
                $"{++i}. Using 'this' ({GetType ( ).Name}) Object.\n    {GetType ( ).Name}.TestString = \"{TestString}\"\n" +
                $"{++i}. Call TinySave.SaveAsync ( \"{dataName}\", {GetType ( ).Name}, SerializationType.Json ))\n";

            // This is the only important line of code to save a MonoBehaviour (or ScriptableObject).
            var saveResult = await TinySave.SaveAsync ( dataName, this, SerializationType.JsonEncrypted );

            resultString += $"{++i}. Returned Response : {saveResult}\n";

            // Use bitwise arithmetic to check for failure.
            if ( ( saveResult & Response.Failure ) == Response.Failure )
            {
                Result.text = resultString;
                return;
            }

            TestString = "String Data AFTER Save.";
            resultString += $"{++i}. Modify {GetType ( ).Name}.TestString = \"{TestString}\"\n";
            Result.text = resultString;
        }


        /// <summary>
        /// Demonstrate filling an instance of a MonoBehaviour object with data read as Json.
        /// </summary>
        public async void LoadMonoBehaviour ( )
        {
            var i = 0;
            var resultString =
                $"{++i}. Load MonoBehaviour Started.\n" +
                $"{++i}. Using 'this' Object.\n    {GetType ( ).Name}.TestString = \"{TestString}\"\n" +
                $"{++i}. Call TinySave.LoadAsync ( \"{dataName}\", {GetType ( ).Name}, SerializationType.Json ))\n";

            var loadResult = await TinySave.LoadAsync<TinySaveTestAdvanced> ( dataName, this, SerializationType.JsonEncrypted );

            resultString += $"{++i}. Returned Response : {loadResult}\n";

            // Use bitwise arithmetic to check for failure.
            if ( ( loadResult & Response.Failure ) == Response.Failure )
            {
                Result.text = resultString;
                return;
            }

            resultString += $"{++i}. Reloaded String \"{TestString}\".\n";
            Result.text = resultString;
        }


        private void OnDestroy ( )
        {
            CleanPlayerPrefs ( key );
        }


        /// <summary>
        /// Clear any PlayerPrefs that might have been created during this demonstration.
        /// </summary>
        void CleanPlayerPrefs ( string key )
        {
            // Clean up the PlayerPrefs item if found
            if ( PlayerPrefs.HasKey ( key ) )
            {
                Debug.Log ( $"Deleting PlayerPrefs {key}. {PlayerPrefs.GetString ( key )}" );
                PlayerPrefs.DeleteKey ( key );
            }
        }
    }
}