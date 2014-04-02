// -----------------------------------------------------------------------
//  <copyright file="PropertyMapAttribute.cs"
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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

#endregion

namespace FogLampz.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class PropertyMapAttribute : Attribute
    {
        #region ConversionStrategy enum

        #endregion

        /// <summary>
        ///   Initializes a new instance of the <see cref="PropertyMapAttribute" /> class.
        /// </summary>
        /// <param name="index"> The index. </param>
        public PropertyMapAttribute(string index) : this(index, ConversionStrategy.String)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="PropertyMapAttribute" /> class.
        /// </summary>
        /// <param name="index"> The index. </param>
        /// <param name="strategy"> The strategy. </param>
        public PropertyMapAttribute(string index, ConversionStrategy strategy)
        {
            Index = index;
            Strategy = strategy;
        }

        /// <summary>
        ///   Gets or sets the strategy.
        /// </summary>
        /// <value> The strategy. </value>
        public ConversionStrategy Strategy { get; private set; }

        /// <summary>
        ///   Gets the index.
        /// </summary>
        public string Index { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this value should not be serialized during property value retrieval.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this value is ignored during serialization; otherwise, <c>false</c>.
        /// </value>
        public bool NotSerialized { get; set; }

        /// <summary>
        ///   Converts the specified value.
        /// </summary>
        /// <param name="value"> The value. </param>
        /// <returns> </returns>
        public object Convert(string value)
        {
            switch (Strategy)
            {
                case ConversionStrategy.Integer:
                    if (string.IsNullOrEmpty(value))
                        return null;
                    int i;
                    return Int32.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out i) && i > 0 ? (object)i : null;

                case ConversionStrategy.IntegerList:
                    {
                        if (string.IsNullOrEmpty(value))
                            return default(List<int>);

                        var values = value.Split(new[] { ',', ';' });
                        var list = new List<int>();
                        foreach (var val in values)
                        {
                            int v;
                            if (Int32.TryParse(val, NumberStyles.Integer, CultureInfo.InvariantCulture, out v) && v > 0)
                                list.Add(v);
                        }
                        return list;
                    }

                case ConversionStrategy.Boolean:
                    return !string.IsNullOrEmpty(value) && Boolean.Parse(value);

                case ConversionStrategy.DateTime:
                    return string.IsNullOrEmpty(value)
                               ? default(DateTime?)
                               : DateTime.ParseExact(value, "yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToLocalTime();
                case ConversionStrategy.StringList:
                    {
                        return string.IsNullOrEmpty(value)
                                   ? default(List<string>)
                                   : value.Split(new[] { ',', ';' }).ToList();
                    }
				case ConversionStrategy.Decimal:
					if (string.IsNullOrEmpty(value))
						return null;
					decimal d;
					return Decimal.TryParse(value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out d) && d > 0 ? (object)d : null;

                default:
                    return value;
            }
        }
    }
}