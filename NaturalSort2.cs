// using System;
// using System.Collections;
// using System.Text;

// namespace sortxml
// {
// 	public class NaturalSortComparator : IComparer
// 	{
// 		private enum ChunkType { Alphanumeric, Numeric };

// 		private bool InChunk( char ch, char otherCh )
// 		{
// 			ChunkType type = ChunkType.Alphanumeric;

// 			if (char.IsDigit(otherCh)) {
// 				type = ChunkType.Numeric;
// 			}

// 			if ((type == ChunkType.Alphanumeric && char.IsDigit(ch))
// 				|| (type == ChunkType.Numeric && !char.IsDigit(ch))) {
// 				return false;
// 			}

// 			return true;
// 		}

// 		public int Compare( object aObj, object bObj )
// 		{
// 			var a = aObj as string;
// 			var b = bObj as string;

// 			if (a == null || b == null) {
// 				return 0;
// 			}

// 			var thisMarker = 0, thisNumericChunk = 0;
// 			var thatMarker = 0, thatNumericChunk = 0;

// 			while ((thisMarker < a.Length) || (thatMarker < b.Length)) {
// 				if (thisMarker >= a.Length) {
// 					return -1;
// 				} else if (thatMarker >= b.Length) {
// 					return 1;
// 				}

// 				var thisCh = a[thisMarker];
// 				var thatCh = b[thatMarker];

// 				var thisChunk = new StringBuilder();
// 				var thatChunk = new StringBuilder();

// 				while ((thisMarker < a.Length) && (thisChunk.Length == 0 || InChunk(thisCh, thisChunk[0]))) {
// 					thisChunk.Append(thisCh);
// 					thisMarker++;

// 					if (thisMarker < a.Length) {
// 						thisCh = a[thisMarker];
// 					}
// 				}

// 				while ((thatMarker < b.Length) && (thatChunk.Length == 0 || InChunk(thatCh, thatChunk[0]))) {
// 					thatChunk.Append(thatCh);
// 					thatMarker++;

// 					if (thatMarker < b.Length) {
// 						thatCh = b[thatMarker];
// 					}
// 				}

// 				var result = 0;

// 				// If both chunks contain numeric characters, sort them numerically
// 				if (char.IsDigit(thisChunk[0]) && char.IsDigit(thatChunk[0])) {
// 					thisNumericChunk = Convert.ToInt64(thisChunk.ToString());
// 					thatNumericChunk = Convert.ToInt64(thatChunk.ToString());

// 					if (thisNumericChunk < thatNumericChunk) {
// 						result = -1;
// 					}

// 					if (thisNumericChunk > thatNumericChunk) {
// 						result = 1;
// 					}
// 				} else {
// 					result = thisChunk.ToString().CompareTo(thatChunk.ToString());
// 				}

// 				if (result != 0) {
// 					return result;
// 				}
// 			}

// 			return 0;
// 		}
// 	}
// }