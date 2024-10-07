// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// A challenge entry object contains information about an individual entry
  /// within a challenge.
  public class ChallengeEntry
  {
    /// A displayable score for this challenge entry. The score is formatted with
    /// thousands separators and the relevant units are appended based on the
    /// associated leaderboard's score type.
    public readonly string DisplayScore;
    /// A 2KB custom data field that is associated with the challenge entry.
    public readonly byte[] ExtraData;
    /// The ID of this challenge entry which can be used by
    /// Challenges.GetEntriesByIds() and Challenges.GetEntries().
    public readonly UInt64 ID;
    /// Challenges can be ranked by highest or lowest scores within a time period.
    /// This indicates the position of this challenge entry.
    public readonly int Rank;
    /// The raw underlying value of the challenge entry score.
    public readonly long Score;
    /// The timestamp of the creation of this entry in the challenge.
    public readonly DateTime Timestamp;
    /// The user corresponding to this entry within the challenge.
    public readonly User User;


    public ChallengeEntry(IntPtr o)
    {
      DisplayScore = CAPI.ovr_ChallengeEntry_GetDisplayScore(o);
      ExtraData = CAPI.ovr_ChallengeEntry_GetExtraData(o);
      ID = CAPI.ovr_ChallengeEntry_GetID(o);
      Rank = CAPI.ovr_ChallengeEntry_GetRank(o);
      Score = CAPI.ovr_ChallengeEntry_GetScore(o);
      Timestamp = CAPI.ovr_ChallengeEntry_GetTimestamp(o);
      User = new User(CAPI.ovr_ChallengeEntry_GetUser(o));
    }
  }

  public class ChallengeEntryList : DeserializableList<ChallengeEntry> {
    public ChallengeEntryList(IntPtr a) {
      var count = (int)CAPI.ovr_ChallengeEntryArray_GetSize(a);
      _Data = new List<ChallengeEntry>(count);
      for (int i = 0; i < count; i++) {
        _Data.Add(new ChallengeEntry(CAPI.ovr_ChallengeEntryArray_GetElement(a, (UIntPtr)i)));
      }

      TotalCount = CAPI.ovr_ChallengeEntryArray_GetTotalCount(a);
      _PreviousUrl = CAPI.ovr_ChallengeEntryArray_GetPreviousUrl(a);
      _NextUrl = CAPI.ovr_ChallengeEntryArray_GetNextUrl(a);
    }

    public readonly ulong TotalCount;
  }
}
