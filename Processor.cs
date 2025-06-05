using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace FunnyGalleryCompositor;

public class Processor(Options options)
{
    public void Run()
    {
        if (MissingInputs() || FailedToCreateOutput()) return;

        var files = options.InputPaths
            .SelectMany(dir => Directory.GetFiles(dir).Where(file => options.Extensions.Any(file.EndsWith)))
            .ToList();

        var totalFiles = files.Count;
        Print($"FILES FOUND: {totalFiles}\n");

        var groups = new string[options.OutputNumber][];
        for (var i = 0; i < options.OutputNumber; i++)
        {
            var randomFiles = new string[16];
            for (var j = 0; j < randomFiles.Length; j++)
            {
                randomFiles[j] = files[Random.Shared.Next(totalFiles)];
            }

            groups[i] = randomFiles;
        }

        var width = options.ImageWidth;
        var margin = options.MarginWidth;
        var collageWidth = 4 * width + 5 * margin;
        var runTimestamp = DateTime.UtcNow.Ticks;
        var collageNumber = 1;
        foreach (var group in groups)
        {
            Print($"COLLAGE #{collageNumber:00}: {AxB(collageWidth, collageWidth), 9}\n");
            var pasteX = margin;
            var pasteY = margin;
            var imageNumber = 1;
            using var collage = new Image<Rgb24>(collageWidth, collageWidth, new Rgb24(255, 255, 255));
            foreach (var file in group)
            {
                using var image = Image.Load(file);
                var aspectRatio = (double)image.Width / image.Height;
                var albumLike = aspectRatio > 1;
                var w = (int)(albumLike ? width * aspectRatio : width);
                var h = (int)(albumLike ? width : width / aspectRatio);
                var x = albumLike ? (w - width) / 2 : 0;
                var y = albumLike ? 0 : (h - width) / 2;
                var cropRect = new Rectangle(x, y, width, width);
                var pastePoint = new Point(pasteX, pasteY);
                Print($"\tIMAGE #{imageNumber:00}: {AxB(image.Width, image.Height),9}");
                Print($" --> scale: {AxB(w, h),9}");
                Print($" --> crop: {AxB(x, y),9}, {AxB(width, width),9}");
                Print($" --> paste: {AxB(pasteX, pasteY),9}\n");
                image.Mutate(ctx => ctx.Resize(w, h).Crop(cropRect));
                collage.Mutate(ctx => ctx.DrawImage(image, pastePoint, opacity: 1F));

                pasteX += width + margin;
                if (pasteX >= collageWidth)
                {
                    pasteX = margin;
                    pasteY += width + margin;
                }

                imageNumber++;
            }

            var savePath = Path.Combine(options.OutputPath, $"FUGACO-{runTimestamp}-{collageNumber:00}.png");
            collage.Save(savePath);
            collageNumber++;
        }
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

    private static string AxB(int a, int b) => $"{a}x{b}";

    private static void Print     (string message) => Console.Write(message);
    private static void PrintError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Black;
        Console.BackgroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ResetColor();
    }
}