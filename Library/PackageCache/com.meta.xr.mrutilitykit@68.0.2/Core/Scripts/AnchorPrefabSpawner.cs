/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using Meta.XR.Util;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = System.Random;

namespace Meta.XR.MRUtilityKit
{
    // tool for swapping scene prefabs with standardized unity objects
    [Feature(Feature.Scene)]
    public class AnchorPrefabSpawner : MonoBehaviour, ICustomAnchorPrefabSpawner
    {
        public enum ScalingMode
        {
            /// Stretch each axis to exactly match the size of the Plane/Volume.
            Stretch,

            /// Scale each axis by the same amount to maintain the correct aspect ratio.
            UniformScaling,

            /// Scale the X and Z axes uniformly but the Y scale can be different.
            UniformXZScale,

            /// Don't perform any scaling.
            NoScaling,

            /// Custom logic, extend this class and override CustomPrefabScaling with your own implementation
            Custom
        }

        public enum AlignMode
        {
            /// For volumes align to the base, for planes align to the center.
            Automatic,

            /// Align the bottom of the prefab with the bottom of the volume or plane
            Bottom,

            /// Align the center of the prefab with the center of the volume or plane
            Center,

            /// Don't add any local offset to the prefab.
            NoAlignment,

            /// Custom logic, extend this class and override CustomPrefabAlignment with your own implementation
            Custom
        }

        public enum SelectionMode
        {
            // Randomly choose a prefab from the list every time
            Random,

            // Chose the prefab the has the smallest difference between its size with the one of the anchor
            ClosestSize,

            /// Custom logic, extend this class and override CustomPrefabSelection with your own implementation
            Custom
        }

        [Serializable]
        public struct AnchorPrefabGroup : IEquatable<AnchorPrefabGroup>
        {
            [FormerlySerializedAs("_include")] [SerializeField, Tooltip("Anchors to include.")]
            public MRUKAnchor.SceneLabels Labels;

            [SerializeField, Tooltip("Prefab(s) to spawn (randomly chosen from list.)")]
            public List<GameObject> Prefabs;

            [SerializeField]
            [Tooltip("The logic that determines what prefab to chose when spawning the relative labels' game objects")]
            public SelectionMode PrefabSelection;

            [SerializeField,
             Tooltip(
                 "When enabled, the prefab will be rotated to try and match the aspect ratio of the volume as closely as possible. This is most useful for long and thin volumes, keep this disabled for objects with an aspect ratio close to 1:1. Only applies to volumes.")]
            public bool MatchAspectRatio;

            [SerializeField,
             Tooltip(
                 "When calculate facing direction is enabled the prefab will be rotated to face away from the closest wall. If match aspect ratio is also enabled then that will take precedence and it will be constrained to a choice between 2 directions only.Only applies to volumes.")]
            public bool CalculateFacingDirection;

            [SerializeField,
             Tooltip(
                 "Set what scaling mode to apply to the prefab. By default the prefab will be stretched to fit the size of the plane/volume. But in some cases this may not be desirable and can be customized here.")]
            public ScalingMode Scaling;

            [SerializeField, Tooltip("Spawn new object at the center, top or bottom of the anchor.")]
            public AlignMode Alignment;

            [SerializeField, Tooltip("Don't analyze prefab, just assume a default scale of 1.")]
            public bool IgnorePrefabSize;

            public bool Equals(AnchorPrefabGroup other)
            {
                return Labels == other.Labels && Equals(Prefabs, other.Prefabs) &&
                       PrefabSelection == other.PrefabSelection && MatchAspectRatio == other.MatchAspectRatio &&
                       CalculateFacingDirection == other.CalculateFacingDirection && Scaling == other.Scaling &&
                       Alignment == other.Alignment && IgnorePrefabSize == other.IgnorePrefabSize;
            }

            public override bool Equals(object obj)
            {
                return obj is AnchorPrefabGroup other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine((int)Labels, Prefabs, (int)PrefabSelection, MatchAspectRatio,
                    CalculateFacingDirection, (int)Scaling, (int)Alignment, IgnorePrefabSize);
            }

