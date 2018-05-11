using System.Collections.Generic;
using System.Linq;
using SlackFilter.Model;

namespace SlackFilter.MessageProcessor
{
    internal static class FieldPredicates
    {
        public static bool BuildIsAllowed(MessageField field, string[] buildList)
        {
            return field != null && (field.Title == "Build Definition" && buildList.Contains(field.Value));
        }

        public static bool RequesterIsAllowed(MessageField field, string[] requesterList)
        {
            return field != null && (field.Title == "Requested by" && requesterList.Contains(field.Value));
        }

        public static bool ReviewersAreAllowed(MessageField field, IEnumerable<string> requesterList)
        {
            return field != null && (field.Title == "Reviewers" &&
                                     requesterList.Any(_ => field.Value.Contains(_)));
        }
    }
}