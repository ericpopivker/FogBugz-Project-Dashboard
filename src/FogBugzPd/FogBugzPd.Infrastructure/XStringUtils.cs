using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Linq;

namespace FogBugzPd.Infrastructure
{
	public static class XStringUtils
	{
		#region Class Properties

		#endregion

		/// <summary>Returns a specified number of characters from the right side of a string.</summary>
		/// <param name="value">string value from which the rightmost characters are returned.</param>
		/// <param name="numChars">Number indicating how many characters to return.</param>
		/// <returns>string value containing specified number of characters from the right side of the inputed string.</returns>
		/// <remarks>If numbers of characters to retriXe is greater then the length of the string -- whole string is returned.</remarks>
		/// <example>.
		/// <code>
		/// string sTemp1=XStringUtils.Right("abcdefg", 3);   //returns "efg"
		/// string sTemp2=XStringUtils.Right("abcdefg", 10);  //returns "abcdefg"
		/// </code>
		/// </example>
		public static string Right(string value, int numChars)
		{
			string result = value;

			if (value.Length > numChars)
			{
				result = value.Substring(value.Length - numChars);
			}

			return (result);
		}


		/// <summary>Returns a specified number of characters from the left side of a string.</summary>
		/// <param name="value">string value from which the leftmost characters are returned.</param>
		/// <param name="numChars">Number indicating how many characters to return.</param>
		/// <returns>string value containing specified number of characters from the left side of the inputed string.</returns>
		/// <remarks>If numbers of characters to retriXe is greater then the length of the string -- whole string is returned.</remarks>
		/// <example>.
		/// <code>
		/// string sTemp1=XStringUtils.Left("abcdefg", 3);   //returns "abc"
		/// string sTemp2=XStringUtils.Left("abcdefg", 10);  //returns "abcdefg"
		/// </code>
		/// </example>
		/// 
		public static string Left(string value, int numChars)
		{
			string result = value;
			if (value.Length > numChars)
			{
				result = value.Substring(0, numChars);
			}

			return (result);
		}


		/// <summary>Returns a specified number of characters from a string.</summary>
		/// <param name="value">string value from which the characters are returned.</param>
		/// <param name="startIndex">Character position in string at which the part to be taken begins.</param>
		/// <param name="length">Number of characters to return.</param>
		/// <returns>string value containing specified number of characters.</returns>
		/// <remarks>If numbers of characters to retriXe overlaps the end of the string -- funciton returns all characters from startIndex position to the end of the string.</remarks>
		/// <example>.
		/// <code>
		/// string sTemp1=XStringUtils.Mid("abcdefg", 3, 2);   //returns "de"
		/// string sTemp2=XStringUtils.Mid("abcdefg", 3, 10);  //returns "defg"
		/// </code>
		/// </example>
		/// 
		public static string Mid(string value, int startIndex, int length)
		{
			return (value.Substring(startIndex, length));
		}


		/// <summary>Same as Mid function.</summary>
		/// <remarks>Similar to string.Substring, except it doesn't throw exception.</remarks>
		public static string Substring(string value, int startIndex, int length)
		{
			if (value.Length < startIndex)
				return "";

			//Get number of characters after the end of sText
			int extraChars = startIndex + length - value.Length;

			if (extraChars > 0)
				return value.Substring(startIndex, length - extraChars);
			else
				return value.Substring(startIndex, length);
		}


		/// <summary>Counts the occurances of the specified substring within string value.</summary>
		/// <param name="value">string value which contains substrings to count.</param>
		/// <param name="subString">Substring to count.</param>
		/// <returns>Returns number of substring occurances in specified string value.</returns>
		/// <example>.
		/// <code>
		/// int nCount1=XStringUtils.SubstringCount("abcabcabc", "ab");     //returns 3
		/// int nCount1=XStringUtils.SubstringCount("abcabcabc", "abcd");   //returns 0
		/// </code>
		/// </example>
		/// 
		public static int SubstringCount(string value, string subString)
		{
			int counter = 0;
			int index = 0;

			while ((index = value.IndexOf(subString, index) + 1) > 0)
				counter++;

			return counter;
		}


