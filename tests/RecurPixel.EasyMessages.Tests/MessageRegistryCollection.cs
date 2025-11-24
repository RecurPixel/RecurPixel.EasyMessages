using Xunit;

// Groups tests that interact with the global MessageRegistry so they run serially
[CollectionDefinition("MessageRegistry")]
public class MessageRegistryCollection { }
