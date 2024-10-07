// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  public class ChallengeOptions {

    public ChallengeOptions() {
      Handle = CAPI.ovr_ChallengeOptions_Create();
    }

    /// The description of the challenge that can be retrieved with
    /// Challenge.GetDescription().
    public void SetDescription(string value) {
      CAPI.ovr_ChallengeOptions_SetDescription(Handle, value);
    }

    /// The timestamp when this challenge ends which can be retrieved with
    /// Challenge.GetEndDate().
    public void SetEndDate(DateTime value) {
      CAPI.ovr_ChallengeOptions_SetEndDate(Handle, value);
    }

    /// This indicates whether to include challenges that are currently active.
    public void SetIncludeActiveChallenges(bool value) {
      CAPI.ovr_ChallengeOptions_SetIncludeActiveChallenges(Handle, value);
    }

    /// This indicates whether to include challenges that have not yet started.
    public void SetIncludeFutureChallenges(bool value) {
      CAPI.ovr_ChallengeOptions_SetIncludeFutureChallenges(Handle, value);
    }

    /// This indicates whether to include challenges that have already ended.
    public void SetIncludePastChallenges(bool value) {
      CAPI.ovr_ChallengeOptions_SetIncludePastChallenges(Handle, value);
    }

    /// Optional: Only find challenges belonging to this leaderboard.
    public void SetLeaderboardName(string value) {
      CAPI.ovr_ChallengeOptions_SetLeaderboardName(Handle, value);
    }

    /// The timestamp when this challenge starts which can be retrieved with
    /// Challenge.GetStartDate().
    public void SetStartDate(DateTime value) {
      CAPI.ovr_ChallengeOptions_SetStartDate(Handle, value);
    }

    /// The title of the challenge that can be retrieved with Challenge.GetTitle().
    public void SetTitle(string value) {
      CAPI.ovr_ChallengeOptions_SetTitle(Handle, value);
    }

    /// An enum that specifies what filter to apply to the list of returned
    /// challenges.
    ///
    /// Returns all public ((ChallengeVisibility.Public)) and invite-only
    /// (ChallengeVisibility.InviteOnly) Challenges in which the user is a
    /// participant or invitee. Excludes private (ChallengeVisibility.Private)
    /// challenges.
    ///
    /// ChallengeViewerFilter.Participating - Returns challenges the user is
    /// participating in.
    ///
    /// ChallengeViewerFilter.Invited - Returns challenges the user is invited to.
    ///
    /// ChallengeViewerFilter.ParticipatingOrInvited - Returns challenges the user
    /// is either participating in or invited to.
    public void SetViewerFilter(ChallengeViewerFilter value) {
      CAPI.ovr_ChallengeOptions_SetViewerFilter(Handle, value);
    }

    /// Specifies who can see and participate in this challenge. It can be
    /// retrieved with Challenge.GetVisibility().
    public void SetVisibility(ChallengeVisibility value) {
      CAPI.ovr_ChallengeOptions_SetVisibility(Handle, value);
    }


    /// For passing to native C
    public static explicit operator IntPtr(ChallengeOptions options) {
      return options != null ? options.Handle : IntPtr.Zero;
    }

    ~ChallengeOptions() {
      CAPI.ovr_ChallengeOptions_Destroy(Handle);
    }

    IntPtr Handle;
  }
}