		/// <summary>Strips out all non-numeric characters from specified string value.</summary>
		/// <param name="value">string value.</param>
		/// <returns>string value consisting only of numeric characters from the original string.</returns>
		/// <example>.
		/// <code>
		/// string sPhone=XStringUtils.StripNonNumeric("(908) 555-2345");		//returns 9085552345
		/// string sFax=XStringUtils.StripNonNumeric("Fax is 1-800-341-1221");	//returns 18003411221
		/// </code>
		/// </example>
		/// 
		public static string StripNonNumeric(string value)
		{
			if (String.IsNullOrEmpty(value)) return null;
			string result = "";
			char[] chars = value.ToCharArray();
			char tempChar;

			for (int count = 0; count < chars.GetLength(0); count++)
			{
				tempChar = chars[count];
				if (Char.IsDigit(tempChar))
					result += tempChar;
			}

			return result;
		}

		public static string StripPhone(string value)
		{
			return StripNonNumeric(value);
		}


		/// <summary>Strips out all non-alpha-numeric characters from specified string value.</summary>
		public static string StripNonAlphaNumeric(string value)
		{
			Regex regex = new Regex("[^0-9a-zA-Z]", RegexOptions.None);
			string result = "";

			if (regex.IsMatch(value))
				result = regex.Replace(value, "");
			else
				result = value;

			return result;
		}


		/// <overloads>
		/// <summary>Removes html tags from the string.</summary>
		/// <remarks>Can be used to Strip out html data before saving a value to a CSV file.</remarks>
		/// </overloads>
		/// <summary>Removes html tags from the string.</summary>
		/// <param name="value">string value to be Stripped of html tags.</param>
		/// <returns>string value without any html tags.</returns>
		/// <example>.
		/// <code>
		/// string sHtml="&lt;b&gt;&lt;font='helvetica'&gt;Description of the task&lt;/font&gt;&lt;/b&gt;";
		/// string sText=XStringUtils.StripHtml(sHtml);		//returns "Description of the task"
		/// </code>
		/// </example>
		/// 
		public static string StripHtml(string value)
		{
			return StripHtml(value, false);
		}


		/// <summary>Removes html tags from the string and replaces &lt;/br&gt;s with return characters (\n).</summary>
		/// <param name="value">string value to be Stripped of html tags.</param>
		/// <param name="replaceBR">Indicated if  &lt;/br&gt;s  should be replaced with return characters (\n).</param>
		/// <returns>string value without any html tags.</returns>
		/// <example>.
		/// <code>
		/// string sHtml="&lt;b&gt;&lt;font='helvetica'&gt;Description of the task&lt;/font&gt;&lt;/b&gt;";
		/// sHtml+="&gt;/br&lt;The task has many steps";
		/// sHtml+="&gt;/br&lt;Step1";
		/// sHtml+="&gt;/br&lt;Step2";
		/// string sText=XStringUtils.StripHtml(sHtml);		//returns "Description of the task\nThe task has many steps\nStep1\nStep2"
		/// </code>
		/// </example>
		/// 
		public static string StripHtml(string value, bool replaceBR)
		{
			string result = value;

			if (replaceBR)
				result = Regex.Replace(result, "<br>", "\n", RegexOptions.IgnoreCase);

			//HtmlTextWriter inserts \r\n for carriage return
			//\r appears as [] in Excel
			result = Regex.Replace(result, "\r\n", "\n");
			result = Regex.Replace(result, "<[^>]*>", "");

			return result;
		}


		/// <overloads>
		/// <summary>Identifies the substrings in the inputed text delimited by specified delimiter, then places the substrings into ArrayList.</summary>
		/// </overloads>
		/// <summary>
		///		Generates ArrayList of sub-strings delimited by ', '
		/// </summary>
		/// <param name="value">string value to divide into sub-strings.</param>
		/// <returns>ArrayList of sub-strings delimited by ', '</returns>
		/// <example>
		/// <code>
		///		ArrayList aNames=XStringUtils.Split("Adam, Eric, John");  //Array will contain 3 items -- 'Adam', 'Eric' and 'John'.
		/// </code>
		/// </example>
		public static List<string> Split(string value)
		{
			return Split(value, ", ");
		}


		/// <summary>
		///		Generates List of substrings found in the inputed text and delimited by specified character/string.
		/// </summary>
		/// <param name="value">string value to divide into substrings.</param>
		/// <param name="delimiter">Delimiter that separates substrings.</param>
		/// <returns>List of sub-strings delimited by specified delimiter</returns>
		/// <example>
		/// <code>
		///		List&lt;string&gt; name=XStringUtils.Split("Adam##Eric##John", "##");  //List will contain 3 items -- 'Adam', 'Eric' and 'John'.
		/// </code>
		/// </example>
		public static List<string> Split(string value, string delimiter)
		{
			List<string> results = new List<string>();
			int index, lastIndex = 0;
			string subString;

			if (value == null)
				return results;

			if (value.Length == 0)
				return results;

			while ((index = value.IndexOf(delimiter, lastIndex)) != -1)
			{
				subString = value.Substring(lastIndex, index - lastIndex);
				lastIndex = index + delimiter.Length;
				results.Add(subString);
			}

			subString = value.Substring(lastIndex);
			results.Add(subString);

			return results;
		}


