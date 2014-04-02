// -----------------------------------------------------------------------
//  <copyright file="Case.cs"
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
    public sealed partial class Case
    {
        /// <summary>
        ///   Gets the parent.
        /// </summary>
        public Case Parent
        {
            get { return IndexParent.HasValue ? FogBugzClient.GetCase(IndexParent.Value) : null; }
        }

        /// <summary>
        ///   Gets the project.
        /// </summary>
        public Project Project
        {
            get { return IndexProject.HasValue ? FogBugzClient.GetProject(IndexProject.Value) : null; }
        }

        /// <summary>
        ///   Gets the area.
        /// </summary>
        public Area Area
        {
            get { return IndexArea.HasValue ? FogBugzClient.GetArea(IndexArea.Value) : null; }
        }

        /// <summary>
        ///   Gets the assigned to.
        /// </summary>
        public Person AssignedTo
        {
            get { return IndexPersonAssignedTo.HasValue ? FogBugzClient.GetPerson(IndexPersonAssignedTo.Value) : null; }
        }

        /// <summary>
        ///   Gets the opened by.
        /// </summary>
        public Person OpenedBy
        {
            get { return IndexPersonOpenedBy.HasValue ? FogBugzClient.GetPerson(IndexPersonOpenedBy.Value) : null; }
        }

        /// <summary>
        ///   Gets the resolved by.
        /// </summary>
        public Person ResolvedBy
        {
            get { return IndexPersonResolvedBy.HasValue ? FogBugzClient.GetPerson(IndexPersonResolvedBy.Value) : null; }
        }

        /// <summary>
        ///   Gets the closed by.
        /// </summary>
        public Person ClosedBy
        {
            get { return IndexPersonClosedBy.HasValue ? FogBugzClient.GetPerson(IndexPersonClosedBy.Value) : null; }
        }

        /// <summary>
        ///   Gets the last edited by.
        /// </summary>
        public Person LastEditedBy
        {
            get { return IndexPersonLastEditedBy.HasValue ? FogBugzClient.GetPerson(IndexPersonLastEditedBy.Value) : null; }
        }

        /// <summary>
        ///   Gets the priority.
        /// </summary>
        public Priority Priority
        {
            get { return IndexPriority.HasValue ? FogBugzClient.GetPriority(IndexPriority.Value) : null; }
        }

        /// <summary>
        ///   Gets the category.
        /// </summary>
        public Category Category
        {
            get { return IndexCategory.HasValue ? FogBugzClient.GetCategory(IndexCategory.Value) : null; }
        }
    }
}