            public static bool operator ==(AnchorPrefabGroup left, AnchorPrefabGroup right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(AnchorPrefabGroup left, AnchorPrefabGroup right)
            {
                return !left.Equals(right);
            }
        }


        [Tooltip("When the scene data is loaded, this controls what room(s) the prefabs will spawn in.")]
        public MRUK.RoomFilter SpawnOnStart = MRUK.RoomFilter.CurrentRoomOnly;

        [Tooltip("If enabled, updates on scene elements such as rooms and anchors will be handled by this class")]
        internal bool TrackUpdates = true;

        [Tooltip("Specify a seed value for consistent prefab selection (0 = Random).")]
        public int SeedValue;

        public Dictionary<MRUKAnchor, GameObject> AnchorPrefabSpawnerObjects { get; private set; } = new();

        [Obsolete("Event onPrefabSpawned will be deprecated in a future version"), NonSerialized]
        public UnityEvent onPrefabSpawned = new();

        [Obsolete(
            "Use AnchorPrefabSpawnerObjects property instead. This property is inefficient because it will generate a new list each time it is accessed")]
        public List<GameObject> SpawnedPrefabs => new(AnchorPrefabSpawnerObjects.Values);

        private MRUK.SceneTrackingSettings SceneTrackingSettings;

        public List<AnchorPrefabGroup> PrefabsToSpawn;
        protected Random _random;
        private static readonly string Suffix = "(PrefabSpawner Clone)";
        private Func<Vector3, Vector3> _customPrefabScalingVolume;
        private Func<Bounds, Bounds?, (Vector3, Vector3)> _customPrefabAlignmentVolume;
        private Func<Vector2, Vector2> _customPrefabScalingPlaneRect;
        private Func<Rect, Bounds?, (Vector3, Vector2)> _customPrefabAlignmentPlaneRect;
        private Func<MRUKAnchor, List<GameObject>, GameObject> _customPrefabSelection;

