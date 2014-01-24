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
