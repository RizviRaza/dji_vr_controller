// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform.Models
{
  using System;
  using System.Collections;
  using Oculus.Platform.Models;
  using System.Collections.Generic;
  using UnityEngine;

  /// The notification about updating a party status information to participate
  /// user.
  public class PartyUpdateNotification
  {
    /// An enum that specifies the type of action related to the party and user
    /// that this notification holds.
    ///
    /// PartyUpdateAction.Join: The user is going to join the party.
    ///
    /// PartyUpdateAction.Leave: The user is going to leave the party.
    ///
    /// PartyUpdateAction.Invite: The user is invited to the party.
    ///
    /// PartyUpdateAction.Uninvite: The user is uninvited from the party.
    public readonly PartyUpdateAction Action;
    /// The Id of the party that will be updated. This can be retrieved with
    /// PartyID.
    public readonly UInt64 PartyId;
    /// The id of the User who initiated the action that this party update status
    /// notification is in reference to. This is equivalent to User.GetID() for
    /// this user.
    public readonly UInt64 SenderId;
    /// A timestamp denoting when the action happened that this status update
    /// notification refers to.
    public readonly string UpdateTimestamp;
    /// The alias of the User whose party status has changed.
    public readonly string UserAlias;
    /// The ID of the user whose party status has changed.
    public readonly UInt64 UserId;
    /// The displayable name of the User whose party status has changed. This is
    /// equivalent to User.GetDisplayName() for this user.
    public readonly string UserName;


    public PartyUpdateNotification(IntPtr o)
    {
      Action = CAPI.ovr_PartyUpdateNotification_GetAction(o);
      PartyId = CAPI.ovr_PartyUpdateNotification_GetPartyId(o);
      SenderId = CAPI.ovr_PartyUpdateNotification_GetSenderId(o);
      UpdateTimestamp = CAPI.ovr_PartyUpdateNotification_GetUpdateTimestamp(o);
      UserAlias = CAPI.ovr_PartyUpdateNotification_GetUserAlias(o);
      UserId = CAPI.ovr_PartyUpdateNotification_GetUserId(o);
      UserName = CAPI.ovr_PartyUpdateNotification_GetUserName(o);
    }
  }

}
