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

using Umbra.Interface;

namespace Umbra.Toolbar;

internal interface IToolbarWidget
{
    /// <summary>
    /// Returns the widget element. The configured anchor determines where the
    /// widget is placed on the toolbar and may be changed during the OnDraw
    /// method.
    /// </summary>
    public Element Element { get; }

    /// <summary>
    /// Invoked on every framework tick. This method runs in the game's main
    /// thread.
    /// </summary>
    public void OnUpdate();

    /// <summary>
    /// Invoked on every frame just before the widget is drawn.
    /// </summary>
    public void OnDraw();
}
