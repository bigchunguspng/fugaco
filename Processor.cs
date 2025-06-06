using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Path = System.IO.Path;

namespace FunnyGalleryCompositor;

public class Processor(Options options)
{
    public void Run()
    {
        if (MissingInputs()) return;

        var groups = GetFileGroups();
        if (groups is null) return;

        if (FailedToCreateOutput()) return;

        AdjustOptions();
        RenderCollages(groups);
    }

    private bool MissingInputs()
    {
        var exit = false;
        foreach (var path in options.InputPaths.Where(dir => !Directory.Exists(dir)))
        {
            exit = true;
            PrintError($"[x_x] Directory \"{path}\" was not found.");
        }

        return exit;
    }

    private bool FailedToCreateOutput()
    {
        try
        {
            Directory.CreateDirectory(options.OutputPath);
            return false;
        }
        catch
        {
            PrintError($"[x_x] Failed to create directory \"{options.OutputPath}\".");
            return true;
        }
    }

    private string[][]? GetFileGroups()
    {
        var searchOption = options.Recursive
            ? SearchOption.AllDirectories
            : SearchOption.TopDirectoryOnly;
        var files = options.InputPaths
            .SelectMany(path => Directory
                .GetFiles(path, "*", searchOption)
                .Where(file => options.Extensions.Any(file.EndsWith)))
            .ToList();

        var totalFiles = files.Count;
        if (totalFiles == 0)
        {
            PrintError("[x_x] No files were found.");
            return null;
        }

        Print($"FILES FOUND: {totalFiles}\n");

        var groupCount = options.OutputNumber;
        var groups = new string[groupCount][];
        for (var i = 0; i < groupCount; i++)
        {
            const int length = 16;
            groups[i] = new string[length];
            for (var j = 0; j < length; j++)
            {
                groups[i][j] = files[Random.Shared.Next(totalFiles)];
            }
        }

        return groups;
    }

    private void AdjustOptions()
    {
        if (options.MarginWidth < 0)
            options.MarginWidth = (int)Math.Round(options.ImageWidth / 24F);
        
        Print($"IMAGE SIZE: {options.ImageWidth,4} px    SPACING: {options.MarginWidth,4} px\n");
    }

    private void RenderCollages(string[][] groups)
    {
        var timestamp = DateTime.UtcNow.Ticks;

        var  width = options.ImageWidth;
        var margin = options.MarginWidth;

        var  targetSize = new Size(width);
        var collageSize = new Size(4 * width + 5 * margin);

        var collageNumber = 1;
        foreach (var group in groups)
        {
            using var collage = new Image<Rgb24>(collageSize.Width, collageSize.Height, new Rgb24(255, 255, 255));

            Print($"COLLAGE #{collageNumber:00}: {AxB(collageSize), 9}\n");

            var paste = new Point(margin, margin);

            var imageNumber = 1;
            foreach (var file in group)
            {
                using var image = Image.Load(file);

                Print($"\tIMAGE #{imageNumber:00}: {AxB(image.Width, image.Height),9}");

                var crop = GetCropSquare(image.Size);

                Print($" --> crop: {AxB(crop.X, crop.Y),9}, {AxB(crop.Width, crop.Height),9}");
                Print($" --> scale: {AxB(targetSize),9}");
                Print($" --> paste: {AxB(paste),9}\n");

                image.Mutate(ctx => ctx.Crop(crop).Resize(targetSize));
                if (options.Checkbox)
                {
                    DrawCheckbox(image, width);
                }
                collage.Mutate(ctx => ctx.DrawImage(image, paste, opacity: 1F));

                paste.X += width + margin;
                if (paste.X >= collageSize.Width)
                {
                    paste.X = margin;
                    paste.Y += width + margin;
                }

                imageNumber++;
            }

            collage.Save(Path.Combine(options.OutputPath, $"FUGACO-{timestamp}-{collageNumber:00}.jpg"));
            collageNumber++;
        }
    }

    private static Rectangle GetCropSquare(Size source)
    {
        var albumLike = source.Width > source.Height;
        var size = albumLike
            ? new Size(source.Height)
            : new Size(source.Width);
        var point = albumLike
            ? new Point((source.Width - source.Height) / 2, 0)
            : new Point(0, (source.Height - source.Width) / 2);
        return new Rectangle(point, size);
    }

    private static void DrawCheckbox(Image image, int width)
    {
        var thickness = width / 75F;
        var offset = width * 0.15F;
        var center = new PointF(width - offset - 1, offset);
        var radius = width * 0.088F;
        var circle = new EllipsePolygon(center, radius);
        var options = new DrawingOptions
        {
            GraphicsOptions = new GraphicsOptions
            {
                BlendPercentage = 1F,
                ColorBlendingMode = PixelColorBlendingMode.Multiply
            }
        };
        image.Mutate(ctx =>
        {
            ctx.Fill(options, Color.FromRgb(243, 243, 243), circle);
            ctx.Draw(Color.White, thickness, circle);
        });
    }

    private static string AxB(Point p) => AxB(p.X, p.Y);
    private static string AxB(Size  s) => AxB(s.Width, s.Height);
    private static string AxB(int a, int b) => $"{a}x{b}";

    private static void Print     (string message) => Console.Write(message);
    private static void PrintError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ResetColor();
    }
}