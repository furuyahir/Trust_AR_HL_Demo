using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

using Debug = TinySaveAPI.Logger;


namespace TinySaveAPI
{    
    public static class TinySave
    {
        // ---------------------------------------------------------------------------- Static Variables and Properties
        // The security key has to be either 16 or 24 characters in length.
        public static string SecurityKey { get; set; } = "Security Key... Change!!";
        public static SerializationType SerializationType { get; set; } = SerializationType.Binary;

        public static bool IsWebGL =>
#if UNITY_WEBGL
            true;
#else
            false;
#endif

        public static bool IsDebug
        { get; set; } = 
#if DEBUG
            true;
#else
            false;
#endif

        // ---------------------------------------------------------------------------- Private Variables
        private static BinaryFormatter binaryFormatter;
        

        // ---------------------------------------------------------------------------- Static Methods
        /// <summary>
        /// Create a new item T from the serialised data.
        /// </summary>
        /// <typeparam name="T">A class or struct. Cannot be a UnityEngine.Object, e.g. GameObject or ScriptableObject.</typeparam>
        /// <param name="name">The name of the item you wish to load. Must match exactly the name that was used to serialise the data.</param>
        /// <param name="serializationOverride">An optional serialisation type override.</param>
        /// <returns>A Tuple containing an operation success Response and the newly created (T)Item.</returns>
        public static async Task<(Response response, T item)> LoadAsync<T> ( string name, SerializationType serializationOverride = SerializationType.None )
        {
            if ( !typeof ( T ).IsSerializable )
            {
                Debug.LogWarning ( $"[DEBUG] LoadAsync<{typeof ( T ).Name}> ( \"{name}\" ) {typeof ( T ).Name} is not Serializable!" );
                return (Response.InvalidParameter, default);
            }

            if ( typeof ( T ).IsAssignableFrom ( typeof ( UnityEngine.Object ) ) )
            {
                // We can't create "new" UnityEngine Objects.
                Debug.LogWarning ( $"[DEBUG] LoadAsync<{typeof ( T ).Name}> ( \"{name}\" ). Cannot deserialize to a new UnityEngine.Object." );
                return (Response.InvalidParameter, default);
            }

            if ( serializationOverride == SerializationType.None ) serializationOverride = SerializationType;


            byte [ ] result;
            int bytesRead = 0;

            if ( IsWebGL )
            {
                if ( serializationOverride != SerializationType.Binary )
                {
                    serializationOverride = SerializationType.Binary;
                    Debug.LogWarning ( $"[DEBUG] LoadAsync<{typeof ( T ).Name}> ( \"{name}\" ) Only Binary formatting supported for WebGL. No action required." );
                }
                var fileName = $"{( uint ) name.GetHashCode ( )}";
                var base64String = PlayerPrefs.GetString ( fileName, string.Empty );
                if ( string.IsNullOrEmpty ( base64String ) )
                return (Response.Empty, default);

                try
                {
                    result = Convert.FromBase64String ( base64String );
                }
                catch ( Exception e )
                {
                    Debug.LogWarning ( $"[DEBUG] LoadAsync<{typeof ( T ).Name}> ( \"{name}\" ) Exception caught.\n{e.Message}" );
                    return (Response.Exception, default);
                }
                bytesRead = result.Length;
            }
            else
            {

                if ( binaryFormatter == null ) binaryFormatter = new BinaryFormatter ( );

                var fileName = $"{Application.persistentDataPath}/{( uint ) name.GetHashCode ( )}.dat";

                if ( !File.Exists ( fileName ) )
                {
                    Debug.LogWarning ( $"[DEBUG] LoadAsync<{typeof ( T ).Name}> ( \"{name}\" ) File Not Found : {fileName}" );
                    return (Response.FileNotFound, default);
                }

                using ( FileStream file = File.Open ( fileName, FileMode.Open, FileAccess.Read, FileShare.None ) )
                {
                    result = new byte [ file.Length ];
                    bytesRead = await file.ReadAsync ( result, 0, ( int ) file.Length );
                }
            }


            Debug.Log ( $"[DEBUG] LoadAsync<{typeof ( T ).Name}> ( \"{name}\" ) {bytesRead} bytes read." );
            if ( bytesRead == 0 )
            {
                Debug.Log ( $"[DEBUG] LoadAsync<{typeof ( T ).Name}> ( \"{name}\" ) {bytesRead} bytes read." );
                return (Response.Empty, default);
            }

            switch ( serializationOverride )
            {
                case SerializationType.Binary:
                    try
                    {
                        using ( MemoryStream stream = new MemoryStream ( result ) )
                        return (Response.Success, ( T ) binaryFormatter.Deserialize ( stream ) );
                    }
                    catch ( Exception e )
                    {
                        Debug.LogWarning ( $"[DEBUG] LoadAsync<{typeof ( T ).Name}> ( \"{name}\" ) [{serializationOverride}] Exception caught.\n{e.Message}" );
                        return (Response.Exception, default);
                    }

                case SerializationType.JsonEncrypted:
                case SerializationType.Json:
                    try
                    {
                        string jsonString = serializationOverride == SerializationType.JsonEncrypted ? Decrypt ( result ) : Encoding.UTF8.GetString ( result );
                        return (Response.Success, JsonUtility.FromJson<T> ( jsonString ));
                    }
                    catch ( Exception e )
                    {
                        Debug.LogWarning ( $"[DEBUG] LoadAsync<{typeof ( T ).Name}> ( \"{name}\" ) [{serializationOverride}] Exception caught.\n{e.Message}" );
                        return (Response.Exception, default);
                    }
            }
            return (Response.None, default);
        }