		public static string Join(IList<string> values)
		{
			return Join<string>(values);
		}


		public static string Join(IList<string> values, string itemDelimiter)
		{
			return Join<string>(values, itemDelimiter);
		}


		public static string Join(IList<string> values, string itemDelimiter, string textQualifier)
		{
			return Join<string>(values, itemDelimiter, textQualifier);
		}

		/// <overloads>
		/// <summary>Concatenates a specified separator between each element of a specified string array, yielding a valuegle concatenated string.</summary>
		/// </overloads>
		/// <summary>Combines array elements into one string inserting ", " between each element.</summary>
		/// <param name="values">Array of values to combine in a comma delimited string</param>
		/// <returns>string value containing comma delimited list of elements.</returns>
		/// <example>
		/// <code>
		/// ArrayList aNames=new ArrayList(new string[] {"Han", "Paul", "StXe"});
		/// string sList=XStringUtils.Join(aNames);	//Sets sList to string 'Han, Paul, StXe'
		/// </code>
		/// </example>
		public static string Join<T>(IList<T> values)
		{
			return Join(values, ", ", "");
		}


		/// <summary>Combines list elements into one string inserting specified item delimiter between each element.</summary>
		/// <param name="values">List of values to combine into a comma delimited string.</param>
		/// <param name="itemDelimiter">string value used to delimit fields in a list.</param>
		/// <returns>string value containing comma delimited list of items from specified array.</returns>
		/// <example>
		/// <code>
		/// List&lt;string&gt; names=new List&lt;string&gt;(new string[] {"Han", "Paul", "StXe"});
		/// string listText=XStringUtils.Join(names, "##");	//Generates string 'Han##Paul##StXe'
		/// </code>
		/// </example>
		public static string Join<T>(ICollection<T> values, string itemDelimiter)
		{
			return Join(values, itemDelimiter, "");
		}


		/// <summary>Combines list elements into one string inserting specified item delimiter between each element and adding specified string delimiter around each list item.</summary>
		/// <param name="values">List of values to combine into a comma delimited string</param>
		/// <param name="itemDelimiter">string value used to delimit fields in the list.</param>
		/// <param name="textQualifier">string value or character inserted around Xery value in the resulting list.</param>
		/// <returns>string value containing comma delimited list of items from specified array.</returns>
		/// <example>
		/// <code>
		/// List&lt;string&gt; names=new List&lt;string&gt;(new string[] {"Han", "Paul", "StXe"});
		/// string list=XStringUtils.Join(names, "##", "\"");	//Generates string '"Han"##"Paul"##"StXe"'
		/// </code>
		/// </example>
		public static string Join<T>(ICollection<T> values, string itemDelimiter, string textQualifier)
		{
			if (values.Count == 0)
				return "";

			StringBuilder sb = new StringBuilder("", values.Count*2);

			foreach (T value in values)
			{
				if (sb.Length != 0)
					sb.Append(itemDelimiter);

				sb.Append(textQualifier + value.ToString() + textQualifier);
			}

			return sb.ToString();
		}


		/// <summary>Checks if string consists only of numeric characters.</summary>
		/// <param name="value">string to check if is numeric.</param>
		/// <returns>Boolean indicating if string consists only of numeric values.</returns>
		/// <remarks>Internally calls Decimal.TryParse function to see if the value is numeric.</remarks>
		/// <example>
		/// <code>
		///	bool b1=XStringUtils.IsNumeric("one");  //Return false
		///	bool b2=XStringUtils.IsNumeric("123.43"); //Returns true
		///	bool b3=XStringUtils.IsNumeric("123"); //Returns true
		///	bool b4=XStringUtils.IsNumeric("123abc"); //Returns false
		/// </code>
		/// </example>
		public static bool IsNumeric(string value)
		{
			bool result;
			decimal d;

			result = Decimal.TryParse(value, NumberStyles.Number, NumberFormatInfo.InvariantInfo, out d);
			return result; //Check if any non-digits 
		}

