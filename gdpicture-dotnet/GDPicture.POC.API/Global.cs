using GDPicture.POC.API.DocuViewareCustomActions;
using GdPicture14.WEB;
using Microsoft.Net.Http.Headers;

namespace GDPicture.POC.API
{
    public class Global
    {
        public static string BuildDocuViewareControlSessionID(HttpContext HttpContext, string clientID)
        {
            if (HttpContext.Session.GetString("DocuViewareInit") == null)
            {
                HttpContext.Session.SetString("DocuViewareInit", "true");
            }

            return HttpContext.Session.Id + clientID;
        }


        public static DocuViewareLocale GetDocuViewareLocale(HttpRequest request)
        {
            if (request != null)
            {
                IList<StringWithQualityHeaderValue> acceptLanguage = request.GetTypedHeaders().AcceptLanguage;

                if (acceptLanguage != null)
                {
                    foreach (StringWithQualityHeaderValue language in acceptLanguage)
                    {
                        object docuviewareLocale;
                        if (Enum.TryParse(typeof(DocuViewareLocale), language.Value.Value, true, out docuviewareLocale))
                        {
                            return (DocuViewareLocale)docuviewareLocale;
                        }
                    }
                }
            }

            return DocuViewareLocale.En;
        }

        public static void CustomActionDispatcher(object sender, CustomActionEventArgs e)
        {
            switch (e.actionName)
            {
                case "automaticRemoveBlackBorders":
                case "autoDeskew":
                case "punchHoleRemoval":
                case "negative":
                case "despeckle":
                case "rotate-90":
                case "rotate+90":
                    ImageCleanupDemo.HandleImageCleanupAction(e);
                    break;

                case "barcodeRecognition":
                    ImageCleanupDemo.HandleImageCleanupAction(e);
                    break;

                case "Calculate":
                case "SaveAsPdf":
                    ImageCleanupDemo.HandleImageCleanupAction(e);
                    break;
                case "addTimestamp":
                case "addSignature":
                    ImageCleanupDemo.HandleImageCleanupAction(e);
                    break;
            }
        }
    }
}