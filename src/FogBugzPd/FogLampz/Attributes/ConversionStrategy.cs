// -----------------------------------------------------------------------
//  <copyright file="ConversionStrategy.cs"
//             project="FogLampz"
//             assembly="FogLampz"
//             solution="FogLampz"
//             company="Chris Adams Studios">
//      Copyright (c) 1996+. All rights reserved.
//  </copyright>
//  <author id="chris@chrisadams-studios.com">Chris Adams</author>
//  <summary></summary>
// -----------------------------------------------------------------------

namespace FogLampz.Attributes
{
    /// <summary>
    ///   Defines the strategy for converting from the original string value into the desired type for best results the resultant type should be nullable
    /// </summary>
    public enum ConversionStrategy
    {
        /// <summary>
        ///   Default, returns the original string without conversion
        /// </summary>
        String,

        /// <summary>
        ///   separates the value on comma and semicolon, returns a List{string}
        /// </summary>
        StringList,

        /// <summary>
        ///   converts ton Int32?
        /// </summary>
        Integer,

        /// <summary>
        ///   separates the value on comma and semicolon, returns a List{Int32}
        /// </summary>
        IntegerList,

        /// <summary>
        ///   Converts to Boolean
        /// </summary>
        Boolean,

        /// <summary>
        ///   Converts to DateTime?
        /// </summary>
        DateTime,


		Decimal
    }
}