        protected virtual void Start()
        {
#if UNITY_EDITOR
            OVRTelemetry.Start(TelemetryConstants.MarkerId.LoadAnchorPrefabSpawner).Send();
#endif
            if (MRUK.Instance is null)
            {
                return;
            }

            SceneTrackingSettings.UnTrackedRooms = new();
            SceneTrackingSettings.UnTrackedAnchors = new();

            MRUK.Instance.RegisterSceneLoadedCallback(() =>
            {
                if (SpawnOnStart == MRUK.RoomFilter.None)
                {
                    return;
                }

                switch (SpawnOnStart)
                {
                    case MRUK.RoomFilter.CurrentRoomOnly:
                        SpawnPrefabs(MRUK.Instance.GetCurrentRoom());
                        break;
                    case MRUK.RoomFilter.AllRooms:
                        SpawnPrefabs();
                        break;
                    case MRUK.RoomFilter.None:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });

            if (!TrackUpdates)
            {
                return;
            }
        }

        protected virtual void OnEnable()
        {
            if (MRUK.Instance)
            {
                MRUK.Instance.RoomCreatedEvent.AddListener(ReceiveCreatedRoom);
                MRUK.Instance.RoomRemovedEvent.AddListener(ReceiveRemovedRoom);
            }
        }

        protected virtual void OnDisable()
        {
            if (MRUK.Instance)
            {
                MRUK.Instance.RoomCreatedEvent.RemoveListener(ReceiveCreatedRoom);
                MRUK.Instance.RoomRemovedEvent.RemoveListener(ReceiveRemovedRoom);
            }
        }

        protected virtual void ReceiveRemovedRoom(MRUKRoom room)
        {
            ClearPrefabs(room);
            UnRegisterAnchorUpdates(room);
        }

        protected virtual void UnRegisterAnchorUpdates(MRUKRoom room)
        {
            room.AnchorCreatedEvent.RemoveListener(ReceiveAnchorCreatedEvent);
            room.AnchorRemovedEvent.RemoveListener(ReceiveAnchorRemovedCallback);
            room.AnchorUpdatedEvent.RemoveListener(ReceiveAnchorUpdatedCallback);
        }

        protected virtual void RegisterAnchorUpdates(MRUKRoom room)
        {
            room.AnchorCreatedEvent.AddListener(ReceiveAnchorCreatedEvent);
            room.AnchorRemovedEvent.AddListener(ReceiveAnchorRemovedCallback);
            room.AnchorUpdatedEvent.AddListener(ReceiveAnchorUpdatedCallback);
        }

        protected virtual void ReceiveAnchorUpdatedCallback(MRUKAnchor anchorInfo)
        {
            // only update the anchor when we track updates
            // &
            // only create when the anchor or parent room is tracked
            if (SceneTrackingSettings.UnTrackedRooms.Contains(anchorInfo.Room) ||
                SceneTrackingSettings.UnTrackedAnchors.Contains(anchorInfo) ||
                !TrackUpdates)
            {
                return;
            }

            ClearPrefabs();
            SpawnPrefabs(anchorInfo);
        }

        protected virtual void ReceiveAnchorRemovedCallback(MRUKAnchor anchorInfo)
        {
            ClearPrefabs();
        }

        protected virtual void ReceiveAnchorCreatedEvent(MRUKAnchor anchorInfo)
        {
            // only create the anchor when we track updates
            // &
            // only create when the parent room is tracked
            if (SceneTrackingSettings.UnTrackedRooms.Contains(anchorInfo.Room) ||
                !TrackUpdates)
            {
                return;
            }

            SpawnPrefabs();
        }

        protected virtual void ReceiveCreatedRoom(MRUKRoom room)
        {
            //only create the room when we track room updates
            if (TrackUpdates &&
                SpawnOnStart == MRUK.RoomFilter.AllRooms)
            {
                SpawnPrefabs(room);
                RegisterAnchorUpdates(room);
            }
        }

        /// <summary>
        ///     Clears all the spawned gameobjects from this AnchorPrefabSpawner in the given room
        /// </summary>
        /// <param name="room">The room from where to remove all the spawned objects</param>
        protected virtual void ClearPrefabs(MRUKRoom room)
        {
            List<MRUKAnchor> anchorsToRemove = new();
            foreach (var kv in AnchorPrefabSpawnerObjects)
            {
                if (kv.Key.Room != room)
                {
                    continue;
                }

                ClearPrefab(kv.Value);
                anchorsToRemove.Add(kv.Key);
            }

            foreach (var anchor in anchorsToRemove)
            {
                AnchorPrefabSpawnerObjects.Remove(anchor);
            }

            SceneTrackingSettings.UnTrackedRooms.Add(room);
        }

        protected virtual void ClearPrefab(GameObject go)
        {
            Destroy(go);
        }

        /// <summary>
        ///     Clears the gameobject associated with the anchor. Useful when receiving an event that a
        ///     specific anchor has been removed
        /// </summary>
        /// <param name="anchorInfo">The anchor reference</param>
        protected virtual void ClearPrefab(MRUKAnchor anchorInfo)
        {
            if (!AnchorPrefabSpawnerObjects.ContainsKey(anchorInfo))
            {
                return;
            }

            ClearPrefab(AnchorPrefabSpawnerObjects[anchorInfo]);
            AnchorPrefabSpawnerObjects.Remove(anchorInfo);
            SceneTrackingSettings.UnTrackedAnchors.Add(anchorInfo);
        }

        /// <summary>
        ///     Clears all the gameobjects created with the PrefabSpawner
        /// </summary>
        protected virtual void ClearPrefabs()
        {
            foreach (var kv in AnchorPrefabSpawnerObjects)
            {
                ClearPrefab(kv.Value);
            }

            AnchorPrefabSpawnerObjects.Clear();
        }


        /// <summary>
        ///     Spawns prefabs according to the settings
        /// </summary>
        /// <param name="clearPrefabs">Clear already existing prefabs before.</param>
        protected virtual void SpawnPrefabs(bool clearPrefabs = true)
        {
            // Perform a cleanup if necessary
            if (clearPrefabs)
            {
                ClearPrefabs();
            }

            foreach (var room in MRUK.Instance.Rooms)
            {
                SpawnPrefabsInternal(room);
            }
#pragma warning disable CS0618 // Type or member is obsolete
            onPrefabSpawned?.Invoke();
#pragma warning restore CS0618 // Type or member is obsolete
        }

        /// <summary>
        ///     Creates gameobjects for the given room.
        /// </summary>
        /// <param name="room">The room reference</param>
        /// <param name="clearPrefabs">clear all before adding them again</param>
        protected virtual void SpawnPrefabs(MRUKRoom room, bool clearPrefabs = true)
        {
            // Perform a cleanup if necessary
            if (clearPrefabs)
            {
                ClearPrefabs();
            }

            SpawnPrefabsInternal(room);
#pragma warning disable CS0618 // Type or member is obsolete
            onPrefabSpawned?.Invoke();
#pragma warning restore CS0618 // Type or member is obsolete
        }

        private void SpawnPrefabsInternal(MRUKRoom room)
        {
            InitializeRandom(ref SeedValue);
            foreach (var anchor in room.Anchors)
            {
                SpawnPrefab(anchor);
            }
        }

        protected virtual void SpawnPrefab(MRUKAnchor anchorInfo)
        {
            var prefabToCreate = LabelToPrefab(anchorInfo.Label, anchorInfo, out var prefabGroup);
            if (prefabToCreate == null)
            {
                return;
            }

            if (AnchorPrefabSpawnerObjects.ContainsKey(anchorInfo))
            {
                Debug.LogWarning("Anchor already associated with a gameobject spawned from this AnchorPrefabSpawner");
                return;
            }

            // Create a new instance of the prefab
            // We will translate location and scale differently depending on the label.
            var prefab = Instantiate(prefabToCreate, anchorInfo.transform);
            prefab.name = string.Concat(prefabToCreate.name, Suffix);
            prefab.name = prefabToCreate.name + Suffix;
            prefab.transform.parent = anchorInfo.transform;

            var prefabBounds = prefabGroup.IgnorePrefabSize ? null : Utilities.GetPrefabBounds(prefabToCreate);

            var resizer = prefab.GetComponentInChildren<GridSliceResizer>(true);
            if (!prefabBounds.HasValue && resizer)
            {
                prefabBounds = resizer.OriginalMesh.bounds;
            }

            var prefabSize = prefabBounds?.size ?? Vector3.one;

            if (anchorInfo.VolumeBounds.HasValue)
            {
                var cardinalAxisIndex = 0;
                if (prefabGroup.CalculateFacingDirection && !prefabGroup.MatchAspectRatio)
                {
                    anchorInfo.Room.GetDirectionAwayFromClosestWall(anchorInfo, out cardinalAxisIndex);
                }

                var volumeBounds = AnchorPrefabSpawnerUtilities.RotateVolumeBounds(anchorInfo.VolumeBounds.Value,
                    cardinalAxisIndex);

                var volumeSize = volumeBounds.size;
                var scale = new Vector3(volumeSize.x / prefabSize.x, volumeSize.z / prefabSize.y,
                    volumeSize.y / prefabSize.z); // flipped z and y to correct orientation

                if (prefabGroup.MatchAspectRatio)
                {
                    AnchorPrefabSpawnerUtilities.MatchAspectRatio(anchorInfo, prefabGroup.CalculateFacingDirection,
                        prefabSize, volumeSize, ref cardinalAxisIndex, ref volumeBounds, ref scale);
                }

                scale = prefabGroup.Scaling == ScalingMode.Custom
                    ? CustomPrefabScaling(scale)
                    : AnchorPrefabSpawnerUtilities.ScalePrefab(scale, prefabGroup.Scaling);

                prefab.transform.localPosition = prefabGroup.Alignment == AlignMode.Custom
                    ? CustomPrefabAlignment(volumeBounds, prefabBounds)
                    : AnchorPrefabSpawnerUtilities.AlignPrefabPivot(volumeBounds, prefabBounds, scale,
                        prefabGroup.Alignment);

                // scene geometry is unusual, we need to swap Y/Z for a more standard prefab structure
                prefab.transform.localRotation = Quaternion.Euler((cardinalAxisIndex - 1) * 90, -90, -90);
                prefab.transform.localScale = scale;
            }

            else if (anchorInfo.PlaneRect.HasValue)
            {
                var planeSize = anchorInfo.PlaneRect.Value.size;
                var scale = new Vector2(planeSize.x / prefabSize.x, planeSize.y / prefabSize.y);

                prefab.transform.localScale = prefabGroup.Scaling == ScalingMode.Custom
                    ? CustomPrefabScaling(scale)
                    : AnchorPrefabSpawnerUtilities.ScalePrefab(scale, prefabGroup.Scaling);

                prefab.transform.localPosition = prefabGroup.Alignment == AlignMode.Custom
                    ? CustomPrefabAlignment(anchorInfo.PlaneRect.Value, prefabBounds)
                    : AnchorPrefabSpawnerUtilities.AlignPrefabPivot(anchorInfo.PlaneRect.Value, prefabBounds, scale,
                        prefabGroup.Alignment);
            }

            AnchorPrefabSpawnerObjects.Add(anchorInfo, prefab);
        }

        private GameObject LabelToPrefab(MRUKAnchor.SceneLabels labels, MRUKAnchor anchor,
            out AnchorPrefabGroup prefabGroup)
        {
            foreach (var item in PrefabsToSpawn)
            {
                if ((item.Labels & labels) == 0 || ((item.Prefabs == null ||
                                                     item.Prefabs.Count == 0) &&
                                                    item.PrefabSelection != SelectionMode.Custom))
                {
                    continue;
                }

                GameObject prefabObjectToSpawn = null;
                if (item.PrefabSelection == SelectionMode.Custom)
                {
                    prefabObjectToSpawn = CustomPrefabSelection(anchor, item.Prefabs);
                }
                else
                {
                    prefabObjectToSpawn =
                        AnchorPrefabSpawnerUtilities.SelectPrefab(anchor, item.PrefabSelection, item.Prefabs,
                            _random);
                }

                prefabGroup = item;
                return prefabObjectToSpawn;
            }

            prefabGroup = new();
            return null;
        }

        /// <summary>
        ///     Initializes a new instance of the Random class using the current seed.
        /// </summary>
        public void InitializeRandom(ref int seed)
        {
            if (seed == 0)
            {
                seed = Environment.TickCount;
            }

            _random = new Random(seed);
        }

        public virtual GameObject CustomPrefabSelection(MRUKAnchor anchor, List<GameObject> prefabs)
        {
            throw new(
                "A custom prefab selection method was selected but no implementation was provided. " +
                "Extend this class and override the `CustomPrefabSelection` method with your custom logic.");
        }

        public virtual Vector3 CustomPrefabScaling(Vector3 localScale)
        {
            throw new NotImplementedException(
                "A custom scaling method for an anchor's volume is selected but no implementation " +
                "was provided. Extend this class and override the `CustomPrefabVolumeScaling` method with your custom logic.");
        }

        public virtual Vector2 CustomPrefabScaling(Vector2 localScale)
        {
            throw new NotImplementedException(
                "A custom scaling method was selected but no implementation was provided. " +
                "Extend this class and override the `CustomPrefabPlaneRectScaling` method with your custom logic.");
        }

        public virtual Vector3 CustomPrefabAlignment(Bounds anchorVolumeBounds, Bounds? prefabBounds)
        {
            throw new NotImplementedException(
                "A custom volume alignment method was selected but no implementation was provided." +
                "Extend this class and override the `CustomPrefabAlignment` method with your custom logic.");
        }

        public virtual Vector3 CustomPrefabAlignment(Rect anchorPlaneRect, Bounds? prefabBounds)
        {
            throw new NotImplementedException(
                "A custom prefab selection method was selected but no implementation was provided. " +
                "Extend this class and override the `CustomPrefabAlignment` method with your custom logic.");
        }

        private void OnDestroy()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            onPrefabSpawned.RemoveAllListeners();
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
}
