/* Umbra.Game | (c) 2024 by Una         ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra.Game is free software: you can          \/     \/             \/
 *     redistribute it and/or modify it under the terms of the GNU Affero
 *     General Public License as published by the Free Software Foundation,
 *     either version 3 of the License, or (at your option) any later version.
 *
 *     Umbra.Game is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

namespace Umbra.Game.Societies;

public struct Society
{
    public uint   Id             { get; init; }
    public uint   ExpansionId    { get; init; }
    public string ExpansionName  { get; init; }
    public string Name           { get; init; }
    public uint   Rank           { get; init; }
    public byte   MaxRank        { get; init; }
    public string RankName       { get; init; }
    public uint   RankColor      { get; init; }
    public uint   IconId         { get; init; }
    public uint   CurrencyItemId { get; init; }
    public ushort CurrentRep     { get; init; }
    public ushort RequiredRep    { get; init; }
}
