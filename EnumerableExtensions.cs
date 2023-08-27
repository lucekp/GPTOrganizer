using CommandLine;

namespace GPTOrganizer;

public static class EnumerableExtensions
{
    public static bool IsHelp(this IEnumerable<Error> errs)
    {
        return errs.Any(err => err is HelpRequestedError);
    }

    public static bool IsVersion(this IEnumerable<Error> errs)
    {
        return errs.Any(err => err is VersionRequestedError);
    }
}