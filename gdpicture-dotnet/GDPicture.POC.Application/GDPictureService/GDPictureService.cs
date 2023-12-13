using GDPicture.POC.Application.Contracts.Azure;
using GDPicture.POC.Application.Contracts.GDPicture;
using GdPicture14;

namespace GDPicture.POC.Application.GDPictureService
{
    public class GDPictureService : IGDPictureService
    {
        public Stream AttachDigitalSignature(Stream stream)
        {
            using (GdPicturePDF oGdPicturePDF = new GdPicturePDF())
            {
                GdPictureStatus status = oGdPicturePDF.LoadFromStream(stream, false);
                //Mandatory steps are the step #1 and the step #2 and the last step #5.
                //Step 1: Setting up the certicate, your digital ID file.
                status = oGdPicturePDF.SetSignatureCertificateFromP12("your_digital_ID.pfx", "test123");

                //Step 2: Setting up the signature information. At least one parameter must be set, others may stay empty.
                status = oGdPicturePDF.SetSignatureInfo("Orpalis", "Important PDF", "Toulouse (France)", "contact@orpalis.com");

                //Step 5: The last step - sign. This step must be the last one. All other optional steps may be done in any order.
                status = oGdPicturePDF.ApplySignature("output.pdf", PdfSignatureMode.PdfSignatureModeAdobePPKMS, true);

                return File.OpenRead("output.pdf");
            }
        }

        public Stream ProcessFileToBlob(Stream stream, string guid)
        {
            using (GdPictureDocumentConverter oConverter = new GdPictureDocumentConverter())
            {
                GdPictureStatus status = oConverter.LoadFromStream(stream, GdPicture14.DocumentFormat.DocumentFormatDOCX);

                if (status == GdPictureStatus.OK)
                {
                    status = oConverter.SaveAsPDF("output.pdf", PdfConformance.PDF_A_1a);
                    if (status == GdPictureStatus.OK)
                    {
                        return File.OpenRead("output.pdf");
                    }
                    else
                    {
                        throw new Exception("Cannot Convert to pdf");
                    }
                }
                else
                {
                    throw new Exception("Cannot Convert to pdf");
                }
            }
        }
    }
}