        /// <summary>
        /// Fill a given instance of an object from the serialized data. This form of the method only accepts UnityEngine.Objects, and works with encrypted or unencrypted JSON.
        /// </summary>
        /// <typeparam name="T">A class that derives from UnityEngine.Object, e.g. GameObject or ScriptableObject.</typeparam>
        /// <param name="name">The name of the item you wish to load. Must match exactly the name that was used to serialise the data.</param>
        /// <param name="serializationOverride">An optional serialisation type override.</param>
        /// <returns>A Response indicating whether or not the operation succeeded.</returns>
        public static async Task<Response> LoadAsync<T> ( string name, T item, SerializationType serializationOverride = SerializationType.None ) where T : UnityEngine.Object
        {
            if ( serializationOverride == SerializationType.None ) serializationOverride = SerializationType;

            if ( serializationOverride != SerializationType.Json && serializationOverride != SerializationType.JsonEncrypted )
            {
                serializationOverride = SerializationType.JsonEncrypted;
                Debug.LogWarning ( $"[DEBUG] LoadAsync<{typeof ( T ).Name}> ( \"{name}\", {typeof ( T ).Name} ) Only Json or JsonEncrypted formatting supported for WebGL. No action required, defaulting to JsonEncrypted." );
            }

            if ( !typeof ( MonoBehaviour ).IsAssignableFrom ( typeof ( T ) ) && !typeof ( ScriptableObject ).IsAssignableFrom ( typeof ( T ) ) )
            {
                Debug.LogWarning ( $"[DEBUG] LoadAsync<{typeof ( T ).Name}> ( \"{name}\", {typeof ( T ).Name} ) Method requires either a MonoBehaviour or ScriptableObject." );
                return Response.InvalidParameter;
            }

            byte [ ] result;
            int bytesRead = 0;
            string json = string.Empty;

            if ( IsWebGL )
            {
                var fileName = $"{( uint ) name.GetHashCode ( )}";
                json = PlayerPrefs.GetString ( fileName, string.Empty );
                if ( json == string.Empty || json == "{}" ) return Response.Empty;

                bytesRead = json.Length;
                if ( serializationOverride == SerializationType.JsonEncrypted ) json = Decrypt ( Convert.FromBase64String ( json ) );
            }
            else
            {
                var fileName = $"{Application.persistentDataPath}/{( uint ) name.GetHashCode ( )}.dat";

                if ( !File.Exists ( fileName ) )
                {
                    Debug.LogWarning ( $"[DEBUG] LoadAsync<{typeof ( T ).Name}> ( \"{name}\", {typeof ( T ).Name} ) File Not Found : {fileName}" );
                    return Response.FileNotFound;
                }

                using ( FileStream file = File.Open ( fileName, FileMode.Open, FileAccess.Read, FileShare.None ) )
                {
                    result = new byte [ file.Length ];
                    bytesRead = await file.ReadAsync ( result, 0, ( int ) file.Length );
                }
                if ( serializationOverride == SerializationType.JsonEncrypted ) json = Decrypt ( result );
            }

            if ( bytesRead == 0 )
            {
                Debug.Log ( $"[DEBUG] LoadAsync<{typeof ( T ).Name}> ( \"{name}\", {typeof ( T ).Name} ) {bytesRead} bytes read." );
                return Response.Empty;
            }
            
            try
            {
                JsonUtility.FromJsonOverwrite ( json, item );
                return Response.Success;
            }
            catch ( Exception e )
            {
                Debug.LogWarning ( $"[DEBUG] LoadAsync<{typeof ( T ).Name}> ( \"{name}\", {typeof ( T ).Name} ) [{serializationOverride}] Exception caught.\n\n{e.Message}" );
                return Response.Exception;
            }
        }



