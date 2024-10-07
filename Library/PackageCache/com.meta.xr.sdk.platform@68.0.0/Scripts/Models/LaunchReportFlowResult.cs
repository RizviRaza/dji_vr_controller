// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// Results of the launched report dialog including resulting report ID and
  /// user action.
  public class LaunchReportFlowResult
  {
    /// Whether the viewer chose to cancel the report flow.
    public readonly bool DidCancel;
    /// ID of the report created by the user.
    public readonly UInt64 UserReportId;


    public LaunchReportFlowResult(IntPtr o)
    {
      DidCancel = CAPI.ovr_LaunchReportFlowResult_GetDidCancel(o);
      UserReportId = CAPI.ovr_LaunchReportFlowResult_GetUserReportId(o);
    }
  }

}
