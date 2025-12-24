using UnityEngine;

namespace  UltraFishing;

public class NewFishingRod : FishingRodWeapon {
  private static FakeWater currentFakeWater; 

  public void OnDisable() {
    ResetFishing();
    FishingHUD.Instance.SetState(FishingRodState.ReadyToThrow);
  }

  public new void FishCaughtAndGrabbed() {
    animator.SetTrigger(Idle);
    FishingHUD.Instance.ShowFishCaught(show: true, hookedFishe.fish);
    NewFishingRod.CreateFishPickup(fishPickupTemplate, hookedFishe.fish, grab: true);
    ResetFishing();
  }

  public new void ThrowBaitEvent() {
    if (this.spawnedBaitCon == null) {
      this.spawnedBaitCon = Object.Instantiate<FishBait>(this.baitPrefab, this.rodTip.position, Quaternion.identity, this.rodTip);
      this.spawnedBaitCon.landed = false;

      if (currentFakeWater != null && currentFakeWater.customSplash != null) {
        this.spawnedBaitCon.splashPrefab = currentFakeWater.customSplash;
      }

      this.spawnedBaitCon.ThrowStart(this.targetingCircle.transform.position, this.rodTip, this);
    }
  }

  public static new GameObject CreateFishPickup(ItemIdentifier template, FishObject fish, bool grab, bool unlock = true) {
    if (unlock) {
      FishManager.Instance.UnlockFish(fish);
    }

    ItemIdentifier itemIdentifier;
    if (fish.customPickup != null) {
      itemIdentifier = Object.Instantiate(fish.customPickup);
      if (!itemIdentifier.GetComponent<FishObjectReference>()) {
        itemIdentifier.gameObject.AddComponent<FishObjectReference>().fishObject = fish;
      }
    }
    else {
      itemIdentifier = Object.Instantiate(template);
      itemIdentifier.gameObject.AddComponent<FishObjectReference>().fishObject = fish;
      Transform obj = itemIdentifier.transform.GetChild(0).transform;
      Vector3 localPosition = obj.localPosition;
      Quaternion localRotation = obj.localRotation;
      Vector3 localScale = obj.localScale;
      Object.Destroy(obj.gameObject);
      GameObject obj2 = fish.InstantiateDumb();
      obj2.transform.SetParent(itemIdentifier.transform);
      obj2.transform.localPosition = localPosition;
      obj2.transform.localRotation = localRotation;
      obj2.transform.localScale = localScale;
    }

    if (grab) {
      ItemIdentifier heldObject = FistControl.Instance.heldObject;
      if (heldObject == null) {
        FistControl.Instance.currentPunch.ResetHeldState();
        FistControl.Instance.currentPunch.ForceHold(itemIdentifier);
        return itemIdentifier.gameObject;
      }
      switch (heldObject.itemType) {
        case ItemType.None: 
        case ItemType.Readable: 
        case ItemType.Soap: 
        case ItemType.Breakable: 
          Object.Destroy(FistControl.Instance.heldObject.gameObject);
          FistControl.Instance.currentPunch.ResetHeldState();
          FistControl.Instance.currentPunch.ForceHold(itemIdentifier);
          break;
        default:
          FistControl.Instance.currentPunch.ResetHeldState();
          FistControl.Instance.currentPunch.ForceHold(itemIdentifier);
          FistControl.Instance.currentPunch.ForceDrop();
          FistControl.Instance.currentPunch.ResetHeldState();
          FistControl.Instance.currentPunch.ForceHold(heldObject);
          break;
      }
    }

    return itemIdentifier.gameObject;
  }