		public static bool IsQuantity(string value)
		{
			if (!IsNumeric(value)) return false;

			decimal valueDecimal = Convert.ToDecimal(value);
			if (valueDecimal < 0 || valueDecimal != (int) valueDecimal) return false;

			return true;
		}

		/// <summary>Checks if string value is a valid e-amil address.</summary>
		/// <param name="value">string to check if e-mail.</param>
		/// <remarks>For backwards compatibility.</remarks>
		/// <returns>True if string is a valid e-mail address; otherwise, false.</returns>
		public static bool IsValidEmail(string value)
		{
			return IsEmail(value);
		}


		/// <summary>Checks if string value is a valid e-amil address.</summary>
		/// <param name="value">string to check if e-mail.</param>
		/// <returns>True if string is a valid e-mail address; otherwise, false.</returns>
		public static bool IsEmail(string value)
		{
			Regex re = new Regex(@"^([\w-]+\.)*[\w-]+@([\da-zA-Z-]+\.)+[\da-zA-Z]+$");
			if (re.IsMatch(value))
				return true;
			else
				return false;
		}

		/// <summary>Checks if string value is a valid currency.</summary>
		/// <param name="value">string to check if currency.</param>
		/// <returns>True if string is a valid currency value; otherwise, false.</returns>
		public static bool IsCurrency(string value)
		{
			Regex re = new Regex("^[\\$]{0,1}[\\d]*(\\.[\\d]{0,2}){0,1}$");
			if (!re.IsMatch(value))
				return false;

			return true;
		}


		/// <summary>Checks if a string is a valid date-time value.</summary>
		/// <param name="value">string to check to see if it's a date.</param>
		/// <returns>True if string contains valid date-time value, otherwise false.</returns>
		/// <remarks>Internally calls DateTime.Parse function and checks if it throws exception.</remarks>
		/// <example>
		/// <code>
		///	bool b1=XStringUtils.IsNumeric("one");  //Return false
		///	bool b2=XStringUtils.IsNumeric("123.43"); //Returns true
		///	bool b3=XStringUtils.IsNumeric("123"); //Returns true
		///	bool b4=XStringUtils.IsNumeric("123abc"); //Returns false
		/// </code>
		/// </example>
		public static bool IsDate(string value)
		{
			try
			{
				DateTime dt = DateTime.Parse(value);
			}
			catch
			{
				return false;
			}

			return true;
		}

		public static bool IsDate(string value, IFormatProvider formatProvider)
		{
			try
			{
				DateTime dt = DateTime.Parse(value, formatProvider);
			}
			catch
			{
				return false;
			}

			return true;
		}

		/// <summary>Checks if string value is a valid IP address.</summary>
		/// <param name="value">string to check if IP address.</param>
		/// <returns>True if string is a valid IP address value; otherwise, false.</returns>
		public static bool IsIPAddress(string value)
		{
			Regex re = new Regex("^(25[0-5]|2[0-4]\\d|[0-1]?\\d?\\d)(\\.(25[0-5]|2[0-4]\\d|[0-1]?\\d?\\d)){3}$");
			if (!re.IsMatch(value))
				return false;

			return true;
		}


		/// <summary>Shrinks the text value to specified length.</summary>
		/// <param name="value">Text value to shrink.</param>
		/// <param name="maxLength">Maximum length of the string.</param>
		/// <returns>Shrunk string value.</returns>
		/// <example>
		/// <code>
		///		string s1=XStringUtils.Shrink("John Smith", 4);		//Returns 'John...'
		///		string s2=XStringUtils.Shrink("John Smith", 20);	//Returns 'John Smith'
		/// </code>
		/// </example>
		public static string Shrink(string value, int maxLength)
		{
			string result;
			if (value == null)
				return "";

			if ((value.Length) > maxLength)
			{
				result = value.Substring(0, (maxLength - 3));
				result = result + "...";
			}
			else
			{
				result = value;
			}

			return result;
		}


		/// <summary>Does case-sensitive or case-insensitive text replacement.</summary>
		/// <param name="value">Input text that contains string values to replace.</param>
		/// <param name="oldValue">Text value to replace</param>
		/// <param name="newValue">Value to replace with</param>
		/// <param name="caseSensitive">Indicates to if search for values to replace should be Case-Sensitive or not</param>
		/// <returns>string with all specified values replaced by new values.</returns>
		public static string Replace(string value, string oldValue, string newValue, bool caseSensitive)
		{
			if (caseSensitive)
			{
				return value.Replace(oldValue, newValue);
			}
			else
			{
				//Use regular expression to do case-insensitive
				Regex oRegex = new Regex(oldValue, RegexOptions.IgnoreCase);
				return oRegex.Replace(value, newValue);
			}
		}


