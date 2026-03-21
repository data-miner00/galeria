namespace Provisioning;

internal class CosmosDbProvisionOptions
{
    public string DatabaseName { get; set; }

    public List<string> Containers { get; set; }
}
