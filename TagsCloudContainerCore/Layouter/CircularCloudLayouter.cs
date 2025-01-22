using TagsCloudContainerCore.FontManager;
using TagsCloudContainerCore.Models;
using TagsCloudContainerCore.Models.Graphics;

namespace TagsCloudContainerCore.Layouter;

public class CircularCloudLayouter : ILayouter
{
    private double _angle;
    private readonly List<Rectangle> _rectangles = new();
    private readonly CircularCloudLayouterConfig _config;
    private readonly IFontManager _fontManager;

    public IReadOnlyList<Rectangle> Rectangles => _rectangles.AsReadOnly();

    public CircularCloudLayouter(CircularCloudLayouterConfig config, IFontManager fontManager)
    {
        _fontManager = fontManager;
        _config = config;
    }

    public Result<Tag[]> LayoutTags(Dictionary<string, double> words)
    {
        if (words.Count == 0)
        {
            return Result.Fail<Tag[]>("Words dictionary is empty");
        }

        var minWeight = words.Values.Min();
        var maxWeight = words.Values.Max();

        var processed = words
            .Select(word => LayoutSingleTag(word, minWeight, maxWeight))
            .ToArray();

        var failedResult = processed.FirstOrDefault(r => !r.IsSuccess);
        if (failedResult.IsSuccess == false)
        {
            return Result.Fail<Tag[]>(failedResult.Error);
        }

        return processed.Select(r => r.Value).ToArray();
    }

    private Result<Tag> LayoutSingleTag(KeyValuePair<string, double> word, double minWeight, double maxWeight)
    {
        var adjustedFontSize = GetAdjustedFontSize(word.Value, minWeight, maxWeight);
        return PutNextTag(word, adjustedFontSize);
    }

    private float GetAdjustedFontSize(double wordValue, double minWeight, double maxWeight)
    {
        if (maxWeight == minWeight)
            return _config.MinFontSize;
        var adjustedFontSize = (float)(_config.MinFontSize + (_config.MaxFontSize - _config.MinFontSize) *
            (wordValue - minWeight) / (maxWeight - minWeight));
        return adjustedFontSize;
    }


    private Result<Tag> PutNextTag(KeyValuePair<string, double> word, float adjustedFontSize)
    {
        return _fontManager.MeasureString(word.Key, adjustedFontSize)
            .Then(rectangleSize =>
            {
                if (rectangleSize.Width <= 0 || rectangleSize.Height <= 0)
                    return Result.Fail<Size>("Invalid rectangle size");
                return Result.Ok(rectangleSize);
            })
            .Then(rectangleSize =>
            {
                Result<Rectangle> rectangle;
                do
                {
                    var result = GetNextPosition()
                        .Then(r => GetRectangleCenter(r, rectangleSize))
                        .Then(r => Result.Of(() => new Rectangle(
                            r.X,
                            r.Y,
                            r.X + rectangleSize.Width,
                            r.Y + rectangleSize.Height)));
                    if (!result.IsSuccess)
                    {
                        return result;
                    }

                    rectangle = result;
                } while (_rectangles.Any(r => r.IntersectsWith(rectangle.Value)));

                return rectangle;
            })
            .Then(r =>
            {
                _rectangles.Add(r);
                return Result.Ok(new Tag
                {
                    Text = word.Key,
                    FontSize = adjustedFontSize,
                    BBox = r
                });
            });
    }


    private static Result<Point> GetRectangleCenter(Point point, Size rectangleSize)
    {
        return Result.Of(() => new Point(point.X - rectangleSize.Width / 2,
            point.Y - rectangleSize.Height / 2));
    }

    private Result<Point> GetNextPosition()
    {
        var radius = _config.SpiralStep * _angle;
        var x = (float)(radius * Math.Cos(_angle));
        var y = (float)(radius * Math.Sin(_angle));

        _angle += _config.SpiralStep;

        return Result.Of(() => new Point(x, y));
    }
}