// -----------------------------------------------------------------------
//  <copyright file="Status.cs"
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
    public sealed partial class Status
    {
        /// <summary>
        ///   Gets the category.
        /// </summary>
        public Category Category
        {
            get { return IndexCategory.HasValue ? FogBugzClient.GetCategory(IndexCategory.Value) : null; }
        }
    }
}