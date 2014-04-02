// -----------------------------------------------------------------------
//  <copyright file="FixFor.cs"
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
    public sealed partial class FixFor
    {
        /// <summary>
        ///   Gets the project.
        /// </summary>
        public Project Project
        {
            get { return IndexProject.HasValue ? FogBugzClient.GetProject(IndexProject.Value) : null; }
        }
    }
}