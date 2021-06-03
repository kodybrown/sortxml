using System;
using Xunit;
using sortxml;
using System.Reflection;
using System.IO;
using System.Linq;

namespace sortxmlXUnitProject
{
  public class UnitTestAll
  {
    static string GetTestFilesPath()
    {
      var codeBaseUrl = new Uri(Assembly.GetExecutingAssembly().CodeBase);
      var codeBasePath = Uri.UnescapeDataString(codeBaseUrl.AbsolutePath);
      var dirPath = Path.GetFullPath(Path.GetDirectoryName(codeBasePath) + @"/../../../test_files/");
      return dirPath;
    }

    static bool CompareFiles(string baseFilePath, string generatedTestFilePath)
    {
      var testData = File.ReadAllBytes(generatedTestFilePath);
      var baseData = File.ReadAllBytes(baseFilePath);
      return testData.SequenceEqual(baseData);
    }

    [Fact]
    public void TestAllFiles()
    {
      var testFilesPath = GetTestFilesPath();
      foreach(var file in Directory.GetFiles(testFilesPath, "*.xml"))
      {
        if (!file.Contains("_handsorted.xml") && !file.Contains("_sorted.xml") && !file.Contains("_test.xml"))
        {
          var name = Path.GetFileNameWithoutExtension(file);
          var resultFile = testFilesPath + name + "_test.xml";
          var baseFile = testFilesPath + name + "_sorted.xml";
          sortxml.Program.Main(new string[] { "--sort", file, resultFile});
          Assert.True(CompareFiles(baseFile, resultFile), "Comparing " + file);
          File.Delete(resultFile);
        }
      }
    }
  }
}
