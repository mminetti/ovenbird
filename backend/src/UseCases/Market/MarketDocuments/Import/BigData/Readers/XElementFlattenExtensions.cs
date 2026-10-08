using System.Xml.Linq;

namespace UseCases.Market.MarketDocuments.Import.BigData.Readers;

public static class XElementFlattenExtensions
{
    public static IReadOnlyList<(string Name, string Value)> Flatten(this XElement element, string name)
    {
        var fields = new List<(string Name, string Value)>();

        var children = element.Elements().ToList();

        if (children.Count == 0)
        {
            fields.Add((name, element.Value));
        }
        else
        {
            AddChildren(children, name, fields);
        }

        return fields;
    }

    private static void AddChildren(List<XElement> children, string parentPath, List<(string Name, string Value)> fields)
    {
        foreach (var group in children.GroupBy(c => c.Name.LocalName))
        {
            var items = group.ToList();
            var indexed = items.Count > 1;

            for (var i = 0; i < items.Count; i++)
            {
                AddChild(items[i], group.Key, parentPath, indexed, i, fields);
            }
        }
    }

    private static void AddChild(
        XElement element,
        string tag,
        string parentPath,
        bool forceIndexed,
        int index,
        List<(string Name, string Value)> fields)
    {
        var children = element.Elements().ToList();

        if (children.Count == 0)
        {
            fields.Add((BuildPath(parentPath, tag, forceIndexed, index), element.Value));
            return;
        }

        var distinctChildTags = children.Select(c => c.Name.LocalName).Distinct().Count();

        if (distinctChildTags == 1)
        {
            // Pure wrapper (e.g. a *List element holding only one kind of item): drop its own
            // name from the path and force an index on its children, even if there's only one.
            for (var i = 0; i < children.Count; i++)
            {
                AddChild(children[i], children[i].Name.LocalName, parentPath, true, i, fields);
            }

            return;
        }

        var ownPath = BuildPath(parentPath, tag, forceIndexed, index);

        AddChildren(children, ownPath, fields);
    }

    private static string BuildPath(string parentPath, string tag, bool indexed, int index)
    {
        var segment = indexed ? $"{tag}[{index}]" : tag;

        return string.IsNullOrEmpty(parentPath) ? segment : $"{parentPath}.{segment}";
    }
}
