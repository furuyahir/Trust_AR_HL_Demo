// A simple example to demonstrate using the TineySaveAPI.
//
// The important lines are:
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
    /// A component that demonstrates the simple use of TinySaveAPI.
    /// </summary>
    public class TinySaveTestSimple : MonoBehaviour
    {
        // ---------------------------------------------------------------------------- 

        public Text Result;

        private const string fileName = "Player.dat";
               
        // ---------------------------------------------------------------------------- 
        void Start ( )
        {
            Debug.Log ( $"Application.persistentDataPath and file : {Application.persistentDataPath}/{fileName}" );
        }


        /// <summary>
        /// Demonstrate saving a PlayerData object.
        /// </summary>
        public async void SaveData ( )
        {
            var i = 0;
            var playerData = new PlayerData ( )
            {
                Name = "Player1 Name",
                Health = 100,
                Lives = 3
            };

            var resultString =
                $"{++i}. Save Data Started.\n" +
                $"{++i}. Call TinySave.SaveAsync ( \"{fileName}\", {playerData.GetType ( ).Name})\n";

            // This is the only important line of code to save an object.
            var saveResult = await TinySave.SaveAsync ( fileName, playerData, SerializationType.Binary );

            resultString += $"{++i}. Returned Response : {saveResult}\n";
            Result.text = resultString;
        }



        /// <summary>
        /// Demonstrate loading a previously saved PlayerData object.
        /// </summary>
        public async void LoadData ( )
        {
            var i = 0;
            var resultString =
                $"{++i}. Load Object Started.\n" +
                $"{++i}. TinySave.LoadAsync<DummyData> ( \"{fileName}\" )\n";

            // This is the only important line of code to load an object.
            var loadResult = await TinySave.LoadAsync<PlayerData> ( fileName, SerializationType.Binary );

            // We can now se loadResult.response and loadResult.item.
            resultString += $"{++i}. Returned Response : {loadResult.response}\n";
            if ( loadResult.response.HasFlag ( Response.Success ) )
                resultString += $"{++i}. Loaded Data Name : \"{loadResult.item.Name}\"\n";

            Result.text = resultString;
        }
    }
}