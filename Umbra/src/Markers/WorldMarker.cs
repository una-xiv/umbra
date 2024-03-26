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

namespace Umbra.Markers;

public struct WorldMarker()
{
    /// <summary>
    /// The ID of the icon that is displayed on the marker.
    /// </summary>
    public uint IconId;

    /// <summary>
    /// The main label that is displayed below the marker.
    /// </summary>
    public string? Label;

    /// <summary>
    /// An optional sub-label that is displayed below the main label.
    /// </summary>
    public string? SubLabel;

    /// <summary>
    /// The position of the marker in the world.
    /// If the Y component is 0, the marker will be placed on the ground.
    /// </summary>
    public Vector3 Position;

    /// <summary>
    /// Whether the direction of the marker should be shown when the marker is
    /// outside the view frustum.
    /// </summary>
    public bool ShowDirection = true;

    /// <summary>
    /// The minimum opacity of the marker.
    /// </summary>
    public float MinOpacity = 0f;

    /// <summary>
    /// The maximum opacity of the marker.
    /// </summary>
    public float MaxOpacity = 1f;

    /// <summary>
    /// The distance at which the marker has faded out completely.
    /// </summary>
    public float MinFadeDistance = 0f;

    /// <summary>
    /// The distance at which the marker starts to fade out.
    /// </summary>
    public float MaxFadeDistance = 100f;

    /// <summary>
    /// The action that is executed when the marker is clicked.
    /// </summary>
    public Action? OnClick;
}
