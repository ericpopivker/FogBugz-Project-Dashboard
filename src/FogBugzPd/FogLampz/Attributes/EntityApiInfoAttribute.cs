// -----------------------------------------------------------------------
//  <copyright file="EntityApiInfoAttribute.cs"
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

using System;
using System.Diagnostics;
using System.Globalization;

#endregion

namespace FogLampz.Attributes
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class EntityApiInfoAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the create command.
        /// </summary>
        /// <value>
        /// The create command.
        /// </value>
        public string CreateCommand { get; set; }

        /// <summary>
        ///   Gets or sets the command.
        /// </summary>
        /// <value> The command. </value>
        public string ListCommand { get; set; }

        /// <summary>
        ///   Gets or sets the root.
        /// </summary>
        /// <value> The root. </value>
        public string Root { get; set; }

        /// <summary>
        ///   Gets or sets the element.
        /// </summary>
        /// <value> The element. </value>
        public string Element { get; set; }

        /// <summary>
        /// Gets the debugger display string.
        /// </summary>
        private string DebuggerDisplay
        {
            get { return string.Format(CultureInfo.InvariantCulture, "<{0}><{1} /></{0}>", Root, Element); }
        }
    }
}