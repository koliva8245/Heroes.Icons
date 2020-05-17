﻿using Heroes.Models;
using Heroes.Models.AbilityTalents;

namespace Heroes.Icons.Extensions
{
    /// <summary>
    /// Contains extensions for <see cref="Ability"/>.
    /// </summary>
    public static class AbilityExtensions
    {
        /// <summary>
        /// Updates the localized gamestrings to the selected <see cref="Localization"/>.
        /// </summary>
        /// <param name="ability"></param>
        /// <param name="gameStringDocument"></param>
        public static void UpdateGameStrings(this Ability ability, GameStringDocument gameStringDocument)
        {
            gameStringDocument.UpdateGameStrings(ability);
        }
    }
}
