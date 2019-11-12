﻿using Heroes.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Heroes.Icons.DataReader
{
    /// <summary>
    /// Provides access to obtain unit data as well as updating localized strings.
    /// </summary>
    public class UnitDataReader : UnitBaseData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitDataReader"/> class.
        /// <see cref="Localization"/> will be inferred from the <paramref name="jsonDataFilePath"/>.
        /// </summary>
        /// <param name="jsonDataFilePath">JSON file containing unit data.</param>
        public UnitDataReader(string jsonDataFilePath)
            : base(jsonDataFilePath)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitDataReader"/> class.
        /// </summary>
        /// <param name="jsonDataFilePath">JSON file containing unit data.</param>
        /// <param name="localization">The localization of the file.</param>
        public UnitDataReader(string jsonDataFilePath, Localization localization)
            : base(jsonDataFilePath, localization)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitDataReader"/> class.
        /// </summary>
        /// <param name="jsonData">JSON data containing the unit data.</param>
        /// <param name="localization">The localization of the file.</param>
        public UnitDataReader(ReadOnlyMemory<byte> jsonData, Localization localization)
            : base(jsonData, localization)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitDataReader"/> class.
        /// The <paramref name="gameStringReader"/> overrides the <paramref name="jsonDataFilePath"/> <see cref="Localization"/>.
        /// </summary>
        /// <param name="jsonDataFilePath">JSON file containing unit data.</param>
        /// <param name="gameStringReader">Instance of a <see cref="GameStringReader"/>.</param>
        public UnitDataReader(string jsonDataFilePath, GameStringReader gameStringReader)
            : base(jsonDataFilePath, gameStringReader)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitDataReader"/> class.
        /// </summary>
        /// <param name="jsonData">JSON data containing the unit data.</param>
        /// <param name="gameStringReader">Instance of a <see cref="GameStringReader"/>.</param>
        public UnitDataReader(ReadOnlyMemory<byte> jsonData, GameStringReader gameStringReader)
            : base(jsonData, gameStringReader)
        {
        }

        /// <summary>
        /// Gets a <see cref="Unit"/> from the given unit <paramref name="id"/>.
        /// </summary>
        /// <param name="id">Unit id to find.</param>
        /// <param name="abilities">Value indicating to include abilities.</param>
        /// <param name="subAbilities">Value indicating to include sub-abilities.</param>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="KeyNotFoundException" />
        /// <returns></returns>
        public Unit GetUnitById(string id, bool abilities, bool subAbilities)
        {
            if (id is null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (TryGetUnitById(id, out Unit? value, abilities, subAbilities))
                return value;
            else
                throw new KeyNotFoundException();
        }

        /// <summary>
        /// Looks for a unit with the given <paramref name="id"/>, returning a value that indicates whether such value exists.
        /// </summary>
        /// <param name="id">Unit id to find.</param>
        /// <param name="value"></param>
        /// <param name="abilities">Value indicating to include abilities.</param>
        /// <param name="subAbilities">Value indicating to include sub-abilities.</param>
        /// <exception cref="ArgumentNullException" />
        /// <returns></returns>
        public bool TryGetUnitById(string id, [NotNullWhen(true)] out Unit? value, bool abilities, bool subAbilities)
        {
            if (id is null)
                throw new ArgumentNullException(nameof(id));

            value = null;

            if (JsonDataDocument.RootElement.TryGetProperty(id, out JsonElement element))
            {
                value = GetUnitData(id, element, abilities, subAbilities);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets a <see cref="Unit"/> from the given unit <paramref name="hyperlinkId"/>.
        /// </summary>
        /// <param name="hyperlinkId">Unit hyperlinkId to find.</param>
        /// <param name="abilities">Value indicating to include abilities.</param>
        /// <param name="subAbilities">Value indicating to include sub-abilities.</param>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="KeyNotFoundException" />
        /// <returns></returns>
        public Unit GetUnitByHyperlinkId(string hyperlinkId, bool abilities, bool subAbilities)
        {
            if (hyperlinkId is null)
            {
                throw new ArgumentNullException(nameof(hyperlinkId));
            }

            if (TryGetUnitByHyperlinkId(hyperlinkId, out Unit? value, abilities, subAbilities))
                return value;
            else
                throw new KeyNotFoundException();
        }

        /// <summary>
        /// Looks for a unit with the given <paramref name="hyperlinkId"/>, returning a value that indicates whether such value exists.
        /// </summary>
        /// <param name="hyperlinkId">Unit hyperlinkId to find.</param>
        /// <param name="value"></param>
        /// <param name="abilities">Value indicating to include abilities.</param>
        /// <param name="subAbilities">Value indicating to include sub-abilities.</param>
        /// <exception cref="ArgumentNullException" />
        /// <returns></returns>
        public bool TryGetUnitByHyperlinkId(string hyperlinkId, [NotNullWhen(true)] out Unit? value, bool abilities, bool subAbilities)
            => PropertyLookup("hyperlinkId", hyperlinkId, out value, abilities, subAbilities);

        /// <summary>
        /// Gets a collection of all units.
        /// </summary>
        /// <param name="abilities">Value indicating to include abilities.</param>
        /// <param name="subAbilities">Value indicating to include sub-abilities.</param>
        /// <returns></returns>
        public IEnumerable<Unit> GetUnits(bool abilities, bool subAbilities)
        {
            List<Unit> unitList = new List<Unit>();

            foreach (JsonProperty unit in JsonDataDocument.RootElement.EnumerateObject())
            {
                unitList.Add(GetUnitById(unit.Name, abilities, subAbilities));
            }

            return unitList;
        }

        private Unit GetUnitData(string id, JsonElement element, bool includeAbilities, bool includeSubAbilities)
        {
            Unit unit = new Unit
            {
                Id = id,
                CUnitId = id,
            };

            if (element.TryGetProperty("hyperlinkId", out JsonElement value))
                unit.HyperlinkId = value.GetString();

            int index = id.IndexOf('-');
            if (index > -1)
            {
                unit.MapName = id.Substring(0, index);
            }

            if (element.TryGetProperty("name", out value))
                unit.Name = value.GetString();
            if (element.TryGetProperty("innerRadius", out value))
                unit.InnerRadius = value.GetDouble();
            if (element.TryGetProperty("radius", out value))
                unit.Radius = value.GetDouble();
            if (element.TryGetProperty("sight", out value))
                unit.Sight = value.GetDouble();
            if (element.TryGetProperty("speed", out value))
                unit.Speed = value.GetDouble();
            if (element.TryGetProperty("killXP", out value))
                unit.KillXP = value.GetInt32();
            if (element.TryGetProperty("damageType", out value))
                unit.DamageType = value.GetString();
            if (element.TryGetProperty("scalingLinkId", out value))
                unit.ScalingBehaviorLink = value.GetString();
            if (element.TryGetProperty("description", out value))
                unit.Description = new TooltipDescription(value.GetString(), Localization);

            if (element.TryGetProperty("descriptors", out value))
            {
                foreach (JsonElement descriptorArrayElement in value.EnumerateArray())
                    unit.AddHeroDescriptor(descriptorArrayElement.GetString());
            }

            if (element.TryGetProperty("attributes", out value))
            {
                foreach (JsonElement attributeArrayElement in value.EnumerateArray())
                    unit.AddAttribute(attributeArrayElement.GetString());
            }

            if (element.TryGetProperty("units", out value))
            {
                foreach (JsonElement unitArrayElement in value.EnumerateArray())
                    unit.AddUnitId(unitArrayElement.GetString());
            }

            // portraits
            if (element.TryGetProperty("portraits", out value))
            {
                if (value.TryGetProperty("targetInfo", out JsonElement portraitValue))
                    unit.UnitPortrait.TargetInfoPanelFileName = portraitValue.GetString();
                if (value.TryGetProperty("minimap", out portraitValue))
                    unit.UnitPortrait.MiniMapIconFileName = portraitValue.GetString();
            }

            // life
            SetUnitLife(element, unit);

            // shield
            SetUnitShield(element, unit);

            // energy
            SetUnitEnergy(element, unit);

            // armor
            SetUnitArmor(element, unit);

            // weapons
            SetUnitWeapons(element, unit);

            // abilities
            if (includeAbilities && element.TryGetProperty("abilities", out JsonElement abilities))
            {
                AddAbilities(unit, abilities);
            }

            if (includeSubAbilities && element.TryGetProperty("subAbilities", out JsonElement subAbilities))
            {
                foreach (JsonElement subAbilityArrayElement in subAbilities.EnumerateArray())
                {
                    foreach (JsonProperty subAbilityProperty in subAbilityArrayElement.EnumerateObject())
                    {
                        string parentLink = subAbilityProperty.Name;

                        AddAbilities(unit, subAbilityProperty.Value, parentLink);
                    }
                }
            }

            GameStringReader?.UpdateGameStrings(unit);

            return unit;
        }

        private bool PropertyLookup(string propertyId, string propertyValue, [NotNullWhen(true)] out Unit? value, bool abilities, bool subAbilities)
        {
            if (propertyValue is null)
                throw new ArgumentNullException(nameof(propertyValue));

            value = null;

            foreach (JsonProperty heroProperty in JsonDataDocument.RootElement.EnumerateObject())
            {
                if (heroProperty.Value.TryGetProperty(propertyId, out JsonElement nameElement) && nameElement.ValueEquals(propertyValue))
                {
                    value = GetUnitData(heroProperty.Name, heroProperty.Value, abilities, subAbilities);

                    return true;
                }
            }

            return false;
        }
    }
}