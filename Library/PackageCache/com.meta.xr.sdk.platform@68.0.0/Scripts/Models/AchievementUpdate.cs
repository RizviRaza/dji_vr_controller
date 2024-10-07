// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// Represents an update to an existing achievement.
  public class AchievementUpdate
  {
    /// This indicates if this update caused the achievement to unlock.
    public readonly bool JustUnlocked;
    /// The unique AchievementDefinition.GetName() used to reference the updated
    /// achievement.
    public readonly string Name;


    public AchievementUpdate(IntPtr o)
    {
      JustUnlocked = CAPI.ovr_AchievementUpdate_GetJustUnlocked(o);
      Name = CAPI.ovr_AchievementUpdate_GetName(o);
    }
  }

}
