using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Collections;


namespace FogBugzPd.Web.Utils
{
	/// <summary>Consists of static functions used for common conversions.</summary>
	/// <remarks>
	/// <para>
	/// Supports 4 types of conversions used in 3-tier architecture:  
	/// Database Data --> Object Properties, 
	/// Object Properties --> Form Control Values, 
	/// Form Control Values --> Object Properties, 
	/// Object Properties --> Database Data
	/// </para>
	/// <para>XConvert takes into account the following NULL representations of strongly-typed varibales:</para>
	/// <para>Numeric representation of NULL is "XConst.NIL".</para>  
	/// <para>DateTime representation of NULL is "XUtil.GetEmptyDate()"</para>
	/// </remarks>
	public class XConvert
	{
		/// <summary>Converts the specified value value to an integer (int32).</summary>
		/// <param name="value">Text value to convert.</param>
		/// <returns>null if value is null or an empty string.
		/// Otherwise, it returns an integer, equivalent to the specified string. 
		/// </returns>
		/// <remarks>
		/// Usually used to set object property to a Form Control value.  
		/// </remarks>
		/// <example>Set Customer's age property to a value of the value box.
		/// <code>
		/// oCustomer.Age=XConvert.TextToInt(txtAge.Text);
		/// </code>
		/// </example>
		public static int? TextToInt(string value)
		{
			if (String.IsNullOrEmpty(value))
			{
				return null;
			}
			else
			{
				return Convert.ToInt32(value);
			}
		}


		/// <summary>Converts the specified value value to an equivalent double-precision floating point number.</summary>
		/// <param name="value">Text value to convert.</param>
		/// <returns>null if value is null or an empty string.
		/// Otherwise, it returns double-precision floating point number, equivalent to the specified string. 
		/// </returns>
		public static double? TextToDouble(string value)
		{
			if (String.IsNullOrEmpty(value))
			{
				return null;
			}
			else
			{
				return Convert.ToDouble(value);
			}
		}


		/// <summary>
		/// Converts the specified value value to an equivalent decimal number.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static decimal? TextToDecimal(string value)
		{
			if (String.IsNullOrEmpty(value))
			{
				return null;
			}
			else
			{
				return Convert.ToDecimal(value);
			}
		}


		/// <summary>Converts the specified string representation of a date and time to an equivalent DateTime.</summary>
		/// <param name="value">Text value to convert.</param>
		/// <returns>
		/// Returns null if value is null or an empty string.
		/// Otherwise, it returns DateTime equivalent to the specified string. 
		/// </returns>
		public static DateTime? TextToDateTime(string value)
		{
			if (String.IsNullOrEmpty(value))
			{
				return null;
			}
			else
			{
				try
				{
					return Convert.ToDateTime(value);
				}
				catch (Exception)
				{
					return Convert.ToDateTime(value, new CultureInfo("en-US"));
				}
			}
		}


		/// <summary>Converts the specified value value to an equivalent boolean.</summary>
		/// <param name="value">Text value to convert.</param>
		/// <returns>If value is null or an empty string -- returns false.
		/// Otherwise checks if value is "0", "FALSE", "NO".  If it is equal to any of those
		/// values -- returns false.  Method returns 'true' in all other cases. 
		/// </returns>
		public static bool? TextToBool(string value)
		{
			if (String.IsNullOrEmpty(value))
			{
				return null;
			}
			else
			{
				if ((value == "0") || (value.ToUpper() == "FALSE") || (value.ToUpper() == "NO"))
					return false;
				else
					return true;
			}
		}


		/// <summary>Converts strongly-typed variable into equivalent string.</summary>
		/// <param name="value">Boxed value to convert.</param>
		/// <returns>String value equivalent to specified value.</returns>
		/// <remarks>
		/// Usually used to convert object properties into Form Control values.  
		/// </remarks>
		/// <example>Set value of the value box to Customer's age.
		/// <code>
		/// txtBox.Test=XConvert.ObjToText(oCustomer.Age);
		/// </code>
		/// </example>
		public static string ObjToText(object value)
		{
			if (value == null)
				return "";
			else
				return value.ToString();
		}


		/// <summary>Converts strongly-typed variable into equivalent int (Int32).</summary>
		/// <param name="value">Boxed value to convert.</param>
		/// <returns>String value equivalent to specified value.</returns>
		/// <remarks>
		/// Usually used to convert object properties into integer values.  
		/// 0 is returned if the function fails or exception occurs.
		/// </remarks>
		/// <example>Set value of a variable to Customer's age.
		/// <code>
		/// age = XConvert.ObjToInt(oCustomer.Age);
		/// </code>
		/// </example>
		public static int ObjToInt(object value)
		{
			try
			{
				return System.Convert.ToInt32(value);
			}
			catch
			{
				return 0;
			}
		}


