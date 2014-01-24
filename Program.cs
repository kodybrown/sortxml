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

                    if (al.Equals("sort") || al.StartsWith("sortall") || al.StartsWith("sort-all"))
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

            if (sort_node || sort_attr)
            {
                sort(doc.DocumentElement);
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

        static void sort(XmlNode node)
        {
            if (sort_attr)
            {
                sortattr(node);
            }

            // Remove, sort, then re-add the node's children.
            if (sort_node && node.ChildNodes != null && node.ChildNodes.Count > 0)
            {
                List<XmlNode> nodes = new List<XmlNode>(node.ChildNodes.Count);
                int result;

                for (int i = node.ChildNodes.Count - 1; i >= 0; i--)
                {
                    nodes.Add(node.ChildNodes[i]);
                    node.RemoveChild(node.ChildNodes[i]);
                }

                nodes.Sort(delegate(XmlNode a, XmlNode b)
                {
                    result = string.Compare(a.Name, b.Name, StringComparison.CurrentCultureIgnoreCase);
                    if (result == 0)
                    {
                        // todo: must include the attributes in the sorting..
                    }
                    return result;
                });

                for (int i = 0; i < nodes.Count; i++)
                {
                    node.AppendChild(nodes[i]);
                }
            }

            for (int i = 0, len = node.ChildNodes.Count; i < len; i++)
            {
                sort(node.ChildNodes[i]);
            }
        }

        static void sortattr(XmlNode node)
        {
            // Remove, sort, then re-add the node's attributes.
            if (sort_attr && node.Attributes != null && node.Attributes.Count > 0)
            {
                List<XmlAttribute> attrs = new List<XmlAttribute>(node.Attributes.Count);

                for (int i = node.Attributes.Count - 1; i >= 0; i--)
                {
                    attrs.Add(node.Attributes[i]);
                    node.Attributes.RemoveAt(i);
                }

                attrs.Sort(delegate(XmlAttribute a, XmlAttribute b)
                {
                    return string.Compare(a.Name, b.Name, StringComparison.CurrentCultureIgnoreCase);
                });

                for (int i = 0; i < attrs.Count; i++)
                {
                    node.Attributes.Append(attrs[i]);
                }
            }
        }


        static void usage()
        {
            Console.WriteLine("usage: sortxml infile [outfile]");

        }
    }
}
