# sortxml

![dotnet-core-build](https://github.com/kodybrown/sortxml/workflows/dotnet-core-build/badge.svg)
![dotnet-core-release](https://github.com/kodybrown/sortxml/workflows/dotnet-core-release/badge.svg)

Simple utility that sorts (and prettifies) xml files. It uses the Microsoft XML .NET namespace.

Click here for the [latest release](https://github.com/kodybrown/sortxml/releases/latest/).

---

## Build

```powershell
cd "{source-folder}"
dotnet build -c Debug

# Run it..
dotnet run -- /?
dotnet run -- -debug .\test_files\f.xml .\test_files\f_sorted.xml -indent-chars='\t'
dotnet run -- -debug .\test_files\f.xml -indent-chars='\t'

# Build a single binary for Windows
cd "{source-folder}"
dotnet publish -c Release --runtime win-x64 --framework net6.0 -p:PublishSingleFile=true -p:PublishTrimmed=true --self-contained=true
```

- Build platforms: https://docs.microsoft.com/en-us/dotnet/core/rid-catalog.

## Code Format

There is an .editorconfig and matching omnisharp.json file included.

Installing and using the dotnet format tool.

```powershell
dotnet tool install -g dotnet-format
dotnet format sortxml.csproj
```

## Usage

```text
USAGE: sortxml [options] infile [outfile]

  infile        The name of the file to sort, etc.
  outfile       The name of the file to save the output to.
                If outfile is omitted, the output is written to stdout,
                unless `--overwrite` is specified, in which case the
                output is written back to infile, overwriting it.

OPTIONS:

  /? --help [examples]   Shows this help (optionally with examples).
  /p --pause             Pauses when finished.
  /e --debug             Displays debug info and details.

  --pretty               Ignores the input format and prettifies the output (default).
  --new-line-chars=x     Specifies the character(s) to use for each new line.
  --new-line-on-attrs    Separates each attribute onto its own line.
  --indent-chars=x       Specifies the characher(s) for the indentation.

  /s --sort              Sort both the nodes and attributes. (default)
  --sort-node            Sort the nodes.
  --sort-attr            Sort the attributes.
                         If any sort is specified, '--pretty' is assumed.
  /i --case-insensitive  Sorts node and attributes without regard to letter case (default).
  /t --case-sensitive    Sorts node and attributes case-sensitively.

  --overwrite            Writes back to the infile. Ignored if outfile is specified.

  --primary-attr=x       This specified attribute will always be sorted first.

  The '-' and '--' prefixes are interchangable (flags cannot be combined).
  Add a '!' after the prefix, to turn the flag off.
  This utility uses the Microsoft XML .NET namespace.

  Type `sortxml --help examples` to display some examples.

EXAMPLES:

> type sample.xml
  <?xml version="1.0" encoding="utf-8" ?><root><node value="one" name="xyz"/><node2 name="abc" value="two"/></root>

> sortxml sample.xml
  <?xml version="1.0" encoding="utf-8"?>
  <root>
      <node name="xyz" value="one" />
      <node2 name="abc" value="two" />
  </root>

> sortxml sample.xml -!pretty
  <?xml version="1.0" encoding="utf-8"?><root><node name="xyz" value="one" /><node2 name="abc" value="two" /></root>

> sortxml sample.xml -primary-attr=value
  <?xml version="1.0" encoding="utf-8"?>
  <root>
      <node value="one" name="xyz" />
      <node2 value="two" name="abc" />
  </root>

> sortxml sample.xml -indent-chars=' '
  <?xml version="1.0" encoding="utf-8"?>
  <root>
   <node name="xyz" value="one" />
   <node2 name="abc" value="two" />
  </root>

> sortxml sample.xml -indent-chars=' ' -new-line-on-attrs
  <?xml version="1.0" encoding="utf-8"?>
  <root>
   <node
    name="xyz"
    value="one" />
   <node2
    name="abc"
    value="two" />
  </root>
```
