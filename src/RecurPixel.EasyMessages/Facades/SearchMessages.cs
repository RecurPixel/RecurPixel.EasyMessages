using RecurPixel.EasyMessages.Core;

namespace RecurPixel.EasyMessages;

public static partial class Msg
{
    /// <summary>
    /// Search and filter messages.
    /// </summary>
    public static class Search
    {
        /// <summary>Returns SEARCH_001: No results found for '{query}'.</summary>
        public static Message NoResults(string? query = null)
        {
            var message = MessageRegistry.Get(MessageCodes.Search.NoResultsFound);

            return query != null ? message.WithParams(new { query }) : message;
        }

        /// <summary>Returns SEARCH_002: Found {count} result(s) for '{query}'.</summary>
        public static Message Completed(int? count = null, string? query = null)
        {
            var message = MessageRegistry.Get(MessageCodes.Search.SearchCompleted);

            return message.WithParamsIfProvided(new { count, query });
        }

        /// <summary>Returns SEARCH_003: Your search returned too many results.</summary>
        public static Message TooManyResults() =>
            MessageRegistry.Get(MessageCodes.Search.TooManyResults);

        /// <summary>Returns SEARCH_004: The search query contains invalid characters or syntax.</summary>
        public static Message InvalidQuery() =>
            MessageRegistry.Get(MessageCodes.Search.InvalidSearchQuery);
    }
}