		/// <summary>Returns true if specified string exists in specified array.</summary>
		/// <remarks>Comparison is case-insensetive</remarks>
		/// <param name="value">string to find</param>
		/// <param name="values">Array of values</param>
		/// <returns>True when string exists, false otherwise</returns>
		public static bool ExistsInList(string value, List<string> values)
		{
			int? index = FindInList(value, values);
			if (index != null)
				return true;
			else
				return false;
		}


		/// <summary>Finds string in ArrayList uvalueg case-insensitive comparison</summary>
		/// <param name="value">Value to find</param>
		/// <param name="values">Array of data</param>
		/// <returns>Index of matching string or NIL if not found</returns>
		public static int? FindInList(string value, List<string> values)
		{
			string arrayItem = "";
			for (int count = 0; count < values.Count; count++)
			{
				arrayItem = values[count].ToString();

				if (arrayItem.ToUpper() == value.ToUpper())
					return count;
			}

			return null;
		}

		/// <summary>Appends string value to delimeted string list.</summary>
		/// <param name="value">Value to add to list</param>
		/// <param name="list">Delimited list that will change</param>
		/// <param name="delimiter">The delimitor used to separate items in the list.</param>
		/// <example>Create a comma delimited list of names.
		/// <code>
		///	string list="";
		///	XStringUtils.AppendToList("Adam", ref list, ", ");
		///	XStringUtils.AppendToList("Eric", ref list, ", ");
		///	XStringUtils.AppendToList("John", ref list, ", ");
		///	
		///	Console.WriteLine(list);
		///	//Displays 'Adam, Eric, John'
		///	
		/// </code>
		/// </example>
		public static void AppendToList(string value, ref string list, string delimiter)
		{
			if (list != "")
				list += delimiter;
			list += value;
		}

		/// <summary>Appends string value to delimeted string list represented by StringBuilder.</summary>
		/// <param name="value">Value to add to list</param>
		/// <param name="stringBuilder">Delimited list that will change</param>
		/// <param name="delimiter">The delimitor used to separate items in the list.</param>
		public static void AppendToList(string value, StringBuilder stringBuilder, string delimiter)
		{
			if (stringBuilder.Length != 0)
				stringBuilder.Append(delimiter);

			stringBuilder.Append(value);
		}

		private static char _maskingCharacterDefault = '*';

		/// <summary>Masks all the characters in the string with the specified character</summary>
		/// <param name="value">Value to mask</param>
		/// <param name="maskingCharacter">Charachter used for masking</param>
		public static String Mask(String value, char? maskingCharacter)
		{
			return new string(maskingCharacter == null ? _maskingCharacterDefault : maskingCharacter.Value, value.Length);
		}

		/// <summary>Masks charachters in the string with the specified character. The number of charachters from the right end of the string are not masked.</summary>
		/// <param name="value">Value to mask</param>
		/// <param name="maskingCharacter">Charachter used for masking</param>
		/// <param name="numVisibleChars">Number of charachters not to mask</param>
		public static String MaskLeft(String value, char? maskingCharacter, int numVisibleChars)
		{
			if (value == null) return value;
			if (value.Length < numVisibleChars) return value;
			if (numVisibleChars < 0) return value;

			return value.Substring(value.Length - numVisibleChars).PadLeft(value.Length,
																		   maskingCharacter == null
																			? _maskingCharacterDefault
																			: maskingCharacter.Value);
		}

		/// <summary>Masks charachters in the string with the specified character. The number of charachters from the left end of the string are not masked.</summary>
		/// <param name="value">Value to mask</param>
		/// <param name="maskingCharacter">Charachter used for masking</param>
		/// <param name="numVisibleChars">Number of charachters not to mask</param>
		public static String MaskRight(String value, char? maskingCharacter, int numVisibleChars)
		{
			if (value == null) return value;
			if (value.Length < numVisibleChars) return value;
			if (numVisibleChars < 0) return value;

			return value.Substring(0, numVisibleChars).PadRight(value.Length,
																maskingCharacter == null
																	? _maskingCharacterDefault
																	: maskingCharacter.Value);
		}



		public static string CapitalizeWords(string value)
		{
			if (value == null)
				throw new ArgumentNullException("value");
			if (value.Length == 0)
				return value;

			StringBuilder result = new StringBuilder(value.ToLower());
			result[0] = char.ToUpper(result[0]);
			for (int i = 1; i < result.Length; ++i)
			{
				if (char.IsWhiteSpace(result[i - 1]))
					result[i] = char.ToUpper(result[i]);
				else
					result[i] = char.ToLower(result[i]);
			}
			return result.ToString();
		}

