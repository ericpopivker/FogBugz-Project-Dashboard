// -----------------------------------------------------------------------
//  <copyright file="Project.cs"
//             project="FogLampz"
//             assembly="FogLampz"
//             solution="FogLampz"
//             company="Chris Adams Studios">
//      Copyright (c) 1996+. All rights reserved.
//  </copyright>
//  <author id="chris@chrisadams-studios.com">Chris Adams</author>
//  <summary></summary>
// -----------------------------------------------------------------------

namespace FogLampz.Model
{
    public sealed partial class Project
    {
        /// <summary>
        ///   Gets the owner.
        /// </summary>
        public Person Owner
        {
            get { return IndexOwner.HasValue ? FogBugzClient.GetPerson(IndexOwner.Value) : null; }
        }
    }
}