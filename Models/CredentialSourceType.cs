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
} 