// See https://aka.ms/new-console-template for more information
using LexicalAnalyzer.Services;
 
foreach (var file in ReadJsFiles())
{
    Console.WriteLine("----------------------");
    try
    {
        Console.WriteLine(file.fileName);
        Console.WriteLine("----------------------\n");
        var results = AnalyzerService.Analyze(file.content);
        foreach (var result in results)
        {
            string valueToShow = result.lexeme == LexicalAnalyzer.Domain.Models.LexemeType.Nl ? "\\n" : result.value;
            Console.WriteLine($"{result.lexeme} - {valueToShow}");
        }
    }
    catch (ArgumentException ex)
    {
        Console.WriteLine(ex.Message);
    }

    Console.WriteLine("----------------------\n");
}


static IEnumerable<(string fileName, string content)> ReadJsFiles()
{
    var sampleFileNames = Directory.GetFiles("JsFiles").Where(f => Path.GetExtension(f) is ".js").ToArray();
    foreach (var fileName in sampleFileNames)
    {
        yield return (fileName, File.ReadAllText(fileName));
    }
}