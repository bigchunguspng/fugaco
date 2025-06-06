using CommandLine;

namespace FunnyGalleryCompositor;

public class Options
{
    [Option('i', "input", Required = false, HelpText = "Directories to grab images from.", Default = new[] { "." })]
    public IEnumerable<string> InputPaths { get; set; } = null!;

    [Option('o', "output", Required = false, HelpText = "Directory to put results to.", Default = "./fugaco")]
    public string OutputPath { get; set; } = null!;

    [Option('e', "extensions", Required = false, HelpText = "Supported extensions.", Default = new[] { ".png", ".jpg", ".jpeg" })]
    public IEnumerable<string> Extensions { get; set; } = null!;

    [Option('r', "recursive", Required = false, HelpText = "Grab images from subdirectories.")]
    public bool Recursive { get; set; }

    [Option('n', "number", Required = false, HelpText = "Number of output images.", Default = 4)]
    public int OutputNumber { get; set; }

    [Option('s', "size", Required = false, HelpText = "Size of photos in gallery, px.", Default = 240)]
    public int ImageWidth { get; set; }

    [Option('m', "margin", Required = false, HelpText = "(Default: size/24) Spacing between photos, px.")]
    public int MarginWidth { get; set; } = -1;

    [Option('c', "checkbox", Required = false, HelpText = "Add decorative checkboxes.")]
    public bool Checkbox { get; set; }
}