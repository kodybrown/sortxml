/*!
	Copyright (c) 2014-2022 Kody Brown (@kodybrown)

	MIT License:

	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to
	deal in the Software without restriction, including without limitation the
	rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
	sell copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in
	all copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
	FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
	DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace sortxml
{
    class Program
    {
        static bool OptShowHelp = false;
        static bool OptShowExamples = false;
        static bool OptPause = false;
        static bool OptDebug = false;
        static bool OptOverwriteInFile = false;
        static bool OptPrettify = true;
        static bool OptSortNodes = true;
        static StringComparison OptNodeSortCase = StringComparison.CurrentCulture; // Default to case-sensitive sorting.
        static bool OptSortAttrs = true;
        static StringComparison OptAttrSortCase = StringComparison.CurrentCulture;
        static string OptPrimarySortAttr = "";
        static string OptNewLineChars = "\\r\\n";
        static string OptIndentChars = "\\t";
        static bool OptNewLineOnAttrs = false;

        static XmlWriterSettings settings { get; set; }

        static int Main( string[] arguments )
        {
            var inFile = "";
            var outFile = "";

            var doc = new XmlDocument();

            for (var i = 0; i < arguments.Length; i++) {
                var a = arguments[i];
                var isFlag = false;
                var flagVal = true;

                if (OptDebug) {
                    Console.WriteLine($"argument[{i}] = [`{a}`]");
                }

                a = RemoveOutsideQuotes(a);

                while (a[0] == '-' || a[0] == '/' || a[0] == '!') {
                    if (a[0] == '!') {
                        flagVal = false;
                    }
                    isFlag = true;
                    a = a.Substring(1);
                }

                a = RemoveOutsideQuotes(a);

                if (isFlag) {
                    if (a == "?" || a == "help") {
                        OptShowHelp = true;
                    } else if (a == "example" || a == "examples") {
                        OptShowExamples = true;
                    } else if (a == "p" || a == "pause") {
                        OptPause = flagVal;
                    } else if (a == "e" || a == "debug") {
                        OptDebug = flagVal;

                    } else if (a == "i" || a == "case-insensitive") {
                        OptNodeSortCase =
                        OptAttrSortCase = flagVal
                            ? StringComparison.CurrentCultureIgnoreCase
                            : StringComparison.CurrentCulture;
                    } else if (a == "node-case-insensitive") {
                        OptNodeSortCase = flagVal
                            ? StringComparison.CurrentCultureIgnoreCase
                            : StringComparison.CurrentCulture;
                    } else if (a == "attr-case-insensitive") {
                        OptAttrSortCase = flagVal
                            ? StringComparison.CurrentCultureIgnoreCase
                            : StringComparison.CurrentCulture;
                    } else if (a == "t" || a == "case-sensitive") {
                        OptNodeSortCase =
                        OptAttrSortCase = flagVal
                            ? StringComparison.CurrentCulture
                            : StringComparison.CurrentCultureIgnoreCase;
                    } else if (a == "node-case-sensitive") {
                        OptNodeSortCase = flagVal
                            ? StringComparison.CurrentCulture
                            : StringComparison.CurrentCultureIgnoreCase;
                    } else if (a == "attr-case-sensitive") {
                        OptAttrSortCase = flagVal
                            ? StringComparison.CurrentCulture
                            : StringComparison.CurrentCultureIgnoreCase;

                    } else if (a == "s" || a == "sort" || a == "sort-all") {
                        OptSortNodes =
                        OptSortAttrs = flagVal;
                    } else if (a == "sort-node" || a == "sort-nodes") {
                        OptSortNodes = flagVal;
                    } else if (a == "sort-attr" || a == "sort-attrs") {
                        OptSortAttrs = flagVal;

                    } else if (a == "pretty") {
                        OptPrettify = flagVal;
                    } else if (a == "overwrite") {
                        OptOverwriteInFile = flagVal;

                    } else if (a.StartsWith("primary-attr=")) {
                        OptPrimarySortAttr = a.Substring("primary-attr=".Length).Trim();
                        OptPrimarySortAttr = RemoveOutsideQuotes(OptPrimarySortAttr);

                    } else if (a.StartsWith("new-line-chars=")) {
                        OptNewLineChars = a.Substring("new-line-chars=".Length);
                        OptNewLineChars = RemoveOutsideQuotes(OptNewLineChars);
                    } else if (a.StartsWith("indent-chars=")) {
                        OptIndentChars = a.Substring("indent-chars=".Length);
                        OptIndentChars = RemoveOutsideQuotes(OptIndentChars);
                    } else if (a == "new-line-on-attrs") {
                        OptNewLineOnAttrs = flagVal;

                    } else {
                        Console.WriteLine($"**** Unknown flag: '{arguments[i]}'. ****");
                        return 11;
                    }
                } else {
                    if (OptShowHelp && (a == "example" || a == "examples")) {
                        OptShowExamples = true;
                    } else if (inFile.Length == 0) {
                        inFile = a;
                    } else if (outFile.Length == 0) {
                        outFile = a;
                    } else {
                        Console.WriteLine($"**** Unknown argument: '{arguments[i]}'. ****");
                        return 11;
                    }
                }
            }

            if (OptDebug) {
                Console.WriteLine($"┌─{new string('─', 18 - 1)} {"DEBUG ".PadRight(23, '─')}─┐");
                Console.WriteLine($"│ {"OptPause",-18} = {OptPause.ToString().ToLower(),-20} │");
                Console.WriteLine($"│ {"OptDebug",-18} = {OptDebug.ToString().ToLower(),-20} │");
                Console.WriteLine($"│ {"OptPrettify",-18} = {OptPrettify.ToString().ToLower(),-20} │");
                Console.WriteLine($"│ {"OptOverwriteInFile",-18} = {OptOverwriteInFile.ToString().ToLower(),-20} │");
                Console.WriteLine($"│ {"OptSortNodes",-18} = {OptSortNodes.ToString().ToLower(),-20} │");
                Console.WriteLine($"│ {"OptNodeSortCase",-18} = {OptNodeSortCase,-20} │");
                Console.WriteLine($"│ {"OptSortAttrs",-18} = {OptSortAttrs.ToString().ToLower(),-20} │");
                Console.WriteLine($"│ {"OptAttrSortCase",-18} = {OptAttrSortCase,-20} │");
                Console.WriteLine($"│ {"OptPrimarySortAttr",-18} = {$"'{OptPrimarySortAttr}'",-20} │");
                Console.WriteLine($"│ {"OptNewLineChars",-18} = {$"'{OptNewLineChars}'",-20} │");
                Console.WriteLine($"│ {"OptIndentChars",-18} = {$"'{OptIndentChars}'",-20} │");
                Console.WriteLine($"│ {"OptNewLineOnAttrs",-18} = {OptNewLineOnAttrs.ToString().ToLower(),-20} │");
                Console.WriteLine($"└─{new string('─', 41)}─┘");
            }

            if (OptShowHelp) {
                usage(OptShowExamples);
                return 0;
            }

            if (inFile.Length == 0) {
                Console.WriteLine("**** Missing infile. ****\n");
                usage();
                return 1;
            }

            try {
                doc.PreserveWhitespace = !OptPrettify;
                doc.LoadXml(File.ReadAllText(inFile));
            } catch (Exception ex) {
                Console.WriteLine("**** Could not load input file. ****");
                Console.WriteLine(ex.Message);
                return 100;
            }

            if (OptSortAttrs) {
                // > I don't like defaulting a primary key -
                //   ie: changing an expected behavior without notice/clear understanding..
                // if (string.IsNullOrEmpty(primary_attr)) {
                //     primary_attr = "GUID";
                // }
                SortNodeAttrs(doc.DocumentElement);
            }
            if (OptSortNodes) {
                SortNodes(doc.DocumentElement);
            }

            if (outFile.Length == 0 && OptOverwriteInFile) {
                outFile = inFile;
            }

            settings = new XmlWriterSettings() {
                CloseOutput = true,
                // Encoding = Encoding.UTF8,
                Indent = true, //!string.IsNullOrEmpty(indent_chars),
                IndentChars = OptIndentChars.Replace("\\r", "\r").Replace("\\n", "\n").Replace("\\t", "\t").Replace("\\s", " "),
                NewLineChars = OptNewLineChars.Replace("\\r", "\r").Replace("\\n", "\n").Replace("\\t", "\t").Replace("\\s", " "),
                NewLineHandling = NewLineHandling.Replace,
                NewLineOnAttributes = OptNewLineOnAttrs
            };

            if (outFile.Length > 0) {
                try {
                    if (OptPrettify) {
                        var xmlWriter = XmlWriter.Create(outFile, settings);
                        doc.Save(xmlWriter);
                    } else {
                        doc.Save(outFile);
                    }
                } catch (Exception ex) {
                    Console.WriteLine("**** Could not save output file. ****");
                    Console.WriteLine(ex.Message);
                    return 101;
                }
            } else {
                if (OptPrettify) {
                    var xmlWriter = XmlWriter.Create(Console.Out, settings);
                    doc.Save(xmlWriter);
                } else {
                    doc.Save(Console.Out);
                }
            }

            if (OptPause) {
                Console.Write("Press any key to quit: ");
                Console.ReadKey(true);
                Console.WriteLine();
            }

            return 0;
        }

        static string RemoveOutsideQuotes( string s )
        {
            if ((s.StartsWith('"') && s.EndsWith('"')) || (s.StartsWith('\'') && s.EndsWith('\''))) {
                return s = s.Substring(1, s.Length - 2);
            }
            return s;
        }

        static void SortNodes( XmlNode node )
        {
            // Go down to the furthest child and start there..
            // That is so I can include child nodes in the current node's sort,
            // if all of it's attributes match..
            for (int i = 0, len = node.ChildNodes.Count; i < len; i++) {
                SortNodes(node.ChildNodes[i]);
            }

            // Remove, sort, then re-add the node's children.
            if (OptSortNodes && node.ChildNodes != null && node.ChildNodes.Count > 0) {
                var nodes = new List<XmlNode>(node.ChildNodes.Count);

                for (var i = node.ChildNodes.Count - 1; i >= 0; i--) {
                    nodes.Add(node.ChildNodes[i]);
                    node.RemoveChild(node.ChildNodes[i]);
                }

                nodes.Sort(SortDelegate);

                for (var i = 0; i < nodes.Count; i++) {
                    node.AppendChild(nodes[i]);
                }
            }
        }

        static int SortDelegate( XmlNode a, XmlNode b )
        {
            var result = string.Compare(a.Name, b.Name, OptNodeSortCase);

            // NOTE: Always sort the _nodes_ based on its attributes (when the
            //       name matches), but don't actually sort the node's attributes.
            //       Sorting attributes, if specified, is done before node sorting happens..

            if (result == 0) {
                var col1 = (a.Attributes.Count >= b.Attributes.Count) ? a.Attributes : b.Attributes;
                var col2 = (a.Attributes.Count >= b.Attributes.Count) ? b.Attributes : a.Attributes;

                for (var i = 0; i < col1.Count; i++) {
                    if (i < col2.Count) {
                        var aa = col1[i];
                        var bb = col2[i];
                        result = string.Compare(aa.Name, bb.Name, OptAttrSortCase);
                        if (result == 0) {
                            result = string.Compare(aa.Value, bb.Value, OptAttrSortCase);
                            if (result != 0) {
                                return result;
                            }
                            // Attribute name and value match.. continue loop.
                        } else {
                            return result;
                        }
                    } else {
                        return 1;
                    }
                }

                // If we get here, that means that the node's attributes (and values) all match..
                // TODO: Should we go down into the child node collections for sorting?
                //       See example `c.xml`..
                //Console.WriteLine(a.Name + "==" + b.Name + " all attributes matched");
            }

            return result;
        }

        static void SortNodeAttrs( XmlNode node )
        {
            // Remove, sort, then re-add the node's attributes.
            if (OptSortAttrs && node.Attributes != null && node.Attributes.Count > 0) {
                SortXmlAttributeCollection(node.Attributes);
            }

            // Sort the children node's attributes also.
            for (int i = 0, len = node.ChildNodes.Count; i < len; i++) {
                SortNodeAttrs(node.ChildNodes[i]);
            }
        }

        static void SortXmlAttributeCollection( XmlAttributeCollection col )
        {
            // Remove, sort, then re-add the attributes to the collection.
            if (OptSortAttrs && col != null && col.Count > 0) {
                var attrs = new List<XmlAttribute>(col.Count);

                for (var i = col.Count - 1; i >= 0; i--) {
                    attrs.Add(col[i]);
                    col.RemoveAt(i);
                }

                SortAttributeList(attrs);

                for (var i = 0; i < attrs.Count; i++) {
                    col.Append(attrs[i]);
                }
            }
        }

        static void SortAttributeList( List<XmlAttribute> attrs )
        {
            attrs.Sort(delegate ( XmlAttribute a, XmlAttribute b ) {
                var result = string.Compare(a.Name, b.Name, OptAttrSortCase);
                if (result == 0) {
                    return string.Compare(a.Value, b.Value, OptAttrSortCase);
                } else if (!string.IsNullOrEmpty(OptPrimarySortAttr)) {
                    // If a primary_attr is specified, it is always made the first attribute!
                    if (a.Name.Equals(OptPrimarySortAttr, OptAttrSortCase)) {
                        return -1;
                    } else if (b.Name.Equals(OptPrimarySortAttr, OptAttrSortCase)) {
                        return 1;
                    }
                }
                return result;
            });
        }

        static void usage( bool showExamples = false )
        {
            Console.WriteLine("sortxml - small utility that sorts (and prettifies) xml files.");
            Console.WriteLine("Copyright (c) 2014-2022 Kody Brown (@kodybrown)");
            Console.WriteLine("");
            Console.WriteLine("USAGE: sortxml [options] infile [outfile]");
            Console.WriteLine("");
            Console.WriteLine("  infile        The name of the file to sort, etc.");
            Console.WriteLine("  outfile       The name of the file to save the output to.");
            Console.WriteLine("                If outfile is omitted, the output is written to stdout,");
            Console.WriteLine("                unless `--overwrite` is specified, in which case the");
            Console.WriteLine("                output is written back to infile, overwriting it.");
            Console.WriteLine("");
            Console.WriteLine("OPTIONS:");
            Console.WriteLine("");
            Console.WriteLine("  /? --help [-examples]  Shows this help (optionally with examples).");
            Console.WriteLine("  /p --pause             Pauses when finished.");
            Console.WriteLine("  /e --debug             Displays debug info and details.");
            Console.WriteLine("");
            Console.WriteLine("  --pretty               Ignores the input format and prettifies the output (default).");
            Console.WriteLine("  --new-line-chars=x     Specifies the character(s) to use for each new line.");
            Console.WriteLine("  --new-line-on-attrs    Separates each attribute onto its own line.");
            Console.WriteLine("  --indent-chars=x       Specifies the characher(s) for the indentation.");
            Console.WriteLine("");
            Console.WriteLine("  /s --sort              Sort both the nodes and attributes. (default)");
            Console.WriteLine("  --sort-node            Sort the nodes.");
            Console.WriteLine("  --sort-attr            Sort the attributes.");
            Console.WriteLine("                         If any sort is specified, '--pretty' is assumed.");
            Console.WriteLine("  /i --case-insensitive  Sorts node and attributes without regard to letter case (default).");
            Console.WriteLine("     --node-case-insensitive");
            Console.WriteLine("     --attr-case-insensitive");
            Console.WriteLine("  /t --case-sensitive    Sorts node and attributes case-sensitively.");
            Console.WriteLine("     --node-case-sensitive");
            Console.WriteLine("     --attr-case-sensitive");
            Console.WriteLine("");
            Console.WriteLine("  --overwrite            Writes back to the infile. Ignored if outfile is specified.");
            Console.WriteLine("");
            // Console.WriteLine("  --primary-node=x       This specified node will always be sorted first.");
            Console.WriteLine("  --primary-attr=x       This specified attribute will always be sorted first.");
            // Console.WriteLine("                         Use commas to specify multiple attribute priorities.");
            Console.WriteLine("");
            Console.WriteLine("  The '-' and '--' prefixes are interchangable (flags cannot be combined).");
            Console.WriteLine("  Add a '!' after the prefix, to turn the flag off.");
            Console.WriteLine("  This utility uses the Microsoft XML .NET namespace.");
            Console.WriteLine("");

            Console.WriteLine("  Type `sortxml --help examples` to display some examples.");
            Console.WriteLine("");

            if (showExamples) {
                Console.WriteLine("EXAMPLES:");
                Console.WriteLine("");
                Console.WriteLine("> type sample.xml");
                Console.WriteLine("  <?xml version=\"1.0\" encoding=\"utf-8\" ?><root><node value=\"one\" name=\"xyz\"/><node2 name=\"abc\" value=\"two\"/></root>");
                Console.WriteLine("");
                Console.WriteLine("> sortxml sample.xml");
                Console.WriteLine("  <?xml version=\"1.0\" encoding=\"utf-8\"?>");
                Console.WriteLine("  <root>");
                Console.WriteLine("      <node name=\"xyz\" value=\"one\" />");
                Console.WriteLine("      <node2 name=\"abc\" value=\"two\" />");
                Console.WriteLine("  </root>");
                Console.WriteLine("");
                Console.WriteLine("> sortxml sample.xml -!pretty");
                Console.WriteLine("  <?xml version=\"1.0\" encoding=\"utf-8\"?><root><node name=\"xyz\" value=\"one\" /><node2 name=\"abc\" value=\"two\" /></root>");
                Console.WriteLine("");
                Console.WriteLine("> sortxml sample.xml -primary-attr=value");
                Console.WriteLine("  <?xml version=\"1.0\" encoding=\"utf-8\"?>");
                Console.WriteLine("  <root>");
                Console.WriteLine("      <node value=\"one\" name=\"xyz\" />");
                Console.WriteLine("      <node2 value=\"two\" name=\"abc\" />");
                Console.WriteLine("  </root>");
                Console.WriteLine("");
                Console.WriteLine("> sortxml sample.xml -indent-chars=' '");
                Console.WriteLine("  <?xml version=\"1.0\" encoding=\"utf-8\"?>");
                Console.WriteLine("  <root>");
                Console.WriteLine("   <node name=\"xyz\" value=\"one\" />");
                Console.WriteLine("   <node2 name=\"abc\" value=\"two\" />");
                Console.WriteLine("  </root>");
                Console.WriteLine("");
                Console.WriteLine("> sortxml sample.xml -indent-chars=' ' -new-line-on-attrs");
                Console.WriteLine("  <?xml version=\"1.0\" encoding=\"utf-8\"?>");
                Console.WriteLine("  <root>");
                Console.WriteLine("   <node");
                Console.WriteLine("    name=\"xyz\"");
                Console.WriteLine("    value=\"one\" />");
                Console.WriteLine("   <node2");
                Console.WriteLine("    name=\"abc\"");
                Console.WriteLine("    value=\"two\" />");
                Console.WriteLine("  </root>");
                Console.WriteLine("");
            }
        }
    }
}
