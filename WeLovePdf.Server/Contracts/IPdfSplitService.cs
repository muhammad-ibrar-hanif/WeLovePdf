namespace WeLovePdf.Server.Contracts
{
    public interface IPdfSplitService
    {
        Task<List<byte[]>> SplitAsync(byte[] pdfFile);
    }
}
