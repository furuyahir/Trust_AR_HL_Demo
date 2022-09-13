using UnityEngine;

// Include TinySaveAPI so you can use the TinySave features.
using TinySaveAPI;


[DisallowMultipleComponent]
#if UNITY_2018_3_OR_NEWER
[ExecuteAlways]
#else
[ExecuteInEditMode]
#endif
public class TinySaveManagerComponent : MonoBehaviour
{
    private static TinySaveManagerComponent singleton;

    [Tooltip ( "The security key used to encrypt JSON data. Only required if saving as Encrytped JSON. Note: The key should be either 16 or 24 characters in length." )]
    public string SecurityKey = "Security Key... Change!!";

    [Tooltip ( "The default Serialization Type used." )]
    public SerializationType SerializationType = SerializationType.Binary;

    private void OnEnable ( )
    {
        var items = FindObjectsOfType<TinySaveManagerComponent> ( );
        if ( items.Length == 1 )
        {
            singleton = this;
        }

        if ( singleton != this )
        {
            Debug.LogWarning ( $"An extra SaveItComponent has been found on GameObject \"{this.gameObject.name}\". This instance is being destryoed. Add only one TinySaveManager instance to your game." );
            if ( Application.isEditor )
                DestroyImmediate ( this );
            else
                Destroy ( this );
            return;
        }

        TinySave.SecurityKey = SecurityKey;
        TinySave.SerializationType = SerializationType;
    }

    private void OnValidate ( )
    {
        if ( SecurityKey.Length < 16 )
            SecurityKey = SecurityKey.PadRight ( 16, ' ' );
        else if ( SecurityKey.Length > 16 && SecurityKey.Length < 24 )
            SecurityKey = SecurityKey.PadRight ( 24, ' ' );
        else if ( SecurityKey.Length > 24 )
            SecurityKey = SecurityKey.Substring ( 0, 24 );
    }
}