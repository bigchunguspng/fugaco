using CommandLine;

namespace FunnyGalleryCompositor;

public class Options
{
    [Option('i', "input", Required = false, HelpText = "Directories to grab images from.", Default = new[] { "." })]
    public IEnumerable<string> InputPaths { get; set; } = null!;

    [Option('o', "output", Required = false, HelpText = "Directory to put results to.", Default = "./fugaco")]
    public string OutputPath { get; set; } = null!;

    [Option('e', "extensions", Required = false, HelpText = "Directories to grab images from.", Default = new[] { ".png", ".jpg", ".jpeg" })]
    public IEnumerable<string> Extensions { get; set; } = null!;

    [Option('r', "recursive", Required = false, HelpText = "Grab images from subdirectories.")]
    public bool Recursive { get; set; }

    [Option('n', "number", Required = false, HelpText = "Number of output images.", Default = 4)]
    public int OutputNumber { get; set; }

    [Option('s', "size", Required = false, HelpText = "Width and height of photos in gallery.", Default = 240)]
    public int ImageWidth { get; set; }

    [Option('m', "margin", Required = false, HelpText = "Width and height of margins between photos.", Default = 10)]
    public int MarginWidth { get; set; }

    [Option('c', "checkbox", Required = false, HelpText = "Add decorative checkboxes.")]
    public bool Checkbox { get; set; }
}