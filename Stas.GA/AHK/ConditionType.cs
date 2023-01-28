﻿
namespace Stas.GA;

/// <summary>
///     Conditions supported by this plugin.
/// </summary>
public enum ConditionType {
    /// <summary>
    ///     Condition based on player Vitals.
    /// </summary>
    VITALS,

    /// <summary>
    ///     Condition based on what player is doing.
    /// </summary>
    ANIMATION,

    /// <summary>
    ///     Condition based on player Buffs/Debuffs.
    /// </summary>
    STATUS_EFFECT,

    /// <summary>
    ///     Condition based on flask mod active on player or not.
    /// </summary>
    FLASK_EFFECT,

    /// <summary>
    ///     Condition based on number of charges flask has.
    /// </summary>
    FLASK_CHARGES,

    /// <summary>
    ///     Condition based on Ailment on the player.
    /// </summary>
    AILMENT

    ///// <summary>
    /////     Condition base on user code
    ///// </summary>
    //DYNAMIC
}
