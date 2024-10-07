// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  public class AbuseReportOptions {

    public AbuseReportOptions() {
      Handle = CAPI.ovr_AbuseReportOptions_Create();
    }

    /// Set whether or not to show the user selection step in the report dialog.
    public void SetPreventPeopleChooser(bool value) {
      CAPI.ovr_AbuseReportOptions_SetPreventPeopleChooser(Handle, value);
    }

    /// The intended entity being reported, whether user or object/content.
    public void SetReportType(AbuseReportType value) {
      CAPI.ovr_AbuseReportOptions_SetReportType(Handle, value);
    }


    /// For passing to native C
    public static explicit operator IntPtr(AbuseReportOptions options) {
      return options != null ? options.Handle : IntPtr.Zero;
    }

    ~AbuseReportOptions() {
      CAPI.ovr_AbuseReportOptions_Destroy(Handle);
    }

    IntPtr Handle;
  }
}
