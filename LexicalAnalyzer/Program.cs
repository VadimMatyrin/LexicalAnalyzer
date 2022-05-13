// See https://aka.ms/new-console-template for more information
using LexicalAnalyzer.Domain.Models;
using LexicalAnalyzer.Services;
using LexicalAnalyzer.Services.Services;
using System.Data;

Console.WriteLine("Parser for while statement");
RunParserForWhile();
Console.WriteLine("Ok");
Console.WriteLine("-----------------");
try
{

    Console.WriteLine("Parser for while witn invalid bool");
    RunParserForWhileWithInvalidBool();
    Console.WriteLine("Ok");
    Console.WriteLine("-----------------");

}
catch (InvalidExpressionException e)
{
    Console.WriteLine(e.Message);
    Console.WriteLine("Error");
    Console.WriteLine("-----------------");
}

Console.WriteLine("Parser for while with math");
RunParserForWhileWithMath();
Console.WriteLine("Ok");
Console.WriteLine("-----------------");


Console.WriteLine("Parser for math");
RunParserForMath();
Console.WriteLine("Ok");
Console.WriteLine("-----------------");

try
{
    RunParserForWhileWithMathWithNoClosingBracket();
}
catch (InvalidExpressionException e)
{
    Console.WriteLine(e.Message);
}

static void RunAnalyzer()
{
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
}

static void RunParserForWhile()
{
    var testExpression = new[]
    {LexemeType.While, LexemeType.Lp, LexemeType.Id, LexemeType.RelOp, LexemeType.Number, LexemeType.Rp,
        LexemeType.Lb, LexemeType.Id, LexemeType.Assign, LexemeType.Number, LexemeType.Sc,LexemeType.Rb };

    var test = new LexemParser(testExpression);

    test.ParseWhileStatement();
}

static void RunParserForWhileWithInvalidBool()
{
    var testExpression = new[]
    {LexemeType.While, LexemeType.Lp, LexemeType.Id, LexemeType.AddOp, LexemeType.Number, LexemeType.Rp,
        LexemeType.Lb, LexemeType.Id, LexemeType.Assign, LexemeType.Number, LexemeType.Sc,LexemeType.Rb };

    var test = new LexemParser(testExpression);

    test.ParseWhileStatement();
}


static void RunParserForWhileWithMath()
{
    var testExpression = new[]
    {LexemeType.While, LexemeType.Lp, LexemeType.Number, LexemeType.AddOp, LexemeType.Number, LexemeType.RelOp, LexemeType.Id, LexemeType.Rp,
        LexemeType.Lb, LexemeType.Id, LexemeType.Assign, LexemeType.Number, LexemeType.Sc,LexemeType.Rb };

    var test = new LexemParser(testExpression);

    test.ParseWhileStatement();
}

static void RunParserForWhileWithMathWithNoClosingBracket()
{
    var testExpression = new[]
    {LexemeType.While, LexemeType.Lp, LexemeType.Number, LexemeType.AddOp, LexemeType.Number, LexemeType.RelOp, LexemeType.Id, LexemeType.Rp,
        LexemeType.Lb, LexemeType.Id, LexemeType.Assign, LexemeType.Number, LexemeType.Sc };

    var test = new LexemParser(testExpression);

    test.ParseWhileStatement();
}

static void RunParserForMath()
{
    var testExpression = new[]
    {LexemeType.Lp, LexemeType.Number, LexemeType.AddOp, LexemeType.Number, LexemeType.Rp };

    var test = new LexemParser(testExpression);

    test.ParseMathExpression();
}

static IEnumerable<(string fileName, string content)> ReadJsFiles()
{
    var sampleFileNames = Directory.GetFiles("JsFiles").Where(f => Path.GetExtension(f) is ".js").ToArray();
    foreach (var fileName in sampleFileNames)
    {
        yield return (fileName, File.ReadAllText(fileName));
    }
}