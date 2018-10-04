using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace sortxml
{
    /*
     * Basic LexicographicalComparer lifted from https://stackoverflow.com/a/7205788/11569
     * 
     */
    class LexicographicalComparer : IComparer<String>
    {

        static readonly Int32 NORM_IGNORECASE = 0x00000001;
        static readonly Int32 NORM_IGNORENONSPACE = 0x00000002;
        static readonly Int32 NORM_IGNORESYMBOLS = 0x00000004;
        static readonly Int32 LINGUISTIC_IGNORECASE = 0x00000010;
        static readonly Int32 LINGUISTIC_IGNOREDIACRITIC = 0x00000020;
        static readonly Int32 NORM_IGNOREKANATYPE = 0x00010000;
        static readonly Int32 NORM_IGNOREWIDTH = 0x00020000;
        static readonly Int32 NORM_LINGUISTIC_CASING = 0x08000000;
        static readonly Int32 SORT_STRINGSORT = 0x00001000;
        static readonly Int32 SORT_DIGITSASNUMBERS = 0x00000008;

        static readonly String LOCALE_NAME_USER_DEFAULT = null;
        static readonly String LOCALE_NAME_INVARIANT = String.Empty;
        static readonly String LOCALE_NAME_SYSTEM_DEFAULT = "!sys-default-locale";

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        static extern Int32 CompareStringEx(
          String localeName,
          Int32 flags,
          String str1,
          Int32 count1,
          String str2,
          Int32 count2,
          IntPtr versionInformation,
          IntPtr reserved,
          Int32 param
        );


        readonly String locale;

        public LexicographicalComparer() : this(CultureInfo.CurrentCulture) { }

        public LexicographicalComparer(CultureInfo cultureInfo)
        {
            if (cultureInfo.IsNeutralCulture)
                this.locale = LOCALE_NAME_INVARIANT;
            else
                this.locale = cultureInfo.Name;
        }

        public Int32 Compare(String x, String y)
        {
            // CompareStringEx return 1, 2, or 3. Subtract 2 to get the return value.
            return CompareStringEx(
              this.locale,
              SORT_DIGITSASNUMBERS, // Add other flags if required.
              x,
              x.Length,
              y,
              y.Length,
              IntPtr.Zero,
              IntPtr.Zero,
              0) - 2;
        }
        public static Int32 Compare(String x, String y,  System.StringComparison options)
        {
            Int32 flags = SORT_DIGITSASNUMBERS | SORT_STRINGSORT;
            string locale = CultureInfo.CurrentCulture.Name;
            switch (options)
            {
                case StringComparison.CurrentCulture:
                    locale = CultureInfo.CurrentCulture.Name;
                    flags = SORT_DIGITSASNUMBERS | SORT_STRINGSORT;
                    break;
                case StringComparison.CurrentCultureIgnoreCase:
                    locale = CultureInfo.CurrentCulture.Name;
                    flags = SORT_DIGITSASNUMBERS | SORT_STRINGSORT | LINGUISTIC_IGNORECASE;
                    break;
                case StringComparison.InvariantCulture:
                    locale = LOCALE_NAME_INVARIANT;
                    flags = SORT_DIGITSASNUMBERS | SORT_STRINGSORT;
                    break;
                case StringComparison.InvariantCultureIgnoreCase:
                    locale = LOCALE_NAME_INVARIANT;
                    flags = SORT_DIGITSASNUMBERS | SORT_STRINGSORT | LINGUISTIC_IGNORECASE;
                    break;
                case StringComparison.Ordinal:
                    locale = CultureInfo.CurrentCulture.Name;
                    flags = SORT_DIGITSASNUMBERS | SORT_STRINGSORT;
                    break;
                case StringComparison.OrdinalIgnoreCase:
                    locale = CultureInfo.CurrentCulture.Name;
                    flags = SORT_DIGITSASNUMBERS | SORT_STRINGSORT | NORM_IGNORECASE;
                    break;
                default:
                    locale = CultureInfo.CurrentCulture.Name;
                    flags = SORT_DIGITSASNUMBERS | SORT_STRINGSORT;
                    break;
            }
            // CompareStringEx return 1, 2, or 3. Subtract 2 to get the return value.
            return CompareStringEx(
              locale,
              flags, // Add other flags if required.
              x,
              x.Length,
              y,
              y.Length,
              IntPtr.Zero,
              IntPtr.Zero,
              0) - 2;
        }
    }
    }
