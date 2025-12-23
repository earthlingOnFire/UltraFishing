using System;
using System.Collections.Generic;
using UnityEngine;

namespace UltraFishing;

public class GlobalFishEncyclopedia : FishEncyclopedia {
  private int currentPage;
  private List<int> pageIndices = new List<int>();
  private List<string> pageTitles = new List<string>();
  public GameObject mannequinFishDescription;

  public void StartEncyclopedia() {
    fishButtonTemplate.gameObject.SetActive(value: false);

    foreach (FishCollection collection in GlobalFishManager.GetFishCollections()) {
      pageIndices.Add(fishGrid.childCount);
      pageTitles.Add(collection.name);
      int previousPageIndex = fishGrid.childCount;

      foreach (FishData fishData in collection.fishes) {
        FishObject fish = fishData.fish;
        bool value = fishData.found;

        FishMenuButton fishMenuButton = UnityEngine.Object.Instantiate(
            fishButtonTemplate, 
            fishGrid, 
            worldPositionStays: false
        );
        fishButtons.Add(fish, fishMenuButton);
        fishMenuButton.Populate(fish, !value);
        fishMenuButton.GetComponent<ControllerPointer>().OnPressed.RemoveAllListeners();
        fishMenuButton.GetComponent<ControllerPointer>().OnPressed.AddListener(delegate {
            SelectFish(fish);
        });

        if (fishGrid.childCount == previousPageIndex + 13) {
          pageIndices.Add(fishGrid.childCount - 1);
          pageTitles.Add(collection.name);
          previousPageIndex = fishGrid.childCount - 1;
        }
      }
    }

    FishManager instance = FishManager.Instance;
    instance.onFishUnlocked = (Action<FishObject>)Delegate.Combine(instance.onFishUnlocked, new Action<FishObject>(OnFishUnlocked));

    currentPage = 0;

    DisplayCurrentPage();
	}

  private void DisplayCurrentPage() {
    int first = pageIndices[currentPage];
    int last;
    
    if (pageIndices.Count == currentPage + 1) {
      last = fishGrid.childCount;
    }
    else {
      last = pageIndices[currentPage + 1];
    }

    for (int i = 1; i < fishGrid.childCount; i++) {
      if (i >= first && i < last) {
        fishGrid.GetChild(i).gameObject.SetActive(true);
      }
      else {
        fishGrid.GetChild(i).gameObject.SetActive(false);
      }
    }

    ChangeTitle();
  }

  public void NextPage() {
    Plugin.logger.LogInfo($"Button pressed! Current page is {currentPage}");
    if (currentPage + 1 < pageIndices.Count) {
      currentPage++;
      DisplayCurrentPage();
    }
  }

  public void PreviousPage() {
    if (currentPage > 0) {
      currentPage--;
      DisplayCurrentPage();
    }
  }

  private void ChangeTitle() {
    string newTitle = pageTitles[currentPage];

    Transform titleTransform = fishGrid.parent.GetChild(0);
    titleTransform.GetComponent<TMPro.TextMeshProUGUI>().text = newTitle;
  }

	private new void DisplayFish(FishObject fish) {
    foreach (Transform item in fish3dRenderContainer.transform) {
      UnityEngine.Object.Destroy(item.gameObject);
    }
    if (GlobalFishManager.FoundFish(fish)) {
      GameObject obj = fish.InstantiateDumb();
      obj.transform.SetParent(fish3dRenderContainer.transform);
      obj.transform.localPosition = Vector3.zero;
      obj.transform.localScale = Vector3.one;
      SandboxUtils.SetLayerDeep(obj.transform, LayerMask.NameToLayer("VirtualRender"));
    }
	}

  public new void SelectFish(FishObject fish) {
    fishName.text = (GlobalFishManager.FoundFish(fish) ? fish.fishName : "???");

    if (fish.fishName == "Mannequin Fish") {
      mannequinFishDescription.SetActive(true);
      fishDescription.text = "";
    }
    else {
      mannequinFishDescription.SetActive(false);
      fishDescription.text = GlobalFishManager.GetFishDescription(fish);
    }

    fishPicker.SetActive(value: false);
    fishInfoContainer.SetActive(value: true);
    DisplayFish(fish);
  }
}
