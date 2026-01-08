namespace WeLovePdf.Server.Contracts
{
    public interface IPdfMergeService
    {
        Task<byte[]> MergeAsync(List<byte[]> pdfFiles);
    }
}
