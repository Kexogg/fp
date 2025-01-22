using System.Text.RegularExpressions;
using TagsCloudContainerCore.Models;
using TagsCloudContainerCore.TextProcessor;

namespace TagsCloudContainerCLI.CLI;

public partial class CliHandler
{
    public static Result<None> ValidateOptions(CliOptions opts)
    {
        if (!Enum.TryParse<SortOrder>(opts.SortOrder, true, out _))
        {
            return Result.Fail<None>($"Invalid sort order: {opts.SortOrder}. Must be one of: {string.Join(", ", Enum.GetNames<SortOrder>())}");
        }

        foreach (var part in opts.ExcludedPartsOfSpeech.Split(","))
        {
            if (!Enum.TryParse<PartOfSpeech>(part, true, out _))
            {
                return Result.Fail<None>($"Invalid part of speech: {part}. Must be one of: {string.Join(", ", Enum.GetNames<PartOfSpeech>())}");
            }
        }
        
        if (opts.RenderScale <= 0)
        {
            return Result.Fail<None>($"Render scale must be positive, got: {opts.RenderScale}");
        }

        if (opts.MaxWords <= 0)
        {
            return Result.Fail<None>($"Max words must be positive, got: {opts.MaxWords}");
        }

        if (opts.MinFontSize <= 0 || opts.MaxFontSize <= 0 || opts.MinFontSize > opts.MaxFontSize)
        {
            return Result.Fail<None>($"Font sizes must be positive and min size must be less or equal to max size, got: {opts.MinFontSize}, {opts.MaxFontSize}");
        }

        if (opts.LayoutSpacing < 0)
        {
            return Result.Fail<None>($"Layout spacing must be non-negative, got: {opts.LayoutSpacing}");
        }

        return Result.Ok();
    }

}