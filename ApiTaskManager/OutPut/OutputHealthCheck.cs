namespace ApiTaskManager.OutPut;

public class OutputHealthCheck
{
    public string Status { get; set; }
    public IEnumerable<OutputHealthCheckEntry> Checks { get; set; }
}

public class OutputHealthCheckEntry
{
    public string Key { get; set; }
    public string Status { get; set; }
    public string Description { get; set; }
    public IDictionary<string, object> Data { get; set; }
}
