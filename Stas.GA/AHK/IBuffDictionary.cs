﻿namespace Stas.GA; 

/// <summary>
///     Describes a set of buffs applied to the player
/// </summary>
public interface IBuffDictionary
{
    /// <summary>
    ///     Returns a buff description
    /// </summary>
    /// <param name="id">The buff id</param>
    IStatusEffect this[string id] { get; }

    /// <summary>
    ///     Checks whether the buff is present
    /// </summary>
    bool Has(string id);
}
