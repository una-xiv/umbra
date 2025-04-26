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

using Una.Drawing;

namespace Umbra.Windows.Oobe;

public interface IOobeStep
{
    /// <summary>
    /// The title of this step, shown in the window header.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// A short description of the step, shown in the window header.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// True if the user is allowed to proceed to the next step.
    /// </summary>
    public bool CanContinue { get; set; }

    /// <summary>
    /// The node to render for this step.
    /// </summary>
    public Node Node { get; }

    /// <summary>
    /// Invoked when the step has been committed. This means the user has
    /// clicked the "Next" or "Finish" button.
    /// </summary>
    public void OnCommit();
}
