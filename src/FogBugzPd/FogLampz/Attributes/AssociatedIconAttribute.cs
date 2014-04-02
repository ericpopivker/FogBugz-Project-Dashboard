// -----------------------------------------------------------------------
//  <copyright file="AssociatedIcon.cs"
//             project="FogLampz"
//             assembly="FogLampz"
//             solution="FogLampz"
//             company="Chris Adams Studios">
//      Copyright (c) 1996+. All rights reserved.
//  </copyright>
//  <author id="chris@chrisadams-studios.com">Chris Adams</author>
//  <summary></summary>
// -----------------------------------------------------------------------

using System;

namespace FogLampz.Attributes
{
    /// <summary>
    ///   Allows the attachment of an icon to an enumeration
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class AssociatedIconAttribute : Attribute
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="AssociatedIconAttribute" /> class.
        /// </summary>
        /// <param name="name"> The name. </param>
        public AssociatedIconAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        ///   Gets the name of the icon.
        /// </summary>
        /// <value> The name of the icon. </value>
        public string Name { get; private set; }
    }
}