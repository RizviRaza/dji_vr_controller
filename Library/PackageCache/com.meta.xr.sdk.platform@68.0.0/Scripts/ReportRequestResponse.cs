// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform
{

  using Description = System.ComponentModel.DescriptionAttribute;

  public enum ReportRequestResponse : int
  {
    [Description("UNKNOWN")]
    Unknown,

    /// Response to the platform notification that the in-app reporting flow
    /// request is handled.
    [Description("HANDLED")]
    Handled,

    /// Response to the platform notification that the in-app reporting flow
    /// request is not handled.
    [Description("UNHANDLED")]
    Unhandled,

    /// Response to the platform notification that the in-app reporting flow is
    /// unavailable or non-existent.
    [Description("UNAVAILABLE")]
    Unavailable,

  }

}
