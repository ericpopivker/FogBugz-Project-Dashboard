// -----------------------------------------------------------------------
//  <copyright file="Utility.cs"
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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using FogLampz.Attributes;
using FogLampz.Exceptions;
using FogLampz.Model;

#endregion

namespace FogLampz.Util
{
    internal static class Utility
    {
        /// <summary>
        ///   Tries the assign property values in the selected instance using the mapping values in the passed dictionary. Propertes that are not decorated with a <see
        ///    cref="PropertyMapAttribute" /> are ignored.
        /// </summary>
        /// <param name="instance"> The instance. </param>
        /// <param name="fieldMapping"> The field mapping. </param>
        public static void TryAssignProperty(object instance, IDictionary<string, string> fieldMapping)
        {
            var props = instance.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            foreach (var prop in props)
            {
                var maps = prop.GetCustomAttributes(typeof (PropertyMapAttribute), false);
                if (maps.Length <= 0)
                    continue;

                var map = (PropertyMapAttribute) maps[0];

                foreach (var field in fieldMapping.Keys.Where(field => map.Index.ToUpperInvariant() == field.ToUpperInvariant()))
                {
                    prop.SetValue(instance, map.Convert(fieldMapping[field]), null);
                }
            }
        }

        /// <summary>
        ///   Reflects the properties in the referenced entity that contain a <see cref="PropertyMapAttribute" /> . Loads a dictionary containing the name and value of the mapped entites.
        /// </summary>
        /// <param name="entity"> The entity. </param>
        /// <returns> the name/value pairs of the reflected properties. </returns>
        public static IDictionary<string, string> TryGatherProperties(IFogBugzEntity entity)
        {
            var dictionary = new Dictionary<string, string>();

            var props = entity.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            foreach (var prop in props)
            {
                var maps = prop.GetCustomAttributes(typeof (PropertyMapAttribute), false);
                if (maps.Length <= 0)
                    continue;

                var map = (PropertyMapAttribute) maps[0];

                if (map.NotSerialized)
                    continue;

                if (dictionary.ContainsKey(map.Index))
                    dictionary.Remove(map.Index);

                var value = prop.GetValue(entity, null);

                if (value != null)
                    dictionary.Add(map.Index, GetValueFromStrategy(value, map.Strategy));
            }
            return dictionary;
        }

        /// <summary>
        ///   Gets the value from strategy.
        /// </summary>
        /// <param name="value"> The value. </param>
        /// <param name="strategy"> The strategy. </param>
        /// <returns> </returns>
        private static string GetValueFromStrategy(object value, ConversionStrategy strategy)
        {
            switch (strategy)
            {
                case ConversionStrategy.StringList:
                    return string.Join(",", ((List<string>) value));
                default:
                    return value.ToString();
            }
        }

        /// <summary>
        ///   Gets the API info.
        /// </summary>
        /// <param name="entity"> The entity. </param>
        /// <returns> </returns>
        public static EntityApiInfoAttribute GetApiInfo(IFogBugzEntity entity)
        {
            var attributes = entity.GetType().GetCustomAttributes(typeof (EntityApiInfoAttribute), false);
            if (attributes.Length <= 0)
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Entity {0} does not implement API attribute", entity.GetType().FullName));
            return (EntityApiInfoAttribute) attributes[0];
        }

        /// <summary>
        ///   Extends the <see cref="System.String" /> class to return an xml reader from the current string.
        /// </summary>
        /// <param name="xml"> The XML. </param>
        /// <exception cref="InvalidOperationException">Thrown when the
        ///   <paramref name="xml" />
        ///   parameter is not valid XML.</exception>
        /// <returns> An xml reader. </returns>
        public static XmlReader GetXmlReader(this string xml)
        {
            try
            {
				//var settings = new XmlReaderSettings
				//                   {
				//                       IgnoreComments = true,
				//                       IgnoreWhitespace = true,
				//                       ConformanceLevel = ConformanceLevel.Fragment,
				//                       ValidationType = ValidationType.None
				//                   };

                using (var stringReader = new StringReader(xml.Trim()))
                {
                    var reader = XmlReader.Create(stringReader);//, settings);
                    reader.MoveToContent();
                    return reader;
                }
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Unable to create XML reader from selected string value", e);
            }
        }

        /// <summary>
        ///   Gets the sub element values from the xml reader and returns a dictionary.
        /// </summary>
        /// <param name="reader"> The reader. </param>
        /// <returns> the dictionary of index names and values. </returns>
        public static IDictionary<string, string> GetSubElementValues(this XmlReader reader)
        {
            var dictionary = new Dictionary<string, string>();

            if (reader.NodeType != XmlNodeType.Element)
                throw new InvalidOperationException("XML reader was not positioned on a valid element");

            var name = reader.Name;

            #region Case entity special cases

            //cases entity contains operations in an attribute not as an element collection
            if (name == "case")
            {
                dictionary.Add("Operations", reader.GetAttribute("operations"));
            }

            #endregion

            #region Filter entity special cases

            //filters contain values in attributes, not elements.  element content is the filter name
            if (name == "filter")
            {
                dictionary.Add("type", reader.GetAttribute("type"));
                dictionary.Add("sFilter", reader.GetAttribute("sFilter"));
                var status = reader.GetAttribute("status");
                if (!string.IsNullOrEmpty(status))
                    dictionary.Add("status", status);

                dictionary.Add("sName", reader.ReadElementContentAsString());
                return dictionary;
            }

            #endregion

            reader.Read();

            var tagList = new List<string>();
            while (!reader.EOF)
            {
                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "tags")
                {
                    reader.Read();
                    dictionary.Add("sTags", string.Join(",", tagList));
                }
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "tags")
                {
                    reader.Read();
                }
                else if (reader.NodeType == XmlNodeType.Element && reader.Name == "tag")
                {
                    reader.Read();
                    tagList.Add(reader.ReadContentAsString());
                }
                else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "tag")
                {
                    reader.Read();
                }
				else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == name)
				{
					reader.Read();
					return dictionary;
				}
				else if (reader.NodeType == XmlNodeType.Element && reader.Name != "setixFixForDependency")
                {
                    dictionary[reader.Name] = reader.ReadElementContentAsString();
                }
				else if (reader.NodeType == XmlNodeType.Element && reader.Name == "setixFixForDependency")
				{
					try
					{
						dictionary[reader.Name] = reader.ReadElementContentAsString();
					}
					catch
					{
						reader.Read();
						dictionary["setixFixForDependency"] = reader.ReadContentAsInt().ToString();
						reader.Read();
					}
				}
                else
                {
                    reader.Read();
                }
            }
            throw new XmlException("XML reader reached end of file without locating end of element");
        }

        /// <summary>
        ///   Gets the error exception info.
        /// </summary>
        /// <param name="reader"> The reader. </param>
        /// <returns> a new FogBugzException witht the error code and message returned by the XML API. </returns>
        public static FogBugzException GetErrorExceptionInfo(this XmlReader reader)
        {
            var codestr = reader.GetAttribute("code");
            var message = reader.ReadElementContentAsString();

            if (!string.IsNullOrEmpty(codestr))
            {
                var code = Int32.Parse(codestr, NumberStyles.Integer, CultureInfo.InvariantCulture);
                return new FogBugzException(message, code);
            }
            return new FogBugzException(message);
        }
    }
}