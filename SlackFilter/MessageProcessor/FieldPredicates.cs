using System.Collections.Generic;
using System.Linq;
using SlackFilter.Model;
using SlackFilter.ServiceClients;

namespace SlackFilter.MessageProcessor
{
    internal static class FieldPredicates
    {
        public static bool ReviewersAreAllowed(MessageField field, IEnumerable<string> requesterList)
        {
            return field != null && (field.Title == "Reviewers" &&
                                     requesterList.Any(_ => field.Value.Contains(_)));
        }
    }
}