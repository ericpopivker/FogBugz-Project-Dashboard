// -----------------------------------------------------------------------
//  <copyright file="CategoryIcon.cs"
//             project="FogLampz"
//             assembly="FogLampz"
//             solution="FogLampz"
//             company="Chris Adams Studios">
//      Copyright (c) 1996+. All rights reserved.
//  </copyright>
//  <author id="chris@chrisadams-studios.com">Chris Adams</author>
//  <summary></summary>
// -----------------------------------------------------------------------

using FogLampz.Attributes;

namespace FogLampz.Model
{
    public enum CategoryIcon
    {
        [AssociatedIcon("none")]
        None = 0,
        [AssociatedIcon("connections")]
        Bug = 1,
        [AssociatedIcon("connected")]
        BugError = 5,
        [AssociatedIcon("fire")]
        Error = 9,
        [AssociatedIcon("lab")]
        Feature = 2,
        [AssociatedIcon("email")]
        Inquiry = 3,
        [AssociatedIcon("unlock")]
        Key = 8,
        [AssociatedIcon("search")]
        Magnifier = 7,
        [AssociatedIcon("arrow_round")]
        ScheduleItem = 4,
        [AssociatedIcon("wrench")]
        Wrench = 6
    }
}