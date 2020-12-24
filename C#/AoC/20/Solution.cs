using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AoC._20
{
    public sealed class Solution : ISolution
    {
        public async Task Run()
        {
            var part1 = await Part1();
            Console.WriteLine($"Part 1: {part1}");

            var part2 = await Part2();
            Console.WriteLine($"Part 2: {part2}");
        }

        private async Task<long> Part1()
        {
            var images = await ParseInput();

            var placed = PlaceImages(images);

            var minX = placed.Values.Min(v => v.X);
            var maxX = placed.Values.Max(v => v.X);
            var minY = placed.Values.Min(v => v.Y);
            var maxY = placed.Values.Max(v => v.Y);

            //var stringBuilder = new StringBuilder();
            //for (var j = minY; j <= maxY; j++)
            //{
            //    for (var j2 = 0; j2 < 10; j2++)
            //    {
            //        for (var i = minX; i <= maxX; i++)
            //        {
            //            var image = placed.Single(kvp => kvp.Value == (i, j)).Key;
            //            for (var i2 = 0; i2 < 10; i2++)
            //            {
            //                var pixel = image[(i2, j2)];
            //                stringBuilder.Append(pixel == Pixel.Black ? '#' : '.');
            //            }

            //            stringBuilder.Append(" ");
            //        }

            //        stringBuilder.AppendLine();
            //    }

            //    stringBuilder.AppendLine();
            //}
            //Console.WriteLine(stringBuilder);

            var topLeft = placed.Single(kvp => kvp.Value == (minX, minY)).Key;
            var topRight = placed.Single(kvp => kvp.Value == (maxX, minY)).Key;
            var bottomRight = placed.Single(kvp => kvp.Value == (maxX, maxY)).Key;
            var bottomLeft = placed.Single(kvp => kvp.Value == (minX, maxY)).Key;

            return topLeft.Id * topRight.Id * bottomRight.Id * bottomLeft.Id;
        }

        private async Task<long> Part2()
        {
            var images = await ParseInput();
            var placed = PlaceImages(images);

            var fullImage = new HashSet<(int X, int Y)>();
            
            foreach (var image in placed.Keys)
            {
                var position = placed[image];
                var xOffset = position.X * 8;
                var yOffset = position.Y * 8;
                foreach (var pixel in image.Pixels)
                {
                    if (!new[] {0, 9}.Intersect(new[] {pixel.Key.X, pixel.Key.Y}).Any())
                    {
                        if (pixel.Value == Pixel.Black)
                        {
                            fullImage.Add((xOffset + pixel.Key.X, yOffset + pixel.Key.Y));
                        }
                    }
                }
            }

            var foundSeaMonster = false;
            for (var i = 0; i < 4; i++)
            {
                foreach (var coord in fullImage)
                {
                    var seaMonsterCoords = GetSeaMonsterCoords(coord);
                    if (seaMonsterCoords.All(c => fullImage.Contains(c)))
                    {
                        foundSeaMonster = true;
                        break;
                    }

                    fullImage = new HashSet<(int X, int Y)>(fullImage.Select(c => (c.Y, -c.X)));
                }
            }

            if (!foundSeaMonster)
            {
                fullImage = new HashSet<(int X, int Y)>(fullImage.Select(c => (c.X, -c.Y)));
                foreach (var coord in fullImage)
                {
                    var seaMonsterCoords = GetSeaMonsterCoords(coord);
                    if (seaMonsterCoords.All(c => fullImage.Contains(c)))
                    {
                        foundSeaMonster = true;
                        break;
                    }

                    fullImage = new HashSet<(int X, int Y)>(fullImage.Select(c => (c.Y, -c.X)));
                }
            }

            if (!foundSeaMonster)
            {
                throw new Exception("No orientation contains a sea monster");
            }

            var minX = fullImage.Min(v => v.X);
            var maxX = fullImage.Max(v => v.X);
            var minY = fullImage.Min(v => v.Y);
            var maxY = fullImage.Max(v => v.Y);

            var stringBuilder = new StringBuilder();
            for (var j = minY; j <= maxY; j++)
            {
                for (var i = minX; i <= maxX; i++)
                {
                    stringBuilder.Append(fullImage.Contains((i, j)) ? '#' : '.');
                }

                stringBuilder.AppendLine();
            }
            Console.WriteLine(stringBuilder);

            var inSeaMonster = new HashSet<(int X, int Y)>();

            foreach (var coord in fullImage)
            {
                var seaMonsterCoords = GetSeaMonsterCoords(coord);
                if (seaMonsterCoords.All(c => fullImage.Contains(c)))
                {
                    seaMonsterCoords.ForEach(c => inSeaMonster.Add(c));
                }
            }

            return fullImage.Except(inSeaMonster).Count();
        }
        
        private List<(int X, int Y)> GetSeaMonsterCoords((int X, int Y) c)
        {
            return new List<(int X, int Y)>
            {
                (c.X, c.Y),
                (c.X + 1, c.Y + 1),
                (c.X + 4, c.Y + 1),
                (c.X + 5, c.Y),
                (c.X + 6, c.Y),
                (c.X + 7, c.Y + 1),
                (c.X + 10, c.Y + 1),
                (c.X + 11, c.Y),
                (c.X + 12, c.Y),
                (c.X + 13, c.Y + 1),
                (c.X + 16, c.Y + 1),
                (c.X + 17, c.Y),
                (c.X + 18, c.Y),
                (c.X + 18, c.Y - 1),
                (c.X + 19, c.Y),
            };
        }

        private Dictionary<Image, (int X, int Y)> PlaceImages(List<Image> images)
        {
            var dimension = (int)Math.Sqrt(images.Count);

            var processed = new Dictionary<Image, (int X, int Y)> { { images[0], (0, 0) } };
            var toProcess = images.Skip(1).ToList();
            while (toProcess.Any())
            {
                var batch = toProcess.ToList();
                foreach (var image in batch)
                {
                    var placed = false;
                    foreach (var placedImage in processed.Keys)
                    {
                        var placedCoord = processed[placedImage];

                        if (!processed.Values.Contains((placedCoord.X, placedCoord.Y - 1)) && processed.Values.Max(v => v.Y) - placedCoord.Y + 1 < dimension)
                        {
                            var placedTopEdge = placedImage.TopEdge;

                            for (var i = 0; i < 4; i++)
                            {
                                if (image.BottomEdge.Equals(placedTopEdge))
                                {
                                    processed[image] = (placedCoord.X, placedCoord.Y - 1);
                                    placed = true;
                                    break;
                                }

                                image.RotateClockwise();
                            }

                            if (placed)
                            {
                                break;
                            }

                            image.FlipHorizontally();

                            for (var i = 0; i < 4; i++)
                            {
                                if (image.BottomEdge.Equals(placedTopEdge))
                                {
                                    processed[image] = (placedCoord.X, placedCoord.Y - 1);
                                    placed = true;
                                    break;
                                }

                                image.RotateClockwise();
                            }

                            if (placed)
                            {
                                break;
                            }
                        }

                        if (!processed.Values.Contains((placedCoord.X + 1, placedCoord.Y)) && placedCoord.X + 1 - processed.Values.Min(v => v.X) < dimension)
                        {
                            var placedRightEdge = placedImage.RightEdge;

                            for (var i = 0; i < 4; i++)
                            {
                                if (image.LeftEdge.Equals(placedRightEdge))
                                {
                                    processed[image] = (placedCoord.X + 1, placedCoord.Y);
                                    placed = true;
                                    break;
                                }

                                image.RotateClockwise();
                            }

                            if (placed)
                            {
                                break;
                            }

                            image.FlipHorizontally();

                            for (var i = 0; i < 4; i++)
                            {
                                if (image.LeftEdge.Equals(placedRightEdge))
                                {
                                    processed[image] = (placedCoord.X + 1, placedCoord.Y);
                                    placed = true;
                                    break;
                                }

                                image.RotateClockwise();
                            }

                            if (placed)
                            {
                                break;
                            }
                        }

                        if (!processed.Values.Contains((placedCoord.X, placedCoord.Y + 1)) && placedCoord.Y + 1 - processed.Values.Min(v => v.Y) < dimension)
                        {
                            var placedBottomEdge = placedImage.BottomEdge;

                            for (var i = 0; i < 4; i++)
                            {
                                if (image.TopEdge.Equals(placedBottomEdge))
                                {
                                    processed[image] = (placedCoord.X, placedCoord.Y + 1);
                                    placed = true;
                                    break;
                                }

                                image.RotateClockwise();
                            }

                            if (placed)
                            {
                                break;
                            }

                            image.FlipHorizontally();

                            for (var i = 0; i < 4; i++)
                            {
                                if (image.TopEdge.Equals(placedBottomEdge))
                                {
                                    processed[image] = (placedCoord.X, placedCoord.Y + 1);
                                    placed = true;
                                    break;
                                }

                                image.RotateClockwise();
                            }

                            if (placed)
                            {
                                break;
                            }
                        }

                        if (!processed.Values.Contains((placedCoord.X - 1, placedCoord.Y)) && processed.Values.Max(v => v.X) - placedCoord.X + 1 < dimension)
                        {
                            var placedLeftEdge = placedImage.LeftEdge;

                            for (var i = 0; i < 4; i++)
                            {
                                if (image.RightEdge.Equals(placedLeftEdge))
                                {
                                    processed[image] = (placedCoord.X - 1, placedCoord.Y);
                                    placed = true;
                                    break;
                                }

                                image.RotateClockwise();
                            }

                            if (placed)
                            {
                                break;
                            }

                            image.FlipHorizontally();

                            for (var i = 0; i < 4; i++)
                            {
                                if (image.RightEdge.Equals(placedLeftEdge))
                                {
                                    processed[image] = (placedCoord.X - 1, placedCoord.Y);
                                    placed = true;
                                    break;
                                }

                                image.RotateClockwise();
                            }

                            if (placed)
                            {
                                break;
                            }
                        }
                    }

                    if (placed)
                    {
                        toProcess.Remove(image);
                    }
                }
            }

            return processed;
        }

        public static async Task<List<Image>> ParseInput()
        {
            var text = await File.ReadAllTextAsync("20/input");
            var lines = text.Split("\r\n");
            var lineCounter = 0;

            var images = new List<Image>();

            while (lineCounter < lines.Length)
            {
                var idLine = lines[lineCounter];
                var idRegex = new Regex("Tile (?<id>\\d+):");
                var id = Convert.ToInt32(idRegex.Match(idLine).Groups["id"].Value);
                lineCounter++;

                var pixels = new Dictionary<(int X, int Y), Pixel>();
                for (var j = 0; j < 10; j++)
                {
                    var line = lines[lineCounter];
                    for (var i = 0; i < 10; i++)
                    {
                        pixels[(i, j)] = line[i] == '#' ? Pixel.Black : Pixel.White;
                    }

                    lineCounter++;
                }

                images.Add(new Image(id, pixels));
                lineCounter++;
            }

            return images;
        }
    }

    public sealed class Image
    {
        public long Id { get; }

        private IReadOnlyDictionary<(int X, int Y), Pixel> _pixels;

        public IReadOnlyDictionary<(int X, int Y), Pixel> Pixels => _pixels;

        public Image(long id, IReadOnlyDictionary<(int X, int Y), Pixel> pixels)
        {
            Id = id;
            _pixels = pixels;
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            for (var j = 0; j < 10; j++)
            {
                var lineBuilder = new StringBuilder();
                for (var i = 0; i < 10; i++)
                {
                    var pixel = this[(i, j)];
;                   lineBuilder.Append(pixel == Pixel.White ? '.' : '#');
                }

                stringBuilder.AppendLine(lineBuilder.ToString());
            }

            return stringBuilder.ToString();
        }

        public void RotateClockwise()
        {
            _pixels = _pixels.ToDictionary(
                    kvp => (9 - kvp.Key.Y, kvp.Key.X),
                    kvp => kvp.Value);
        }

        public void RotateAntiClockwise()
        {
            _pixels = _pixels.ToDictionary(
                    kvp => (kvp.Key.Y, 9 - kvp.Key.X), 
                    kvp => kvp.Value);
        }

        public void FlipHorizontally()
        {
            _pixels = _pixels.ToDictionary(
                    kvp => (9 - kvp.Key.X, kvp.Key.Y),
                    kvp => kvp.Value);
        }

        public void FlipVertically()
        {
            _pixels = _pixels.ToDictionary(
                    kvp => (kvp.Key.X, 9 - kvp.Key.Y),
                    kvp => kvp.Value);
        }

        public Pixel this[(int X, int Y) c] => _pixels.TryGetValue(c, out var p) ? p : Pixel.White;

        public ImageEdge TopEdge => new ImageEdge(Enumerable.Range(0, 10).Select(i => this[(i, 0)]).ToList());
        public ImageEdge BottomEdge => new ImageEdge(Enumerable.Range(0, 10).Select(i => this[(i, 9)]).ToList());
        public ImageEdge LeftEdge => new ImageEdge(Enumerable.Range(0, 10).Select(i => this[(0, i)]).ToList());
        public ImageEdge RightEdge => new ImageEdge(Enumerable.Range(0, 10).Select(i => this[(9, i)]).ToList());
    }

    public struct ImageEdge : IEquatable<ImageEdge>
    {
        private readonly IReadOnlyList<Pixel> _pixels;

        public ImageEdge(IReadOnlyList<Pixel> pixels)
        {
            _pixels = pixels;
        }

        public bool Equals(ImageEdge other)
        {
            return _pixels.SequenceEqual(other._pixels);
        }

        public override bool Equals(object obj)
        {
            return obj is ImageEdge other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (_pixels != null ? _pixels.GetHashCode() : 0);
        }
    }

    public enum Pixel
    {
        Black,
        White
    }
}