  public void NewUpdate() {
    if (GameStateManager.Instance.PlayerInputLocked || InputManager.Instance.PerformingCheatMenuCombo()) {
      return;
    }
    if ((float)FishingHUD.Instance.timeSinceFishCaught >= 1f && (InputManager.Instance.InputSource.Punch.WasPerformedThisFrame || InputManager.Instance.InputSource.Fire1.WasPerformedThisFrame)) {
      FishingHUD.Instance.ShowFishCaught(show: false);
    }
    FishingHUD.Instance.SetState(state);
    switch (state) {
      case FishingRodState.ReadyToThrow:
        if (InputManager.Instance.InputSource.Fire1.WasPerformedThisFrame && (float)timeSinceAction > 0.1f) {
          FishingHUD.Instance.SetPowerMeter(0f, canFish: false);
          selectedPower = 0f;
          climaxed = false;
          fishHooked = false;
          baitThrown = false;
          state = FishingRodState.SelectingPower;
          targetingCircle = Object.Instantiate(targetPrefab, approximateTargetPosition, Quaternion.identity);
          timeSinceAction = 0f;
        }
        break;
      case FishingRodState.SelectingPower: {
          selectedPower += (Time.deltaTime * 0.4f + selectedPower * 0.01f) * (climaxed ? (-0.5f) : 1f);
          if (selectedPower > 1f) {
            selectedPower = 1f;
            climaxed = true;
          }
          if (selectedPower < 0.1f) {
            climaxed = false;
          }
          Vector3 vector = approximateTargetPosition;
          bool flag = false;
          if (Physics.Raycast(vector + Vector3.up * 3f, Vector3.down, out var hitInfo, 30f)) {
            // Plugin.logger.LogInfo(GenericHelper.GetFullPath(hitInfo.collider.gameObject));
            vector = hitInfo.point;
            if (hitInfo.collider.TryGetComponent<Water>(out var component) && (bool)component.fishDB) {
              currentFishPool = component.fishDB;
              currentWater = component;
              flag = true;
              if ((bool)component.overrideFishingPoint) {
                vector = component.overrideFishingPoint.position;
              }
            }
            else if (hitInfo.collider.TryGetComponent<FakeWater>(out var fwcomponent) && (bool)fwcomponent.fishDB) {
              currentFishPool = fwcomponent.fishDB;
              currentFakeWater = fwcomponent;
              flag = true;
              if ((bool)fwcomponent.overrideFishingPoint) {
                vector = fwcomponent.overrideFishingPoint.position;
              }
            }
            else {
              currentFishPool = null;
              currentWater = null;
              currentFakeWater = null;
              flag = false;
            }
          }
          else {
            currentFishPool = null;
            currentWater = null;
            currentFakeWater = null;
            flag = false;
          }
          FishingHUD.Instance.SetPowerMeter(selectedPower, flag);
          if (flag) {
            targetingCircle.transform.position = vector + Vector3.up * 0.5f;
            targetingCircle.SetState(isGood: true, Vector3.Distance(hitInfo.point, NewMovement.Instance.transform.position));
            targetingCircle.waterNameText.text = currentFishPool.fullName;
            targetingCircle.waterNameText.color = currentFishPool.symbolColor;
          }
          else {
            targetingCircle.transform.position = vector + Vector3.up * 0.5f;
            targetingCircle.SetState(isGood: false, Vector3.Distance(hitInfo.point, NewMovement.Instance.transform.position));
            targetingCircle.waterNameText.text = "";
          }
          targetingCircle.transform.forward = NewMovement.Instance.transform.forward;
          if (InputManager.Instance.InputSource.Fire1.WasCanceledThisFrame && (float)timeSinceAction > 0.1f) {
            if (flag) {
              targetingCircle.GetComponent<Animator>().SetTrigger(FishingRodWeapon.Set);
              animator.ResetTrigger(FishingRodWeapon.Throw);
              state = FishingRodState.Throwing;
              timeSinceAction = 0f;
            }
            else {
              ResetFishing();
            }
          }
          break;
        }
      case FishingRodState.Throwing:
        targetingCircle.transform.forward = NewMovement.Instance.transform.forward;
        fishHooked = false;
        if (!baitThrown) {
          baitThrown = true;
          animator.SetTrigger(FishingRodWeapon.Throw);
        }
        if ((bool)spawnedBaitCon && spawnedBaitCon.landed) {
          state = FishingRodState.WaitingForFish;
          timeSinceBaitInWater = 0f;
          distanceAfterThrow = Vector3.Distance(NewMovement.Instance.transform.position, spawnedBaitCon.baitPoint.position);
          Object.Destroy(targetingCircle.gameObject);
        }
        break;
      case FishingRodState.WaitingForFish:
        baitThrown = false;
        if (Vector3.Distance(NewMovement.Instance.transform.position, spawnedBaitCon.baitPoint.position) > distanceAfterThrow + 30f) {
          Object.Destroy(spawnedBaitCon.gameObject);
          HudMessageReceiver.Instance.SendHudMessage("Fishing interrupted");
          ResetFishing();
          break;
        }
        if (!fishHooked && Random.value < 0.002f + (float)timeSinceBaitInWater * 0.01f) {
          if (currentWater == null) {
            hookedFishe = currentFishPool.GetRandomFish(currentFakeWater.attractFish);
          }
          else {
            hookedFishe = currentFishPool.GetRandomFish(currentWater.attractFish);
          }
          if (hookedFishe == null) {
            if (!noFishErrorDisplayed) {
              noFishErrorDisplayed = true;
              HudMessageReceiver.Instance.SendHudMessage("Nothing seems to be biting here...");
            }
            break;
          }
          if (currentWater == null) {
            currentFakeWater.attractFish = null;
          }
          else {
            currentWater.attractFish = null;
          }
          fishHooked = true;
          FishingHUD.Instance.SetFishHooked(hooked: true);
          spawnedBaitCon.FishHooked();
        }
        if (InputManager.Instance.InputSource.Fire1.IsPressed || InputManager.Instance.InputSource.Fire2.IsPressed) {
          animator.SetTrigger(FishingRodWeapon.Pull);
          if (fishHooked) {
            FishingHUD.Instance.SetFishHooked(hooked: false);
            state = FishingRodState.FishStruggle;
            spawnedBaitCon.CatchFish(hookedFishe.fish);
          }
          else {
            Object.Destroy(spawnedBaitCon.gameObject);
            animator.SetTrigger(FishingRodWeapon.Idle);
            animator.ResetTrigger(FishingRodWeapon.Throw);
            animator.Play(FishingRodWeapon.Idle);
            ResetFishing();
          }
        }
        break;
      case FishingRodState.FishStruggle:
        fishDesirePosition = Mathf.PerlinNoise(Time.time * 0.3f, 0f);
        fishTolerance = 0.1f + 0.4f * Mathf.PerlinNoise(Time.time * 0.4f, 0f);
        if (InputManager.Instance.InputSource.Fire1.IsPressed) {
          playerPositionVelocity += 1.9f * Time.deltaTime;
          animator.SetTrigger(FishingRodWeapon.Pull);
        }
        else if (InputManager.Instance.InputSource.Fire2.IsPressed) {
          playerPositionVelocity -= 1.9f * Time.deltaTime;
          animator.SetTrigger(FishingRodWeapon.Pull);
        }
        else {
          playerPositionVelocity *= 1f - 2f * Time.deltaTime;
        }
        playerProvidedPosition += playerPositionVelocity * Time.deltaTime;
        if (playerProvidedPosition > 1f) {
          playerProvidedPosition = 1f;
          playerPositionVelocity = 0f - playerPositionVelocity;
        }
        if (playerProvidedPosition < 0f) {
          playerProvidedPosition = 0f;
          playerPositionVelocity = 0f - playerPositionVelocity;
        }
        FishingHUD.Instance.SetPlayerStrugglePosition(playerProvidedPosition);
        FishingHUD.Instance.SetStruggleSatisfied(struggleSatisfied);
        FishingHUD.Instance.SetFishDesire(Mathf.Clamp01(topBound), Mathf.Clamp01(bottomBound));
        spawnedBaitCon.allowedToProgress = struggleSatisfied;
        FishingHUD.Instance.SetStruggleProgress(spawnedBaitCon.flyProgress, hookedFishe.fish.blockedIcon, hookedFishe.fish.icon);
        break;
    }
  }
}
