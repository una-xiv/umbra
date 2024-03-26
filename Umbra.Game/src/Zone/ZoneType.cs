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

 namespace Umbra.Game;

// Borrowed from https://github.com/Xpahtalo/XpahtaLib
// These values are represented via TerritoryType.TerritoryIntededUse.
public enum TerritoryType : byte
{
    MainCity = 0,
    SharedOverworld = 1,
    InnRoom = 2,
    DungeonsAndGuildhests = 3,
    VariantDungeon = 4,
    MordionGaol = 5,
    NewCharacterCity = 6,
    SharedWaitingRoom = 7,
    AllianceRaid = 8,
    PreEwOverworldQuestBattle = 9,
    Trial = 10,
    CurrentlyUnused1 = 11,
    PostDutyRoom = 12,
    HousingWard = 13,
    HousingInstance = 14,
    SoloOverworldInstances = 15,
    Raid1 = 16,
    Raid2 = 17,
    Frontlines = 18,
    CurrentlyUnused2 = 19,
    ChocoboRacing = 20,
    TheFirmament = 21,
    SanctumOfTheTwelve = 22,
    TheGoldSaucer = 23,
    CurrentlyUnused3 = 24,
    LordOfVerminion = 25,
    CurrentlyUnused4 = 26,
    HallOfTheNovice = 27,
    SmallScalePvp = 28,
    SoloDuty = 29,
    GrandCompanyBarracks = 30,
    DeepDungeon = 31,
    HolidayInstance = 32,
    TimewornTreasureMap = 33,
    HolidayDuty = 34,
    TripleTriadBattlehall = 35,
    CurrentlyUnused5 = 36,
    PvpCustomMatch = 37,
    CurrentlyUnused6 = 38,
    RivalWings = 39,
    HolidayEventRoom = 40,
    Eureka = 41,
    CrystalTowerTrainingGrounds = 42,
    CurrentlyUnused7 = 43,
    LeapOfFaith = 44,
    MaskedCarnivale = 45,
    OceanFishing = 46,
    CurrentlyUnused8 = 47,
    Bozja = 48,
    IslandSanctuary = 49,
    TripleTriadTournament = 50,
    TripleTriadParlor = 51,
    DelebrumReginae = 52,
    DelebrumReginaeSavage = 53,
    EndwalkerMsqSoloOverworld = 54,
    CurrentlyUnused9 = 55,
    TribalGathering = 56,
    CriterionDungeon = 57,
    CriterionDungeonSavage = 58,
    Blunderville = 59,
    CurrentlyUnused10 = 60,
}
