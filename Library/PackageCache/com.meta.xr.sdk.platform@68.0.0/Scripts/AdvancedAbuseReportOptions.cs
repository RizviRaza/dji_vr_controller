// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  public class AdvancedAbuseReportOptions {

    public AdvancedAbuseReportOptions() {
      Handle = CAPI.ovr_AdvancedAbuseReportOptions_Create();
    }

    /// This field is intended to allow developers to pass custom metadata through
    /// the report flow. The metadata passed through is included with the report
    /// received by the developer.
    public void SetDeveloperDefinedContext(string key, string value) {
      CAPI.ovr_AdvancedAbuseReportOptions_SetDeveloperDefinedContextString(Handle, key, value);
    }

    public void ClearDeveloperDefinedContext() {
      CAPI.ovr_AdvancedAbuseReportOptions_ClearDeveloperDefinedContext(Handle);
    }

    /// If report_type is object/content, a string representing the type of content
    /// being reported. This should correspond to the object_type string used in
    /// the UI
    public void SetObjectType(string value) {
      CAPI.ovr_AdvancedAbuseReportOptions_SetObjectType(Handle, value);
    }

    /// The intended entity being reported, whether user or object/content.
    public void SetReportType(AbuseReportType value) {
      CAPI.ovr_AdvancedAbuseReportOptions_SetReportType(Handle, value);
    }

    /// Provide a list of users to suggest for reporting. This list should include
    /// users that the reporter has recently interacted with to aid them in
    /// selecting the right user to report.
    public void AddSuggestedUser(UInt64 userID) {
      CAPI.ovr_AdvancedAbuseReportOptions_AddSuggestedUser(Handle, userID);
    }

    public void ClearSuggestedUsers() {
      CAPI.ovr_AdvancedAbuseReportOptions_ClearSuggestedUsers(Handle);
    }

    /// The video mode controls whether or not the abuse report flow should collect
    /// evidence and whether it is optional or not. "Collect" requires video
    /// evidence to be provided by the user. "Optional" presents the user with the
    /// option to provide video evidence. "Skip" bypasses the video evidence
    /// collection step altogether.
    public void SetVideoMode(AbuseReportVideoMode value) {
      CAPI.ovr_AdvancedAbuseReportOptions_SetVideoMode(Handle, value);
    }


    /// For passing to native C
    public static explicit operator IntPtr(AdvancedAbuseReportOptions options) {
      return options != null ? options.Handle : IntPtr.Zero;
    }

    ~AdvancedAbuseReportOptions() {
      CAPI.ovr_AdvancedAbuseReportOptions_Destroy(Handle);
    }

    IntPtr Handle;
  }
}
