using GdPicture14;
using Microsoft.AspNetCore.Mvc;

namespace GDPicture.POC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GDPictureController : ControllerBase
    {
        public GDPictureController()
        {
            LicenseManager oLicenseManager = new LicenseManager();
            oLicenseManager.RegisterKEY("0402583831552455551491240");
        }
        [HttpPost("TIFFtoPDF")]
        public IActionResult TIFFtoPDF(IFormFile file)
        {
            GdPictureImaging oGdPictureImaging = new GdPictureImaging();
            int imageID = oGdPictureImaging.CreateGdPictureImageFromStream(file.OpenReadStream());
            if (oGdPictureImaging.GetStat() == GdPictureStatus.OK)
            {
                GdPictureOCR oGdPictureOCR = new GdPictureOCR();
                //Setting the OCR parameters.
                oGdPictureOCR.ResourceFolder = "D:\\GdPicture.NET 14\\Redist\\OCR";
                oGdPictureOCR.CharacterSet = "";
                //Setting up the language and the image.
                if ((oGdPictureOCR.AddLanguage(OCRLanguage.English) == GdPictureStatus.OK) &&
                    (oGdPictureOCR.SetImage(imageID) == GdPictureStatus.OK))
                {
                    //Running the OCR process.
                    string resID = oGdPictureOCR.RunOCR();
                    if (oGdPictureOCR.GetStat() == GdPictureStatus.OK)
                    {
                        //Getting the result as a text.
                        string content = oGdPictureOCR.GetOCRResultText(resID);
                        if (oGdPictureOCR.GetStat() == GdPictureStatus.OK)
                        {
                            //Creating a searchable PDF document.
                            using (GdPicturePDF oGdPicturePDF = new GdPicturePDF())
                            {
                                //Setting up your prefered page size and font parameters.
                                if ((oGdPicturePDF.CreateFromText(PdfConformance.PDF_A_1b, 595, 842, 10, 10, 10, 10,
                                                                    TextAlignment.TextAlignmentNear, content, 12, "Arial",
                                                                    false, false, true, false) == GdPictureStatus.OK) &&
                                    (oGdPicturePDF.SaveToFile("OCR3.pdf", true, true) == GdPictureStatus.OK))
                                {
                                    Console.WriteLine("Done");
                                }
                                else
                                {
                                    Console.WriteLine("Error");
                                }
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error");
                    }
                }
                oGdPictureImaging.ReleaseGdPictureImage(imageID);
                oGdPictureOCR.Dispose();
            }
            oGdPictureImaging.Dispose();
            return Ok();
        }
        [HttpPost("ConvertDocumentToPDF")]
        public IActionResult ConvertDocumentToPDF(IFormFile file)
        {
            //We assume that GdPicture has been correctly installed and unlocked.
            using (GdPictureDocumentConverter oConverter = new GdPictureDocumentConverter())
            //  using (GdPicturePDF oConverter = new GdPicturePDF())
            {
                //Select your source document and its document format.
                GdPictureStatus status = oConverter.LoadFromStream(file.OpenReadStream(), GdPicture14.DocumentFormat.DocumentFormatDOCX);

                //oConverter.LoadFromFile("input.docx", GdPicture14.DocumentFormat.DocumentFormatDOCX);
                if (status == GdPictureStatus.OK)
                {
                    //Select the conformance of the resulting PDF document.
                    status = oConverter.SaveAsPDF("out2.pdf", PdfConformance.PDF_A_1a);
                    if (status == GdPictureStatus.OK)
                    {
                        byte[] pdfBytes = System.IO.File.ReadAllBytes("out2.pdf");
                        System.IO.File.Delete("out2.pdf");
                        return File(pdfBytes, "application/pdf", "output.pdf");
                    }
                    else
                    {
                        Console.WriteLine("Error");
                    }
                }
                else
                {
                    Console.WriteLine("Error");
                }
            }
            return BadRequest("Failed to Convert to PDF");
        }

        [HttpPost("MergePDF")]
        public IActionResult MergePDF(List<IFormFile> files)
        {
            GdPicturePDF[] arPDF = new GdPicturePDF[files.Count];

            for (int i = 0; i < files.Count; i++)
            {
                arPDF[i] = new GdPicturePDF();
            }

            for (int i = 0; i < files.Count; i++)
            {
                if (arPDF[i].LoadFromStream(files[i].OpenReadStream(), false) == GdPictureStatus.OK)
                {

                }
            }
            GdPicturePDF gdPicturePDF = arPDF[0].MergeDocuments(arPDF);
            GdPictureStatus status = arPDF[0].GetStat();
            if (status == GdPictureStatus.OK)
            {
                //MessageBox.Show("All documents have been successfully merged.", "Example: MergeDocuments");
                if (gdPicturePDF.SaveToFile("test_merged.pdf") == GdPictureStatus.OK)
                {
                    //MessageBox.Show("Merged document has been successfully saved.", "Example: MergeDocuments");
                    gdPicturePDF.CloseDocument();
                }
            }
            else
            {
                //MessageBox.Show("The MergeDocuments() method has failed with the status: " + status.ToString(), "Example: MergeDocuments");
            }
            gdPicturePDF.Dispose();
            foreach (var pdf in arPDF)
            {
                pdf.Dispose();
            }
            byte[] pdfBytes = System.IO.File.ReadAllBytes("test_merged.pdf");
            System.IO.File.Delete("test_merged.pdf");
            return File(pdfBytes, "application/pdf", "test_merged.pdf");
        }

        [HttpPost("MergePDFFromFile")]
        public IActionResult MergePDFFromFile()
        {
            GdPicturePDF[] arPDF = new GdPicturePDF[3];
            arPDF[0] = new GdPicturePDF();
            arPDF[1] = new GdPicturePDF();
            arPDF[2] = new GdPicturePDF();
            if ((arPDF[0].LoadFromFile("out1.pdf", false) == GdPictureStatus.OK) &&
                (arPDF[1].LoadFromFile("out2.pdf", false) == GdPictureStatus.OK) &&
                (arPDF[2].LoadFromFile("out3.pdf", false) == GdPictureStatus.OK))
            {
                GdPicturePDF dstPDF = arPDF[0].MergeDocuments(arPDF);
                //You can use also arPDF[1] or arPDF[2] object to call the MergeDocuments method.
                GdPictureStatus status = arPDF[0].GetStat();
                if (status == GdPictureStatus.OK)
                {
                    //MessageBox.Show("All documents have been successfully merged.", "Example: MergeDocuments");
                    if (dstPDF.SaveToFile("test_merged.pdf") == GdPictureStatus.OK)
                    {
                        //MessageBox.Show("Merged document has been successfully saved.", "Example: MergeDocuments");
                        dstPDF.CloseDocument();
                    }
                }
                else
                {
                    //MessageBox.Show("The MergeDocuments() method has failed with the status: " + status.ToString(), "Example: MergeDocuments");
                }
                dstPDF.Dispose();
                arPDF[2].CloseDocument();
                arPDF[1].CloseDocument();
                arPDF[0].CloseDocument();
            }
            else
            {
                //MessageBox.Show("Loading of the source documents has failed.", "Example: MergeDocuments");
            }
            arPDF[2].Dispose();
            arPDF[1].Dispose();
            arPDF[0].Dispose();
            return Ok();
        }
    }
}
