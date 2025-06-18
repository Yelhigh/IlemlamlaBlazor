namespace IlemlamlaBlazor.Models
{
    public enum CredentialSourceType
    {
        Configuration,
        EnvironmentVariable,
        AwsKeyVault,
        NotFound
    }

    public enum CredentialStatus
    {
        Found,
        NotFound
    }

    public static class AwsErrorCodes
    {
        public const string ResourceNotFoundException = "ResourceNotFoundException";
        public const string AccessDeniedException = "AccessDeniedException";
        public const string ProvisionedThroughputExceededException = "ProvisionedThroughputExceededException";
    }
} 