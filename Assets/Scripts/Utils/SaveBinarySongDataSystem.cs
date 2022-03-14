using UnityEngine;
using System;
using System.IO;
//using System.Runtime.Serialization.Formatters.Binary;

namespace Mio.TileMaster {
    public static class SaveBinarySongDataSystem {
        //static SharpSerializer binarySerializer;
        static SaveBinarySongDataSystem(){
           // binarySerializer = new SharpSerializer(true);
        }
        public static bool SaveTileData (SongTileData saveGame, string name) {
            //BinaryFormatter formatter = new BinaryFormatter();

            //using (FileStream stream = new FileStream(GetSavePath(name), FileMode.Create)) {
            //    try {
            //        formatter.Serialize(stream, saveGame);
            //    }
            //    catch (Exception e) {
            //        Debug.LogWarning(e.Message);
            //        return false;
            //    }
            //}

            //return true;

            //BinaryFormatter formatter = new BinaryFormatter();
            string data = JsonUtility.ToJson(saveGame);
            using (FileStream stream = new FileStream(GetSavePath(name), FileMode.Create)) {
                try {
                    //binarySerializer.Serialize(saveGame, stream);
                    var bytes = System.Text.ASCIIEncoding.ASCII.GetBytes(data);
                    stream.Write(bytes, 0, bytes.Length);
                }
                catch (Exception e) {
                    Debug.LogWarning(e.Message);
                    return false;
                }
                finally {
                    stream.Close();
                }
            }

            return true;
            //return false;
        }

        public static SongTileData LoadTileDataFromResource (string storeID) {
            //Debug.Log("Trying to load tile data: " + storeID);
            //TextAsset asset = Resources.Load("songs/" + storeID) as TextAsset;
            //if (asset != null) {
            //    BinaryFormatter formatter = new BinaryFormatter();
            //    using (Stream s = new MemoryStream(asset.bytes)) {
            //        try {
            //            return formatter.Deserialize(s) as SongTileData;
            //        }
            //        catch (Exception ex) {
            //            Debug.LogWarning("Could not de-serialize level data with exception: " + ex);
            //            return null;
            //        }
            //    }
            //}
            //Debug.Log("NULLLL: " + storeID);
            //return null;

            TextAsset asset = Resources.Load("songs/" + storeID) as TextAsset;
            if (asset != null) {
                //using (Stream s = new MemoryStream(asset.bytes)) {
                //    try {
                //        string data = System.Text.ASCIIEncoding.ASCII.GetString()
                //        //SongTileData level = binarySerializer.Deserialize(s) as SongTileData;
                //        //return level;
                //    }
                //    catch (Exception ex) {
                //        Debug.LogWarning("Could not de-serialize level data with exception: " + ex);
                //        return null;
                //    }
                //}
                return JsonUtility.FromJson<SongTileData>(asset.text);
                //return RSManager.DeserializeData<SongTileData>(asset.text);
            }
            Debug.Log("NULLLL: " + storeID);
            return null;
            //return null;
        }

        public static SongTileData LoadTileData (string name) {
            if (!IsTileDataExist(name)) {
                return null;
            }

            //BinaryFormatter formatter = new BinaryFormatter();

            //using (FileStream stream = new FileStream(GetSavePath(name), FileMode.Open)) {
            //    try {
            //        return formatter.Deserialize(stream) as SongTileData;
            //    }
            //    catch (Exception e) {
            //        Debug.Log(e.Message);
            //        return null;
            //    }
            //}

            using (FileStream stream = new FileStream(GetSavePath(name), FileMode.Open)) {
                try {
                    //SongTileData level = binarySerializer.Deserialize(stream) as SongTileData;
                    //stream.Close();
                    //return level;
                    var bytes = ReadFully(stream);
                    string data = System.Text.ASCIIEncoding.ASCII.GetString(bytes, 0, bytes.Length);
                    return JsonUtility.FromJson<SongTileData>(data);
                }
                catch (Exception e) {
                    Debug.Log(e.Message);
                    return null;
                }
            }
        }

        public static byte[] ReadFully (Stream input) {
            byte[] buffer = new byte[150 * 1024];
            using (MemoryStream ms = new MemoryStream()) {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0) {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public static bool DeleteTileData (string name) {
            try {
                File.Delete(GetSavePath(name));
            }
            catch (Exception) {
                return false;
            }

            return true;
            //return true;
        }

        public static bool IsTileDataExist (string name) {
            return FileUtilities.IsFileExist(name + ".bytes", false);
            //return false;
        }

        private static string GetSavePath (string name) {
            return FileUtilities.GetWritablePath(name + ".bytes");
            //return string.Empty;
        }
    }
}