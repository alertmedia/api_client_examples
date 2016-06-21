using System;
using System.Collections;
using System.Collections.Generic;

namespace alertmedia
{
	public static class Utils
	{

		public static string Base64Encode(string plainText) {
			var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
			return System.Convert.ToBase64String(plainTextBytes);
		}

		public static string QueryString(Hashtable args)
		{
			var list = new List<string>();
			foreach (string key in args.Keys) {
				list.Add(key + "=" + args[key]);	
			}
			return string.Join("&", list);
		}

		public static bool dateValidate <T>(T dateValue) {
			if (typeof(T) == typeof(DateTime)) {
				return true;
			} else if (typeof(T) == typeof(string)) {
				string date = (string)(object)dateValue;
				if (string.IsNullOrEmpty (date)) {
					return false;
				} else {
					DateTime tempo;
					if (DateTime.TryParse (date, out tempo)) {
						return true;
					} else {
						return false;
					}
				}
			} else {
				return false;
			}
		}

		public static string convertDateToString(DateTime dateValue) {
			return dateValue.ToString("YYYY-MM-DDThh:mm[:ss[.uuuuuu]][+HH:MM|-HH:MM|Z]");
		}


	}
}

