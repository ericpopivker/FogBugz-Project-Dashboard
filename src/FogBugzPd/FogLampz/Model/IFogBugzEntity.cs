// -----------------------------------------------------------------------
//  <copyright file="IFogBugzEntity.cs"
//             project="FogLampz"
//             assembly="FogLampz"
//             solution="FogLampz"
//             company="Chris Adams Studios">
//      Copyright (c) 1996+. All rights reserved.
//  </copyright>
//  <author id="chris@chrisadams-studios.com">Chris Adams</author>
//  <summary></summary>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using FogLampz.Attributes;

namespace FogLampz.Model
{
    /// <summary>
    /// Represents an entity available in the FogBugz client model
    /// </summary>
    public interface IFogBugzEntity
    {
        /// <summary>
        /// Initializes the object using the field dictionary.
        /// </summary>
        /// <param name="fields">The fields.</param>
        void Initialize(IDictionary<string, string> fields);

        /// <summary>
        /// Gets the property values.
        /// </summary>
        /// <returns>An indexed dictionary of property names and values</returns>
        IDictionary<string, string> GetPropertyValues();

        /// <summary>
        /// Gets the API info.
        /// </summary>
        EntityApiInfoAttribute ApiInfo { get; }
    }
}