		public static string Crop(this string str, int length)
		{
			if (str == null) return null;
			var len = str.Length;
			return len < length ? str : str.Substring(0,length);
		}

		public static string RightCrop(this string str, int length)
		{
			if (str == null) return null;
			var startpos = str.Length - length;
			return startpos > 0 ? str.Substring(startpos) : str;
		}

		//For string Some.Namespace.Reference.Api returns Api
		public static string GetAllAfterLastChar(this string str, char chr)
		{
			var parts = str.Split(chr);
			return parts.Last();
		}

		public static string GetAllBeforeLastChar(this string str, char chr)
		{
			var parts = str.Split(chr);
			return String.Join(chr.ToString(), parts.Take(parts.Length - 1));
		}

		public static bool ContainsWords(this string str,string searchString)
		{
			//remove delimiters
			var cleanStr = str.Replace(",", " ").Replace(".", "").Replace("`","").Replace("'","").Replace("\"","").Replace("  ", " ");
			var cleanSearchStr = searchString.Replace(",", " ").Replace(".", "").Replace("`","").Replace("'","").Replace("\"","").Replace("  ", " ");
			//add providerces between numeric and not numeric
			var tempStr = cleanStr;
			cleanStr = "";
			for (var i = 0; i < tempStr.Length; i++)
			{
				if (Char.IsDigit(tempStr[i]))
				{
					//before
					if(i>0)
					{
						var chr = tempStr[i - 1];
						if(!Char.IsDigit(chr))
						{
							//add providerce
							cleanStr += chr;
						}
					}
					cleanStr += tempStr[i];
					//after
					if (i < tempStr.Length-1)
					{
						var chr = tempStr[i + 1];
						if (!Char.IsDigit(chr))
						{
							//add providerce
							cleanStr += chr;
						}
					}
				}
				else
				{
					cleanStr += tempStr[i];
				}
			}

			var tempSearchStr = cleanSearchStr;
			cleanSearchStr = "";
			for (var i = 0; i < tempSearchStr.Length; i++)
			{
				if (Char.IsDigit(tempSearchStr[i]))
				{
					//before
					if (i > 0)
					{
						var chr = tempSearchStr[i - 1];
						if (!Char.IsDigit(chr))
						{
							//add providerce
							cleanSearchStr += chr;
						}
					}
					cleanSearchStr += tempSearchStr[i];
					//after
					if (i < tempSearchStr.Length-1)
					{
						var chr = tempSearchStr[i + 1];
						if (!Char.IsDigit(chr))
						{
							//add providerce
							cleanSearchStr += chr;
						}
					}
				}
				else
				{
					cleanSearchStr += tempSearchStr[i];
				}
			}

			//to lower
			cleanStr = cleanStr.ToLower();
			cleanSearchStr = cleanSearchStr.ToLower();

			//split to array and skip words with lenght<3 if it not numeric
			Decimal numberHolder = 0;
			var excludedWords = new[]{"the"};
			var arrayStr = cleanStr.Split(' ').Where(s => (Decimal.TryParse(s, out numberHolder) || s.Length >= 3) && !excludedWords.Contains(s)).ToList();
			var arraySearchStr = cleanSearchStr.Split(' ').Where(s => (Decimal.TryParse(s, out numberHolder) || s.Length >= 3) && !excludedWords.Contains(s)).ToList();
			var matchedCount = 0;
			// check word by word and exclude from array if it found
			//UPD: "Day Provider" must match "Dayprovider"
			foreach (var s in arraySearchStr)
			{
				if(arrayStr.Any(a=>a.Contains(s)))
				{
					//arrayStr.Remove(s);
					matchedCount++;
				}
			}
			if (matchedCount == arraySearchStr.Count) return true;
			return false;
		}

		public static string UppercaseFirst(this string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return string.Empty;
			}
			char[] a = s.ToCharArray();
			a[0] = char.ToUpper(a[0]);
			return new string(a);
		}

		public static bool ContainsWord(this string str, string word)
		{
			var searchedStr =" "+
				str.Replace(",", " ").Replace(".", " ").Replace("-", " ").Replace(";", " ").Replace("   ", " ").Replace("  ", " ").
					Trim()+" ";

			return searchedStr.Contains(" " + word + " ");
		}

	}
}