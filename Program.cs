using CommandLine;
using CommandLine.Text;

namespace FunnyGalleryCompositor;

//            █   ▞
//    █████  █████  █████
//        █   █  █      █
//      ▄█▀   █  █      █
//    ▀▀     ▀  ▀   ▀▀▀▀▀

internal static class Program
{
    private static void Main(string[] args)
    {
        var parser = new Parser(with => with.HelpWriter = null);
        var result = parser.ParseArguments<Options>(args);
        result
            .WithParsed(o => new Processor(o).Run())
            .WithNotParsed(_ => DisplayHelp(result));
    }

    private static void DisplayHelp<T>(ParserResult<T> result)
    {  
        var helpText = HelpText.AutoBuild(result, help =>
        {
            help.Heading = "FUGACO 1.2.1-full-dub-milka-MGL-9+10--extra-epic";
            help.Copyright = string.Empty;
            return HelpText.DefaultParsingErrorsHandler(result, help);
        });
        Console.WriteLine(helpText);
    }
}