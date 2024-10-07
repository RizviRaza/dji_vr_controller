// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// The unlock progress of a particular achievement. See
  /// [Achievements](https://developer.oculus.com/documentation/native/ps-
  /// achievements/) for more information.
  public class AchievementProgress
  {
    /// For bitfield achievements, the current bitfield state.
    public readonly string Bitfield;
    /// For count achievements, the current counter state.
    public readonly ulong Count;
    /// If the user has already unlocked this achievement.
    public readonly bool IsUnlocked;
    /// The unique string that you use to reference the achievement in your app, as
    /// specified in the developer dashboard.
    public readonly string Name;
    /// If the achievement is unlocked, the time when it was unlocked.
    public readonly DateTime UnlockTime;


    public AchievementProgress(IntPtr o)
    {
      Bitfield = CAPI.ovr_AchievementProgress_GetBitfield(o);
      Count = CAPI.ovr_AchievementProgress_GetCount(o);
      IsUnlocked = CAPI.ovr_AchievementProgress_GetIsUnlocked(o);
      Name = CAPI.ovr_AchievementProgress_GetName(o);
      UnlockTime = CAPI.ovr_AchievementProgress_GetUnlockTime(o);
    }
  }

  public class AchievementProgressList : DeserializableList<AchievementProgress> {
    public AchievementProgressList(IntPtr a) {
      var count = (int)CAPI.ovr_AchievementProgressArray_GetSize(a);
      _Data = new List<AchievementProgress>(count);
      for (int i = 0; i < count; i++) {
        _Data.Add(new AchievementProgress(CAPI.ovr_AchievementProgressArray_GetElement(a, (UIntPtr)i)));
      }

      _NextUrl = CAPI.ovr_AchievementProgressArray_GetNextUrl(a);
    }

  }
}
