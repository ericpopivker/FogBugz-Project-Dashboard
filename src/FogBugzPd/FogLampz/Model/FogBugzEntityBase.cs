// -----------------------------------------------------------------------
//  <copyright file="EntityBase.cs"
//             project="FogLampz"
//             assembly="FogLampz"
//             solution="FogLampz"
//             company="Chris Adams Studios">
//      Copyright (c) 1996+. All rights reserved.
//  </copyright>
//  <author id="chris@chrisadams-studios.com">Chris Adams</author>
//  <summary></summary>
// -----------------------------------------------------------------------

#region References

using System.Collections.Generic;
using FogLampz.Attributes;
using FogLampz.Util;

#endregion

namespace FogLampz.Model
{
    public abstract class FogBugzEntityBase : IFogBugzEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FogBugzEntityBase"/> class.
        /// </summary>
        protected FogBugzEntityBase()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FogBugzEntityBase"/> class.
        /// </summary>
        /// <param name="fields">The fields.</param>
        protected FogBugzEntityBase(IDictionary<string, string> fields)
        {
            Initialize(fields);
        }

        #region IFogBugzEntity Members

        /// <summary>
        ///   Initializes the object using the field dictionary.
        /// </summary>
        /// <param name="fields"> The fields. </param>
        public void Initialize(IDictionary<string, string> fields)
        {
            Utility.TryAssignProperty(this, fields);
        }

        /// <summary>
        ///   Gets the property values.
        /// </summary>
        /// <returns> An indexed dictionary of property names and values </returns>
        public IDictionary<string, string> GetPropertyValues()
        {
            return Utility.TryGatherProperties(this);
        }

        /// <summary>
        ///   Gets the API info.
        /// </summary>
        public EntityApiInfoAttribute ApiInfo
        {
            get { return Utility.GetApiInfo(this); }
        }

        #endregion
    }
}