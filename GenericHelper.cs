using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using Steamworks;

namespace UltraFishing;

public static class GenericHelper {

  public static T[] AppendToArray<T>(T[] array, T element) {
    if (array == null || array.Length == 0) return new T[]{element};

    T[] newArray = new T[array.Length + 1];
    for (int i = 0; i < array.Length; i++) {
      newArray[i] = array[i];
    }
    newArray[array.Length] = element;

    return newArray;
  } 

  public static GameObject FindGameObject(string gameObjectPath) {
    GameObject gameObject = GameObject.Find(gameObjectPath);
    if (gameObject != null) return gameObject;

    GameObject[] sceneObjects = SceneManager.GetActiveScene().GetRootGameObjects();
    string[] parts = gameObjectPath.Split(new char[]{'/'}, StringSplitOptions.RemoveEmptyEntries);
    for (int i = 0; i < parts.Length; i++) {
      parts[i] = parts[i].Replace("%", "/");
    }

    int rootParentIndex = Array.FindIndex(sceneObjects, o => o.name == parts[0]);
    if (rootParentIndex == -1) return null;
    GameObject rootParent = sceneObjects[rootParentIndex];
    if (parts.Length == 1) return rootParent;
    
    string[] rest = new string[parts.Length - 1];
    Array.Copy(parts, 1, rest, 0, rest.Length);
    string subPath = String.Join("/", rest);

    return rootParent.transform.Find(subPath)?.gameObject;
  }

  public static GameObject FindGameObjectContaining(string gameObjectNameSubstring) {
    GameObject[] sceneObjects = SceneManager.GetActiveScene().GetRootGameObjects();

    int gameObjectIndex = Array.FindIndex(sceneObjects, o => o.name.Contains(gameObjectNameSubstring));

    if (gameObjectIndex == -1) {
      return null;
    } 
    else {
      return sceneObjects[gameObjectIndex];
    }
  }

  public static GameObject[] FindGameObjectChildren(string gameObjectPath) {
    GameObject gameObject = FindGameObject(gameObjectPath);

    if (gameObject == null) return null;

    int n = gameObject.transform.childCount;
    GameObject[] children = new GameObject[n];

    for (int i = 0; i < n; i++) {
      children[i] = gameObject.transform.GetChild(i).gameObject;
    }

    return children;
  }

  public static string GetFullPath(GameObject gameObject) {
    Transform transform = gameObject.transform;
    string path = gameObject.name;
    while (transform.parent != null) {
      transform = transform.parent;
      path = transform.name + "/" + path;
    }

    return path;
  }

  public static string GetSteamName() {
    try {
      if (SteamClient.IsLoggedOn) {
        return SteamClient.Name;
      }
      else {
        return "V1";
      }
    }
    catch (Exception) {
      return "V1";
    }
  }
}

