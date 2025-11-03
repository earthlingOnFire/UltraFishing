using System.IO;
using System.Collections.Generic;

namespace UltraFishing;

public static class SaveHelper {

  private static Dictionary<string, byte[]> saveDataCache = new Dictionary<string, byte[]>();

  public static void WriteToSave(string savePath, int saveSlot, byte value) {
    byte[] saveData = GetSaveData(savePath);

    int newLength = saveData.Length;
    if (saveSlot >= newLength) {
      newLength = saveSlot + 1;
    }

    byte[] newData = new byte[newLength];
    for (int i = 0; i < newLength; i++) {
      saveData[i] = i switch {
        _ when (i == saveSlot) => value,
        _ when (i < saveData.Length) => saveData[i],
        _ => 0,
      };
    }

    File.WriteAllBytes(savePath, newData);
    saveDataCache[savePath] = newData;
  }

  public static byte ReadFromSave(string savePath, int saveSlot) {
    byte[] saveData = GetSaveData(savePath);

    if (saveSlot >= saveData.Length) {
      return 0;
    }

    return GetSaveData(savePath)[saveSlot];
  }

  private static byte[] GetSaveData(string savePath) {
    if (saveDataCache.ContainsKey(savePath)) {
      return saveDataCache[savePath];
    }

    if (!File.Exists(savePath)) {
      return new byte[]{0};
    }

    byte[] data = File.ReadAllBytes(savePath);
    saveDataCache.Add(savePath, data);
    return data;
  }
}
