sortxml
=======

This is a  very simple utility that prettifies and sorts xml files.

    USAGE: sortxml.exe [options] infile [outfile]

      infile      The name of the file to sort, etc.
      outfile     The name of the file to save the output to.
                  If this is omitted, then the output is written to stdout.

    OPTIONS:

      --pretty    Ignores the input formatting and makes the output look nice.
      --sort      Sort both the nodes and attributes.
      --sortnode  Sort the nodes.
      --sortattr  Sort the attributes.

    (prefix an option with ! to turn it off.)


The default is to output pretty and sorted nodes and attributes. So, given the following:

    > type sample.xml
    <?xml version="1.0" encoding="utf-8" ?><root><node value="one" attr="name"/></root>

    > sortxml.exe sample.xml

Would output the following:

    <?xml version="1.0" encoding="utf-8"?>
    <root>
      <node attr="name" value="one" />
    </root>
