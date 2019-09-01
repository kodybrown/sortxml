sortxml
=======

This is a very simple utility that prettifies and sorts xml files.
It uses the Microsoft XML .NET namespace.

Copyright 2014-2019 Kody Brown (@wasatchwizard)

    USAGE: sortxml.exe [options] infile [outfile]

      infile        The name of the file to sort, etc.

      outfile       The name of the file to save the output to.
                    If outfile is omitted, the output is written to stdout,
                    unless `--overwrite` is specified, in which case the
                    output is written back to infile, overwriting it.

    OPTIONS:

      /p --pause    Pauses when finished.

      --pretty      Ignores the input format and makes the output look nice.
                    This is the default.

      /s --sort     Sort both the nodes and attributes.
      --sort-node   Sort the nodes.
      --sort-attr   Sort the attributes.
                    If a sort is specified, '--pretty' is assumed.
                    If a sort is NOT is specified, both nodes and attributes
                    will be sorted.

      /i --case-insensitive
                    Sorts node and attributes without regard to letter case.
      !i --case-sensitive
                    Sorts node and attributes case-sensitively.
                    If neither option is specified, uses case-sensitive sort.

      --overwrite   Writes back to the infile.
                    Only used if outfile is not specified.

    Prefix an option with '!' to turn it off.
    The '!' can be applied with or without one of the other prefixes.
    The '/' and '--' prefixes are interchangable.

The default is to output pretty and sorted nodes and attributes:

    > type sample.xml
    <?xml version="1.0" encoding="utf-8" ?><root><node value="one" attr="name"/><node2 attr="name" value="two" /></root>

    > sortxml.exe sample.xml
    <?xml version="1.0" encoding="utf-8"?>
    <root>
      <node attr="name" value="one" />
      <node2 attr="name" value="two" />
    </root>
