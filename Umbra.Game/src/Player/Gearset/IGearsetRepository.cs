﻿using System;
using System.Collections.Generic;

namespace Umbra.Game;

public interface IGearsetRepository
{
    /// <summary>
    /// Invoked when a new gearset is created.
    /// </summary>
    public event Action<Gearset>? OnGearsetCreated;

    /// <summary>
    /// Invoked when the data of a gearset has been modified.
    /// </summary>
    public event Action<Gearset>? OnGearsetChanged;

    /// <summary>
    /// Invoked when a gearset has been removed.
    /// </summary>
    public event Action<Gearset>? OnGearsetRemoved;

    /// <summary>
    /// Invoked when a gearset has been equipped.
    /// </summary>
    public event Action<Gearset>? OnGearsetEquipped;

    /// <summary>
    /// A reference to the currently equipped gearset.
    /// </summary>
    public Gearset? CurrentGearset { get; }

    /// <summary>
    /// Returns a list of valid gearsets.
    /// </summary>
    /// <returns></returns>
    public List<Gearset> GetGearsets();

    /// <summary>
    /// Equips the gearset with the given ID.
    /// </summary>
    /// <param name="id"></param>
    public void EquipGearset(ushort id);

    /// <summary>
    /// Opens the glamour selection window that allows linking
    /// a glamour plate to the given gearset.
    /// </summary>
    public void OpenGlamourSetLinkWindow(Gearset gearset);

    /// <summary>
    /// Opens the portrait (banner) editor window for the given gearset.
    /// </summary>
    /// <param name="gs"></param>
    public void OpenPortraitEditorForGearset(Gearset gs);

    /// <summary>
    /// Unlinks a linked glamour set from the given gearset.
    /// </summary>
    public void UnlinkGlamourSet(Gearset gearset);

    /// <summary>
    /// Find the previous gearset ID in the same category.
    /// </summary>
    /// <param name="gearset"></param>
    /// <returns></returns>
    public ushort? FindPrevIdInCategory(Gearset gearset);

    /// <summary>
    /// Find the next gearset ID in the same category.
    /// </summary>
    /// <param name="gearset"></param>
    /// <returns></returns>
    public ushort? FindNextIdInCategory(Gearset gearset);

    /// <summary>
    /// Duplicates the currently equipped gearset.
    /// </summary>
    public void DuplicateEquippedGearset();

    /// <summary>
    /// Equips a random DoW/DoM job and returns the gearset.
    /// </summary>
    public Gearset? EquipRandomJob();

    /// <summary>
    /// Updates the currently equipped gearset.
    /// </summary>
    public void UpdateEquippedGearset();

    /// <summary>
    /// Deletes the given gearset. Will show a confirmation dialog.
    /// </summary>
    public void DeleteGearset(Gearset gearset);

    /// <summary>
    /// Moves the gearset one slot up within the same category.
    /// </summary>
    public void MoveGearsetUp(Gearset gearset);

    /// <summary>
    /// Moves the gearset one slot down within the same category.
    /// </summary>
    public void MoveGearsetDown(Gearset gearset);
}
