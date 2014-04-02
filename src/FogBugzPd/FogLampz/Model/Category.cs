// -----------------------------------------------------------------------
//  <copyright file="Category.cs"
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
using System.Drawing;
using System.Reflection;
using System.Resources;
using FogLampz.Attributes;
using FogLampz.Properties;

#endregion

namespace FogLampz.Model
{
    public sealed partial class Category
    {
        private static readonly Dictionary<CategoryIcon, Bitmap> IconCache = new Dictionary<CategoryIcon, Bitmap>();

        /// <summary>
        ///   Initializes the <see cref="Category" /> class.
        /// </summary>
        static Category()
        {
            var resMan = new ResourceManager(typeof(Resources));
            var icons = typeof(CategoryIcon).GetFields(BindingFlags.GetField | BindingFlags.Public | BindingFlags.Static);
            foreach (var icon in icons)
            {
                var attributes = icon.GetCustomAttributes(typeof(AssociatedIconAttribute), false);
                if (attributes.Length <= 0)
                    continue;
                try
                {
                    IconCache.Add((CategoryIcon)icon.GetValue(null), (Bitmap)resMan.GetObject(((AssociatedIconAttribute)attributes[0]).Name));
                }
                catch
                {
                }
            }
        }

        /// <summary>
        ///   Gets the icon.
        /// </summary>
        public CategoryIcon Icon
        {
            get
            {
                if (IconIndex.HasValue)
                {
                    try
                    {
                        return (CategoryIcon)IconIndex.Value;
                    }
                    catch
                    {
                        return CategoryIcon.None;
                    }
                }
                return CategoryIcon.None;
            }
        }

        /// <summary>
        ///   Gets the image.
        /// </summary>
        public Bitmap Image
        {
            get
            {
                if (IconIndex.HasValue)
                {
                    if (IconCache.ContainsKey(Icon))
                    {
                        return IconCache[Icon];
                    }
                }
                return null;
            }
        }
    }
}