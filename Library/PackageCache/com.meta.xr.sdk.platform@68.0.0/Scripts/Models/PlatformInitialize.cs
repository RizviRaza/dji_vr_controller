// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// A PlatformInitialize object defines an attempt at initializating the Oculus
  /// Platform SDK. It contains the result of attempting to initialize the
  /// platform.
  public class PlatformInitialize
  {
    /// The result of attempting to initialize the platform.
    public readonly PlatformInitializeResult Result;


    public PlatformInitialize(IntPtr o)
    {
      Result = CAPI.ovr_PlatformInitialize_GetResult(o);
    }
  }

}
