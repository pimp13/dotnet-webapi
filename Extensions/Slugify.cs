using System;
using System.Text.RegularExpressions;

namespace MyFirstApi.Extensions;

public static class Helper
{
    public static string Slugify(string input)
    {
        string slug = input.Trim().ToLowerInvariant();

        slug = Regex.Replace(slug, @"\s+", "-");

        slug = Regex.Replace(slug, @"[^a-z0-9\u0600-\u06FF\-]", "");

        slug = Regex.Replace(slug, @"-+", "-");

        slug = slug.Trim('-');

        return string.IsNullOrEmpty(slug) ? "slug" : slug;
    }
}
