/* Umbra.Interface | (c) 2024 by Una    ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra.Interface is free software: you can    \/     \/             \/
 *     redistribute it and/or modify it under the terms of the GNU Affero
 *     General Public License as published by the Free Software Foundation,
 *     either version 3 of the License, or (at your option) any later version.
 *
 *     Umbra.Interface is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

using System;
using EasingFunctions = Dalamud.Interface.Animation.EasingFunctions;

namespace Umbra.Interface;

// Facade for parameterless Dalamud easing function to be used as a generic
// type argument for the Umbra.Animation<T> class.

public class InQuint() : EasingFunctions.InQuint(TimeSpan.Zero), IEasingFunction;
public class OutQuint() : EasingFunctions.OutQuint(TimeSpan.Zero), IEasingFunction;
public class InOutQuint() : EasingFunctions.InOutQuint(TimeSpan.Zero), IEasingFunction;
public class InCirc() : EasingFunctions.InCirc(TimeSpan.Zero), IEasingFunction;
public class OutCirc() : EasingFunctions.OutCirc(TimeSpan.Zero), IEasingFunction;
public class InOutCirc() : EasingFunctions.InOutCirc(TimeSpan.Zero), IEasingFunction;
public class InCubic() : EasingFunctions.InCubic(TimeSpan.Zero), IEasingFunction;
public class OutCubic() : EasingFunctions.OutCubic(TimeSpan.Zero), IEasingFunction;
public class InOutCubic() : EasingFunctions.InOutCubic(TimeSpan.Zero), IEasingFunction;
public class InSine() : EasingFunctions.InSine(TimeSpan.Zero), IEasingFunction;
public class OutSine() : EasingFunctions.OutSine(TimeSpan.Zero), IEasingFunction;
public class InOutSine() : EasingFunctions.InOutSine(TimeSpan.Zero), IEasingFunction;
public class InElastic() : EasingFunctions.InElastic(TimeSpan.Zero), IEasingFunction;
public class OutElastic() : EasingFunctions.OutElastic(TimeSpan.Zero), IEasingFunction;
public class InOutElastic() : EasingFunctions.InOutElastic(TimeSpan.Zero), IEasingFunction;

public interface IEasingFunction
{
    public TimeSpan Duration { get; set; }
    public double Value { get; }

    public bool IsRunning { get; }
    public bool IsDone { get; }

    public void Start();
    public void Stop();
    public void Reset();
    public void Update();
}
