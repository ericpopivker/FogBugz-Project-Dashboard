// -----------------------------------------------------------------------
//  <copyright file="Person.cs"
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
    public sealed partial class Person
    {
        /// <summary>
        ///   Gets the virtual closed user.
        /// </summary>
        public static Person ClosedUser
        {
            get { return new Person { Index = 1, Name = "*Closed", Virtual = true }; }
        }
    }
}