		/// <summary>Converts value from database to a string.</summary>
		/// <param name="value">Value from database.</param>
		/// <returns>Empty string if database value is DBNull. Otherwise returns string value equivalent to specified database value.</returns>
		/// <remarks>
		/// Usually used to fill database value into an object property. 
		/// </remarks>
		/// <example>Fill customer name with the value from the DataReader or DataSet.
		/// <code>
		/// customer.Name=XConvert.DBToStr(dataReader["Name"]);
		/// </code>
		/// </example>
		/// 		
		public static string DBToStr(object value)
		{
			if (value.GetType().ToString() != "System.DBNull")
				return (string) value;
			else
				return null;
		}


		/// <summary>Converts database value to an integer.</summary>
		/// <param name="value">Value from database.</param>
		/// <returns>Null if database value is DBNull. Otherwise converts database value to an integer and returns it.</returns>
		/// <remarks>
		/// Usually used to fill database value into an object property. 
		/// </remarks>
		/// <example>Fill customer age with the value from the DataSet or DataReader.
		/// <code>
		/// customer.Age=XConvert.DBToInt(dataReader["Age"]);
		/// </code>
		/// </example>
		public static int? DBToInt(object value)
		{
			string valueType;
			valueType = value.GetType().ToString();

			int? result;
			if (valueType != "System.DBNull")
			{
				if (valueType == "System.Decimal")
				{
					result = Decimal.ToInt32((decimal) value);
				}
				else
				{
					result = (int) value;
				}
			}
			else
			{
				result = null;
			}

			return (result);
		}


		/// <summary>Converts database value to a double-precision floating point number.</summary>
		/// <param name="value">Value from database.</param>
		/// <returns>NIL if database value is DBNull. Otherwise converts database value to a double and returns it.</returns>
		/// <remarks>
		/// Usually used to fill database value into an object property. 
		/// </remarks>
		/// <example>
		/// <code>
		/// customer.Salary=XConvert.DBToDbl(dataReader["Salary"]);
		/// </code>
		/// </example>
		public static double? DBToDbl(object value)
		{
			string valueType;
			valueType = value.GetType().ToString();

			double? result;
			if (valueType != "System.DBNull")
			{
				if (valueType == "System.Decimal")
					result = Decimal.ToDouble((decimal) value);
				else
					result = Convert.ToDouble(value);
			}
			else
			{
				result = null;
			}

			return result;
		}


		/// <summary>Converts database value to a double-precision floating point number.</summary>
		/// <param name="value">Value from database.</param>
		/// <returns>Null if database value is DBNull. Otherwise converts database value to a double and returns it.</returns>
		public static double? DBToDouble(object value)
		{
			return DBToDbl(value);
		}


		/// <summary>
		/// Used for Money
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static decimal? DBToDecimal(object value)
		{
			string valueType;
			valueType = value.GetType().ToString();

			decimal? result;
			if (valueType != "System.DBNull")
			{
				//decimal for money field uses 4 digits of precision
				//So amount.ToString() shows 40.0000 instead of 40.00
				//For now - assume all decimals are money.  
				//When that's not the case - will need to use DataReader.GetDataTypeName().
				result = (decimal) value;
				result = Decimal.Round(result.Value, 2);
			}
			else
			{
				result = null;
			}

			return result;
		}


		/// <summary>Converts database value to a DateTime.</summary>
		/// <param name="value">Value from database.</param>
		/// <returns>Returns null if database value is DBNull. Otherwise converts database value to a DateTime and returns it.</returns>
		/// <remarks>
		/// Usually used to set an object property to a value from a database. 
		/// </remarks>
		/// <example>
		/// <code>
		/// customer.DateOfBirth=XConvert.DBToDateTime(dataReader["DateOfBirth"]);
		/// </code>
		/// </example>
		public static DateTime? DBToDateTime(object value)
		{
			if (value.GetType().ToString() != "System.DBNull")
				return (DateTime) value;
			else
				return null;
		}


		/// <summary>Converts database value to a boolean.</summary>
		/// <param name="value">Value from database.</param>
		/// <returns>Retuns 'null' if database value is DBNull. Otherwise returns INullable Bool initialized to a value from a database.</returns>
		/// <remarks>
		/// Usually used to fill database value into an object property. 
		/// </remarks>
		/// <example>
		/// <code>
		/// customer.HasChildren=XConvert.DBToBool(dataReader["HasChildren"]);
		/// </code>
		/// </example>
		/// 
		public static bool? DBToBool(object value)
		{
			if (value.GetType().ToString() != "System.DBNull")
				if ((value.ToString() == "1") || (value.ToString() == "True"))
					return true;
				else
					return false;
			else
				return null;
		}


