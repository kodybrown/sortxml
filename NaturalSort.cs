using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace sortxml
{
	public static class NaturalSort
	{
		public static int Compare( object aObj, object bObj, StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase )
		{
			var a = new SortString(aObj);
			var b = new SortString(bObj);
			return a.CompareTo(b, stringComparison);
		}
	}

	public class SortString
	{
		private const int SAME = 0;
		private const int A = -1;
		private const int B = 1;

		private List<object> items = new List<object>();

		public SortString( object value )
		{
			items = Parse(value);
		}

		public int CompareTo( object b, StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase ) => CompareTo(new SortString(b), stringComparison);

		public int CompareTo( SortString b, StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase )
		{
			var result = 0;
			var a = this;

			for (var i = 0; i < a.items.Count; i++) {
				if (i > b.items.Count) {
					return B; // a and b match so far, but b is shorter..
				}

				var aItem = Convert.ToString(a.items[i]);
				var bItem = Convert.ToString(b.items[i]);

				result = string.Compare(aItem, bItem, stringComparison);
				if (result != 0) {
					if (IsNumeric(aItem) && IsNumeric(bItem) /* && aItem.ToString().Length == bItem.ToString().Length */) {
						result = double.Parse(aItem).CompareTo(double.Parse(bItem));
						if (result != 0) {
							return result;
						}
					} else {
						return result;
					}
				}
			}

			return result;
		}

		private bool IsNumeric( object value )
		{
			foreach (var c in Convert.ToString(value)) {
				if (c < '0' || c > '9') {
					return false;
				}
			}
			return true;
		}

		private List<object> Parse( object value )
		{
			if (value == null) { return new List<object>(); }

			var items = new List<object>();
			var s = Convert.ToString(value);
			var isDigit = false;
			var curValue = "";

			for (var i = 0; i < s.Length; i++) {
				var c = s[i];

				if (c >= '0' && c <= '9') {
					if (isDigit) {
						curValue += c.ToString();
					} else {
						if (!string.IsNullOrEmpty(curValue)) {
							items.Add(curValue);
						}
						curValue = c.ToString();
						isDigit = true;
					}
				} else {
					if (isDigit) {
						if (!string.IsNullOrEmpty(curValue)) {
							items.Add(curValue);
						}
						curValue = c.ToString();
						isDigit = false;
					} else {
						curValue += c.ToString();
					}
				}
			}
			if (!string.IsNullOrEmpty(curValue)) {
				items.Add(curValue);
			}

			// foreach (var x in items) {
			// 	Console.Write(x);
			// }
			// Console.WriteLine();
			return items;
		}

		public override string ToString()
		{
			var s = new StringBuilder();
			foreach (var x in items) {
				s.Append(x);
			}
			return s.ToString();
		}

	}
}