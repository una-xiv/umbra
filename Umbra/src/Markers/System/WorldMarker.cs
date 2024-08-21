/* Umbra | (c) 2024 by Una              ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra is free software: you can redistribute  \/     \/             \/
 *     it and/or modify it under the terms of the GNU Affero General Public
 *     License as published by the Free Software Foundation, either version 3
 *     of the License, or (at your option) any later version.
 *
 *     Umbra UI is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

using System;
using System.Numerics;
using Umbra.Common;

namespace Umbra.Markers.System;

public record WorldMarker
{
    /// <summary>
    /// A unique identifier for this marker. If not set, a random GUID will be
    /// generated and assigned to this property once the marker is added to the
    /// registry via the <see cref="WorldMarkerRegistry.SetMarker"/> method.
    /// </summary>
    public string Key {
        get { return _key ??= Guid.NewGuid().ToString(); }
        set {
            if (_key is not null) throw new ArgumentException("Cannot change the key of an existing marker.");

            _key = value;
        }
    }

    /// <summary>
    /// Specifies the label of the marker.
    /// </summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// Specifies an optional sub-label of the marker. A sub-label will not be
    /// displayed if the marker has been grouped together with other markers
    /// that are in the same location.
    /// </summary>
    public string? SubLabel { get; set; }

    /// <summary>
    /// Specifies the ID of the icon to be displayed for the marker.
    /// </summary>
    public uint IconId { get; set; }

    /// <summary>
    /// <para>
    /// Determines where the marker is placed in the world.
    /// </para>
    /// <para>
    /// When dealing with map coordinates only, the X and Z values correspond
    /// to the map's X and Y axes respectively. The Y value is the elevation.
    /// Since the elevation is not always present, leaving it at 0 will let
    /// the renderer raycast to the ground to determine the proper elevation.
    /// </para>
    /// </summary>
    public Vector3 Position { get; set; }

    /// <summary>
    /// Defines the distance between the camera and the marker at which the
    /// marker will start to fade out.
    /// </summary>
    public Vector2 FadeDistance { get; set; }

    /// <summary>
    /// The ID of the map where the marker is located.
    /// </summary>
    public uint MapId { get; set; }

    /// <summary>
    /// Whether the marker should be displayed in the compass when the world
    /// marker is out of view. The compass is a circular area around the player
    /// that displays directional indicators for out-of-view world markers.
    /// </summary>
    public bool ShowOnCompass { get; set; }

    /// <summary>
    /// Whether the marker is visible.
    /// </summary>
    public bool IsVisible { get; set; } = true;

    public Vector3 WorldPosition {
        get {
            if (Position == _lastPos) return _worldPos;

            _lastPos = Position;

            return Position.Y == 0
                ? _worldPos = Raycaster.Raycast(this)
                : _worldPos = Position;
        }
    }

    private string? _key;
    private Vector3 _lastPos;
    private Vector3 _worldPos;
    private WorldMarkerRaycaster? _raycaster;

    private WorldMarkerRaycaster Raycaster {
        get {
            return _raycaster ??= Framework.Service<WorldMarkerRaycaster>();
        }
    }
}
