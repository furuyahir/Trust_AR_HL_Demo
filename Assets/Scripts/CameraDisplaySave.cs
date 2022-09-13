using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TinySaveAPI;
using UnityEngine;

public class CameraDisplaySave
{
    private Dictionary<int, int> CameraDisplayAssignments;
    private const string fileName = "CameraDisplaySettings.dat";

    public event EventHandler<Dictionary<int, int>> CameraDisplayAssignmentsLoaded;

    public CameraDisplaySave()
    {
        CameraDisplayAssignments = new Dictionary<int, int>();
        Debug.Log ( $"Application.persistentDataPath and file : {Application.persistentDataPath}/{fileName}" );
    }
    
    public void AssignCameraDisplay(int cameraIdx, int displayIdx)
    {
        CameraDisplayAssignments[cameraIdx] = displayIdx;
    }

    public async void LoadData ( )
    {
        var i = 0;
        var resultString =
            $"{++i}. Load Object Started.\n" +
            $"{++i}. TinySave.LoadAsync<DummyData> ( \"{fileName}\" )\n";

        // This is the only important line of code to load an object.
        var loadResult = await TinySave.LoadAsync<Dictionary<int, int>> ( fileName, SerializationType.Binary );

        // We can now se loadResult.response and loadResult.item.
        resultString += $"{++i}. Returned Response : {loadResult.response}\n";
        if ( loadResult.response.HasFlag ( Response.Success ) )
            resultString += $"{++i}. Loaded Data With: \"{loadResult.item.Count} entries\"\n";

        Debug.Log(resultString);
        CameraDisplayAssignmentsLoaded?.Invoke(this, loadResult.item);
    }

    /// <summary>
    /// Demonstrate saving a PlayerData object.
    /// </summary>
    public async void SaveData ( )
    {
        var i = 0;

        var resultString =
            $"{++i}. Save Data Started.\n" +
            $"{++i}. Call TinySave.SaveAsync ( \"{fileName}\"\n";

        // This is the only important line of code to save an object.
        var saveResult = await TinySave.SaveAsync ( fileName, CameraDisplayAssignments, SerializationType.Binary );

        resultString += $"{++i}. Returned Response : {saveResult}\n";
        Debug.Log(resultString);
    }
}