		public static object DBToObj(object value, Type dbColumnType)
		{
			object result;

			switch (dbColumnType.ToString())
			{
				case "System.String":
					result = DBToStr(value);
					break;
				case "System.Int32":
					result = DBToInt(value);
					break;
				case "System.Double":
					result = DBToDouble(value);
					break;
				case "System.Decimal":
					result = DBToDecimal(value);
					break;
				case "System.DateTime":
					result = DBToDateTime(value);
					break;
				case "System.Boolean":
					result = DBToBool(value);
					break;

				default:
					throw new Exception("XConvert::DBToObj Invalid value type '" + dbColumnType.ToString() + "'");
			}

			return result;
		}


		/// <summary>Converts strongly-typed varible to a value ready to be insterted into database.</summary>
		/// <param name="value">Strongly-typed variable.</param>
		/// <returns>
		/// Retuns 'DBNull' if value is equivalent to 'null'. See class 'Remarks' for cases when strongly-typed variables are equivalent to null.
		/// Otherwise returns value.
		/// </returns>
		/// <remarks>
		/// Usually used to insert object property value into database. 
		/// </remarks>
		/// <example>
		/// <code>
		/// dataSet["Name"]	        =XConvert.ObjToDB(oCustomer.Name);
		/// dataSet["Age"]			=XConvert.ObjToDB(oCustomer.Age);
		/// dataSet["HasChildren"]	=XConvert.ObjToDB(oCustomer.HasChildren);
		/// </code>
		/// </example>
		public static object ObjToDB(object value)
		{
			object result = DBNull.Value;
			if (value == null)
				return DBNull.Value;
			else
				return value;
		}


		/// <summary>
		/// Method to convert a custom Object to XML string
		/// From http://www.dotnetjohn.com/articles.aspx?articleid=173
		/// </summary>
		/// <param name="obj">Object that is to be serialized to XML</param>
		/// <returns>XML string</returns>
		public static string ObjToXML(object obj)
		{
			String result = null;
			StringBuilder stringBuilder = new StringBuilder();

			XmlWriterSettings settings = new XmlWriterSettings();
			settings.OmitXmlDeclaration = true; // Remove the <?xml version="1.0" encoding="utf-8"?>
			//settings.Indent = true;

			XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());
			XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, settings);

			//Create our own namespaces for the output
			XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
			namespaces.Add("", ""); //Add an empty namespace and empty value

			//Use namespaces attribute to skipe xmlns attribute in each field
			xmlSerializer.Serialize(xmlWriter, obj, namespaces);

			// Make sure the serialize function is finished
			xmlWriter.Flush();

			result = stringBuilder.ToString();
			return result;
		}


		/// <summary>Converts Nullable bool to equivalent integer value</summary>
		/// <param name="value">Boolean to convert into integer</param>
		/// <returns>Integer equivalent to specified boolean</returns>
		public static int? BoolToInt(bool? value)
		{
			if (value == null)
				return null;
			else if (value == true)
				return 1;
			else
				return 0;
		}


		/// <summary>Converts integer value to equivalent boolean</summary>
		/// <param name="value">Integer to convert into Bool?</param>
		/// <returns>Boolean equivalent to specified integer</returns>
		public static bool? IntToBool(int? value)
		{
			bool? result;

			if (value == null)
				result = null;
			else if (value == 0)
				result = false;
			else
				result = true;

			return result;
		}

		public static IList<int> CSVtoInts(string csv)
		{
			string[] strs = csv.Split(new char[] {','});
			return StringsToInts(strs);
		}

		public static IList<int> StringsToInts(IEnumerable<string> strings)
		{
			var integers = new List<int>();
			foreach (string str in strings)
				integers.Add(int.Parse(str));

			return integers;
		}

		public static string ToCSV(IEnumerable enumerable)
		{
			string csv = String.Empty;
			foreach (object item in enumerable)
			{
				csv += item.ToString();
				csv += ',';
			}
			if (csv.Length > 0)
				csv.Remove(csv.Length - 1, 1);

			return csv;
		}

		//from http://stackoverflow.com/questions/1329426/how-do-i-round-to-the-nearest-0-5
		public static decimal RoundDecimalToHalf(decimal value)
		{
			value *= 2;
			value = Math.Round(value, MidpointRounding.AwayFromZero);
			return value/2;
		}
	}
}