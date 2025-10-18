using System;
using UnityEngine;

namespace UltraFishing;

public class GlobalFishEncyclopedia : FishEncyclopedia {
  private int currentPage;

  public void StartEncyclopedia() {
    fishButtonTemplate.gameObject.SetActive(value: false);
    for (int i = 0; i < GlobalFishManager.FishCount(); i++) {
      FishObject fish = GlobalFishManager.GetFish(i);
      bool value = GlobalFishManager.GetFishValue(i);
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
    }

    FishManager instance = FishManager.Instance;
    instance.onFishUnlocked = (Action<FishObject>)Delegate.Combine(instance.onFishUnlocked, new Action<FishObject>(OnFishUnlocked));

    currentPage = 1;
    DisplayCurrentPage();
	}

  private void DisplayCurrentPage() {
    for (int i = 1; i < fishGrid.childCount; i++) {
      if (i > (currentPage - 1) * 12 && i <= currentPage * 12) {
        fishGrid.GetChild(i).gameObject.SetActive(true);
      }
      else {
        fishGrid.GetChild(i).gameObject.SetActive(false);
      }
    }
  }

  public void NextPage() {
    if (currentPage * 12 < fishGrid.childCount) {
      currentPage++;
      DisplayCurrentPage();
    }
  }

  public void PreviousPage() {
    if (currentPage > 1) {
      currentPage--;
      DisplayCurrentPage();
    }
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
    fishDescription.text = GlobalFishManager.GetFishDescription(fish);
    fishPicker.SetActive(value: false);
    fishInfoContainer.SetActive(value: true);
    DisplayFish(fish);
  }
}
