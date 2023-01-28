﻿namespace Stas.GA;
/// <summary>
///     Different type of player Vitals.
/// </summary>
public enum VitalType
{
    /// <summary>
    ///     Condition based on player Mana.
    /// </summary>
    MANA,

    /// <summary>
    ///     Condition based on player Mana.
    /// </summary>
    MANA_PERCENT,

    /// <summary>
    ///     Condition based on player Life.
    /// </summary>
    LIFE,

    /// <summary>
    ///     Condition based on player Life.
    /// </summary>
    LIFE_PERCENT,

    /// <summary>
    ///     Condition based on player Energy Shield.
    /// </summary>
    ENERGYSHIELD,

    /// <summary>
    ///     Condition based on player Energy Shield.
    /// </summary>
    ENERGYSHIELD_PERCENT
}
