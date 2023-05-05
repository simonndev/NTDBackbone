namespace NtdApi.Application
{
    public abstract class NtdResultBase<TPayload>
    {
        public bool Success { get; set; }
        public TPayload? Payload { get; set; }
        public int HttpStatusCode { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}
