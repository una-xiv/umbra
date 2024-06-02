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

namespace Umbra.Widgets;

internal partial class WeatherWidgetPopup
{
    protected sealed override Node Node { get; } = new() {
        Stylesheet = PopupStylesheet,
        ClassList  = ["popup"],
        ChildNodes = [
            new() {
                Id        = "Background",
                ClassList = ["popup-gradient"],
                BeforeReflow = node => {
                    Size size = node.ParentNode!.Bounds.PaddingSize;

                    node.Bounds.PaddingSize = node.Bounds.ContentSize = new(
                        size.Width,
                        size.Height - node.ComputedStyle.Margin.Top
                    );

                    node.Bounds.MarginSize = node.Bounds.PaddingSize + node.ComputedStyle.Margin.Size;

                    return true;
                }
            },
            new() {
                Id = "Line",
                Style = new() {
                    Anchor          = Anchor.BottomLeft,
                    Size            = new(1, 300),
                    BackgroundColor = new("Widget.PopupBorder"),
                    Margin          = new() { Left = 41, Bottom = 24 },
                }
            },
            new() {
                ClassList = ["header"],
                ChildNodes = [
                    new() {
                        ClassList = ["header-icon"],
                    },
                    new() {
                        ClassList = ["header-text"],
                        ChildNodes = [
                            new() {
                                ClassList = ["header-text--title"],
                                NodeValue = "Weather",
                            },
                            new() {
                                ClassList = ["header-text--subtitle"],
                                NodeValue = "Weather sub title here",
                            }
                        ]
                    }
                ]
            },
            new() {
                ClassList = ["body"],
                ChildNodes = [
                    CreateWeatherForecastNode(1),
                    CreateWeatherForecastNode(2),
                    CreateWeatherForecastNode(3),
                    CreateWeatherForecastNode(4),
                    CreateWeatherForecastNode(5),
                    CreateWeatherForecastNode(6),
                    CreateWeatherForecastNode(7),
                    CreateWeatherForecastNode(8),
                ]
            }
        ]
    };

    private static Node CreateWeatherForecastNode(int id)
    {
        return new() {
            Id        = $"ForecastItem{id}",
            ClassList = ["forecast-item"],
            ChildNodes = [
                new() {
                    ClassList = ["forecast-item--icon"],
                },
                new() {
                    ClassList = ["forecast-item--text"],
                    ChildNodes = [
                        new() {
                            ClassList = ["forecast-item--text--name"],
                            NodeValue = "Weather Type Name",
                        },
                        new() {
                            ClassList = ["forecast-item--text--time"],
                            NodeValue = "In 5 hours and 42 minutes",
                        }
                    ]
                }
            ]
        };
    }
}