        /// <summary>
        /// Save the supplied object, using the given name.
        /// </summary>
        /// <param name="name">The name used as the filename, or key if using PlayerPrefs (WebGL).</param>
        /// <param name="obj">The object to be serialised.</param>
        /// <param name="securityKeyOverride">If left null, uses the static SecurityKey value.</param>
        /// <returns>A Response indicating whether or not the operation succeeded.</returns>
        public static async Task<Response> SaveAsync ( string name, object obj, SerializationType serializationOverride = SerializationType.None )
        {
            if ( serializationOverride == SerializationType.None ) serializationOverride = SerializationType;

            var objectType = obj.GetType ( );

            if ( string.IsNullOrEmpty ( name ) || obj == null )
            {
                Debug.LogWarning ( $"[DEBUG] SaveAsync ( \"{name}\", {objectType.Name} ) [{serializationOverride}] name or object are empty/null." );
                return Response.InvalidParameter;
            }

            if ( objectType.IsAssignableFrom ( typeof ( UnityEngine.Object ) ) && !objectType.IsSerializable )
            {
                Debug.LogWarning ( $"[DEBUG] SaveAsync ( \"{name}\", {objectType.Name} ) [{serializationOverride}] is not Serializable!" );
                return Response.InvalidParameter;
            }
                                                         
            var fileName = $"{( uint ) name.GetHashCode ( )}";
            if ( !IsWebGL )
            {
                if ( binaryFormatter == null ) binaryFormatter = new BinaryFormatter ( );
                fileName = $"{Application.persistentDataPath}/{fileName}.dat";
            }

            switch ( serializationOverride )
            {
                case SerializationType.Binary:
                    try
                    {
                        if ( IsWebGL )
                        {
                            using ( MemoryStream stream = new MemoryStream ( ) )
                            {
                                binaryFormatter.Serialize ( stream, obj );
                                PlayerPrefs.SetString ( fileName, Convert.ToBase64String ( stream.ToArray ( ) ) );
                            }
                        }
                        else
                        {
                            using ( FileStream file = File.Open ( fileName, FileMode.Create ) )
                                binaryFormatter.Serialize ( file, obj );
                        }
                        return Response.Success;
                    }
                    catch ( Exception e )
                    {
                        Debug.LogWarning ( $"[DEBUG] SaveAsync ( \"{name}\", {objectType.Name} ) [{serializationOverride}] Exception caught.\n\n{e.Message}" );
                        return Response.Exception;
                    }

                case SerializationType.Json:
                case SerializationType.JsonEncrypted:
                    try
                    {
                        var json = JsonUtility.ToJson ( obj );
                        if ( json == "{}" )
                            return Response.UnableToSerialise;
                        
                        if ( IsWebGL )
                        {
                            if ( serializationOverride == SerializationType.JsonEncrypted ) json = Convert.ToBase64String ( Encrypt ( json ) );
                            PlayerPrefs.SetString ( fileName, json );
                        }
                        else
                        {
                            using ( FileStream file = File.Open ( fileName, FileMode.Create ) )
                            {
                                byte [ ] bytes = serializationOverride == SerializationType.JsonEncrypted ? Encrypt ( json ) : Encoding.UTF8.GetBytes ( json );
                                await file.WriteAsync ( bytes, 0, bytes.Length );
                            }
                        }
                        return Response.Success;
                    }
                    catch ( Exception e )
                    {
                        Debug.LogWarning ( $"[DEBUG] SaveAsync ( \"{name}\", {objectType.Name} ) [{serializationOverride}] Exception caught.\n\n{e.Message}" );
                        return Response.Exception;
                    }
            }
            return Response.Empty;
        }


        /// <summary>
        /// Encrypt a string, using a security key, into a byte array.
        /// </summary>
        /// <param name="dataString">The string data to encrypt.</param>
        /// <param name="securityKeyOverride">If left null, uses the static SecurityKey value.</param>
        /// <returns>A byte array containing the encrypted string.</returns>
        public static byte [ ] Encrypt ( string dataString, string securityKeyOverride = null )
        {
            if ( string.IsNullOrEmpty ( securityKeyOverride ) )
            {
                if ( SecurityKey == null ) return null;
                securityKeyOverride = SecurityKey;
            }

            var keyArray = Encoding.UTF8.GetBytes ( securityKeyOverride );
            var dataBytesArray = Encoding.UTF8.GetBytes ( dataString );

            using ( var provider = new TripleDESCryptoServiceProvider
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            } )
            {


                using ( var encryptor = provider.CreateEncryptor ( ) )
                {
                    var resultArray = encryptor.TransformFinalBlock ( dataBytesArray, 0, dataBytesArray.Length );
                    provider.Clear ( );
                    return resultArray;
                }
            }
        }


        /// <summary>
        /// Decrypt a byte array, using a security key, into a string.
        /// </summary>
        /// <param name="dataBytesArray">The byte array containing the encrypted data.</param>
        /// <param name="securityKeyOverride">If left null, uses the static SecurityKey value.</param>
        /// <returns>A string containing the unencrypted data.</returns>
        public static string Decrypt ( byte [ ] dataBytesArray, string securityKeyOverride = null )
        {
            if ( string.IsNullOrEmpty ( securityKeyOverride ) )
            {
                if ( SecurityKey == null ) return null;
                securityKeyOverride = SecurityKey;
            }

            var keyArray = Encoding.UTF8.GetBytes ( securityKeyOverride );

            using ( var provider = new TripleDESCryptoServiceProvider
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            } )
            {
                using ( var decryptor = provider.CreateDecryptor ( ) )
                {
                    byte [ ] resultArray = decryptor.TransformFinalBlock ( dataBytesArray, 0, dataBytesArray.Length );
                    provider.Clear ( );
                    return Encoding.UTF8.GetString ( resultArray );
                }
            }
        }
    }
}