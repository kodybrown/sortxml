using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace sortxml
{
    class Program
    {
        static bool sort_node = true,
            sort_attr = true,
            pretty = true;
        static StringComparison
            sort_node_comp = StringComparison.CurrentCulture, // Default to case-sensitive sorting.
            sort_attr_comp = StringComparison.CurrentCulture;

        static int Main(string[] arguments)
        {
            XmlDocument doc;
            string inf = "",
                outf = "";

            doc = new XmlDocument();

            for (int i = 0; i < arguments.Length; i++)
            {
                string a = arguments[i];

                if (a[0] == '-' || a[0] == '/' || a[0] == '!')
                {
                    while (a[0] == '-' || a[0] == '/')
                    {
                        a = a.Substring(1);
                    }
                    string al = a.ToLower();

                    if (al.Equals("i") || al.StartsWith("casei") || al.StartsWith("case-i"))
                    {
                        sort_node_comp = StringComparison.CurrentCultureIgnoreCase;
                        sort_attr_comp = StringComparison.CurrentCultureIgnoreCase;
                    }
                    else if (al.Equals("!i") || al.StartsWith("cases") || al.StartsWith("case-s"))
                    {
                        sort_node_comp = StringComparison.CurrentCulture;
                        sort_attr_comp = StringComparison.CurrentCulture;
                    }
                    else if (al.Equals("sort") || al.StartsWith("sortall") || al.StartsWith("sort-all"))
                    {
                        sort_node = true;
                        sort_attr = true;
                    }
                    else if (al.Equals("!sort") || al.StartsWith("!sortall") || al.StartsWith("!sort-all"))
                    {
                        sort_node = false;
                        sort_attr = false;
                    }
                    else if (al.StartsWith("sortn") || al.StartsWith("sort-n"))
                    {
                        sort_node = true;
                    }
                    else if (al.StartsWith("!sortn") || al.StartsWith("!sort-n"))
                    {
                        sort_node = false;
                    }
                    else if (al.StartsWith("sorta") || al.StartsWith("sort-a"))
                    {
                        sort_attr = true;
                    }
                    else if (al.StartsWith("!sorta") || al.StartsWith("!sort-a"))
                    {
                        sort_attr = false;
                    }
                    else if (al.StartsWith("pretty"))
                    {
                        pretty = true;
                    }
                    else if (al.StartsWith("!pretty"))
                    {
                        pretty = false;
                    }
                }
                else
                {
                    if (inf.Length == 0)
                    {
                        inf = a;
                    }
                    else if (outf.Length == 0)
                    {
                        outf = a;
                    }
                    else
                    {
                        Console.WriteLine("**** Unknown command: " + a);
                    }
                }
            }

            if (inf.Length == 0)
            {
                usage();
                return 1;
            }

            try
            {
                doc.LoadXml(File.ReadAllText(inf));
                doc.PreserveWhitespace = !pretty;
            }
            catch (Exception ex)
            {
                Console.WriteLine("**** Could not load input file");
                Console.WriteLine(ex.Message);
                return 100;
            }

            if (sort_attr)
            {
                SortNodeAttrs(doc.DocumentElement);
            }
            if (sort_node)
            {
                SortNodes(doc.DocumentElement);
            }

            if (outf.Length > 0)
            {
                try
                {
                    doc.Save(outf);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("**** Could not save output file");
                    Console.WriteLine(ex.Message);
                    return 101;
                }
            }
            else
            {
                doc.Save(Console.Out);
            }

            return 0;
        }

        static void SortNodes(XmlNode node)
        {
            // Go down to the furthest child and start there..
            // That is so I can include child nodes in the current node's sort,
            // if all of it's attributes match..
            for (int i = 0, len = node.ChildNodes.Count; i < len; i++)
            {
                SortNodes(node.ChildNodes[i]);
            }

            // Remove, sort, then re-add the node's children.
            if (sort_node && node.ChildNodes != null && node.ChildNodes.Count > 0)
            {
                List<XmlNode> nodes = new List<XmlNode>(node.ChildNodes.Count);

                for (int i = node.ChildNodes.Count - 1; i >= 0; i--)
                {
                    nodes.Add(node.ChildNodes[i]);
                    node.RemoveChild(node.ChildNodes[i]);
                }

                nodes.Sort(SortDelegate);

                for (int i = 0; i < nodes.Count; i++)
                {
                    node.AppendChild(nodes[i]);
                }
            }
        }

        static int SortDelegate(XmlNode a, XmlNode b)
        {
            XmlAttribute aa, bb;
            XmlAttributeCollection col1, col2;
            int result;

            result = string.Compare(a.Name, b.Name, sort_node_comp);
            // NOTE: Always sort the _nodes_ based on its attributes (when the 
            //       name matches), but don't actually sort the node's attributes.
            //       (Sorting attributes is done before node sorting happens,
            //       if specified).
            if (result == 0)
            {
                col1 = (a.Attributes.Count >= b.Attributes.Count) ? a.Attributes : b.Attributes;
                col2 = (a.Attributes.Count >= b.Attributes.Count) ? b.Attributes : a.Attributes;

                for (int i = 0; i < col1.Count; i++)
                {
                    if (i < col2.Count)
                    {
                        aa = col1[i];
                        bb = col2[i];
                        result = string.Compare(aa.Name, bb.Name, sort_attr_comp);
                        if (result == 0)
                        {
                            result = string.Compare(aa.Value, bb.Value, sort_attr_comp);
                            if (result != 0)
                            {
                                return result;
                            }
                            // Attribute name and value match.. continue loop.
                        }
                        else
                        {
                            return result;
                        }
                    }
                    else
                    {
                        return 1;
                    }
                }

                // If we get here, that means that the node's attributes (and values) all match..
                // TODO: Should we go down into the child node collections for sorting?
                //       See example `c.xml`..
                Console.WriteLine(a.Name + "==" + b.Name + " all attributes matched");
            }

            return result;
        }

        static void SortNodeAttrs(XmlNode node)
        {
            // Remove, sort, then re-add the node's attributes.
            if (sort_attr && node.Attributes != null && node.Attributes.Count > 0)
            {
                SortXmlAttributeCollection(node.Attributes);
            }

            // Sort the children node's attributes also.
            for (int i = 0, len = node.ChildNodes.Count; i < len; i++)
            {
                SortNodeAttrs(node.ChildNodes[i]);
            }
        }

        static void SortXmlAttributeCollection(XmlAttributeCollection col)
        {
            // Remove, sort, then re-add the attributes to the collection.
            if (sort_attr && col != null && col.Count > 0)
            {
                List<XmlAttribute> attrs = new List<XmlAttribute>(col.Count);

                for (int i = col.Count - 1; i >= 0; i--)
                {
                    attrs.Add(col[i]);
                    col.RemoveAt(i);
                }

                SortAttributeList(attrs);

                for (int i = 0; i < attrs.Count; i++)
                {
                    col.Append(attrs[i]);
                }
            }
        }

        static void SortAttributeList(List<XmlAttribute> attrs)
        {
            int result;

            attrs.Sort(delegate(XmlAttribute a, XmlAttribute b)
            {
                result = string.Compare(a.Name, b.Name, sort_attr_comp);
                if (result == 0)
                {
                    return string.Compare(a.Value, b.Value, sort_attr_comp);
                }
                return result;
            });
        }

        static void usage()
        {
            Console.WriteLine("usage: sortxml infile [outfile]");

        }
    }
}
