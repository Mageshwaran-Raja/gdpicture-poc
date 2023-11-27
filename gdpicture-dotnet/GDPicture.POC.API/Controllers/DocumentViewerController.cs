using Microsoft.AspNetCore.Mvc;
using GdPicture14.WEB;
using GDPicture.POC.API.Models;

namespace GDPicture.POC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentViewerController : ControllerBase
    {        
        [HttpPost("GetDocuViewareControl")]
        public IActionResult GetDocuViewareControl([FromBody]DocuViewareConfiguration controlConfiguration)
        {
            if (!DocuViewareManager.IsSessionAlive(controlConfiguration.SessionId))
            {
                if (!string.IsNullOrEmpty(controlConfiguration.SessionId) && !string.IsNullOrEmpty(controlConfiguration.ControlId))
                {
                    DocuViewareManager.CreateDocuViewareSession(controlConfiguration.SessionId,
                        controlConfiguration.ControlId, 20);
                }
                else
                {
                    throw new Exception("Invalid session identifier and/or invalid control identifier.");
                }
            }
            using (DocuViewareControl docuVieware = new DocuViewareControl(controlConfiguration.SessionId))
            {
                docuVieware.AllowPrint = controlConfiguration.AllowPrint;
                docuVieware.EnablePrintButton = controlConfiguration.EnablePrintButton;
                docuVieware.AllowUpload = controlConfiguration.AllowUpload;
                docuVieware.EnableFileUploadButton = controlConfiguration.EnableFileUploadButton;
                docuVieware.CollapsedSnapIn = controlConfiguration.CollapsedSnapIn;
                docuVieware.ShowAnnotationsSnapIn = controlConfiguration.ShowAnnotationsSnapIn;
                docuVieware.EnableRotateButtons = controlConfiguration.EnableRotateButtons;
                docuVieware.EnableZoomButtons = controlConfiguration.EnableZoomButtons;
                docuVieware.EnablePageViewButtons = controlConfiguration.EnablePageViewButtons;
                docuVieware.EnableMultipleThumbnailSelection = controlConfiguration.EnableMultipleThumbnailSelection;
                docuVieware.EnableMouseModeButtons = controlConfiguration.EnableMouseModeButtons;
                docuVieware.EnableFormFieldsEdition = controlConfiguration.EnableFormFieldsEdition;
                docuVieware.EnableTwainAcquisitionButton = controlConfiguration.EnableTwainAcquisitionButton;
                docuVieware.MaxUploadSize = 36700160; // 35MB
                using (StringWriter controlOutput = new StringWriter())
                {
                    docuVieware.RenderControl(controlOutput);
                    DocuViewareRESTOutputResponse output = new DocuViewareRESTOutputResponse
                    {
                        HtmlContent = controlOutput.ToString()
                    };
                    return Ok(output);
                }
            }
        }
    }
}