// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

#pragma warning disable 0618

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// An leaderboard entry object contains information about a user in the
  /// leaderboard.
  public class LeaderboardEntry
  {
    /// The score displayed in the leaderboard of this entry.
    public readonly string DisplayScore;
    /// A 2KB custom data field that is associated with the leaderboard entry. This
    /// can be a game replay or anything that provides more detail about the entry
    /// to the viewer. It will be used by two entry methods:
    /// Leaderboards.WriteEntry() and
    /// Leaderboards.WriteEntryWithSupplementaryMetric()
    public readonly byte[] ExtraData;
    /// The ID of this leaderboard entry.
    public readonly UInt64 ID;
    /// The rank of this leaderboard entry in the leaderboard.
    public readonly int Rank;
    /// The raw underlying value of the leaderboard entry score.
    public readonly long Score;
    /// A metric that can be used for tiebreakers by
    /// Leaderboards.WriteEntryWithSupplementaryMetric().
    // May be null. Check before using.
    public readonly SupplementaryMetric SupplementaryMetricOptional;
    [Obsolete("Deprecated in favor of SupplementaryMetricOptional")]
    public readonly SupplementaryMetric SupplementaryMetric;
    /// The timestamp of this entry being created in the leaderboard.
    public readonly DateTime Timestamp;
    /// User of this leaderboard entry.
    public readonly User User;


    public LeaderboardEntry(IntPtr o)
    {
      DisplayScore = CAPI.ovr_LeaderboardEntry_GetDisplayScore(o);
      ExtraData = CAPI.ovr_LeaderboardEntry_GetExtraData(o);
      ID = CAPI.ovr_LeaderboardEntry_GetID(o);
      Rank = CAPI.ovr_LeaderboardEntry_GetRank(o);
      Score = CAPI.ovr_LeaderboardEntry_GetScore(o);
      {
        var pointer = CAPI.ovr_LeaderboardEntry_GetSupplementaryMetric(o);
        SupplementaryMetric = new SupplementaryMetric(pointer);
        if (pointer == IntPtr.Zero) {
          SupplementaryMetricOptional = null;
        } else {
          SupplementaryMetricOptional = SupplementaryMetric;
        }
      }
      Timestamp = CAPI.ovr_LeaderboardEntry_GetTimestamp(o);
      User = new User(CAPI.ovr_LeaderboardEntry_GetUser(o));
    }
  }

  public class LeaderboardEntryList : DeserializableList<LeaderboardEntry> {
    public LeaderboardEntryList(IntPtr a) {
      var count = (int)CAPI.ovr_LeaderboardEntryArray_GetSize(a);
      _Data = new List<LeaderboardEntry>(count);
      for (int i = 0; i < count; i++) {
        _Data.Add(new LeaderboardEntry(CAPI.ovr_LeaderboardEntryArray_GetElement(a, (UIntPtr)i)));
      }

      TotalCount = CAPI.ovr_LeaderboardEntryArray_GetTotalCount(a);
      _PreviousUrl = CAPI.ovr_LeaderboardEntryArray_GetPreviousUrl(a);
      _NextUrl = CAPI.ovr_LeaderboardEntryArray_GetNextUrl(a);
    }

    public readonly ulong TotalCount;
  }
}
