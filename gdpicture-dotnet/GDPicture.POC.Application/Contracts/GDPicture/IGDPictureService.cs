namespace GDPicture.POC.Application.Contracts.GDPicture
{
    public interface IGDPictureService
    {
        Stream ProcessFileToBlob(Stream stream, string guid);   
        Stream AttachDigitalSignature(Stream stream);
    }
}
