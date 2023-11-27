import { useEffect, useRef, useState } from "react"
import agent from "../../app/api/agent"
import { DocuViewareConfig } from "../../app/models/DocuViewareConfig"

export default function ViewDocument() {

    const [docuViewareConfig, setDocuViewareConfig] = useState<DocuViewareConfig>({
        SessionId: "",
        ControlId: "DocuVieware1",
        AllowPrint: true,
        EnablePrintButton: true,
        AllowUpload: true,
        EnableFileUploadButton: true,
        CollapsedSnapIn: true,
        ShowAnnotationsSnapIn: true,
        EnableRotateButtons: true,
        EnableZoomButtons: true,
        EnablePageViewButtons: true,
        EnableMultipleThumbnailSelection: true,
        EnableMouseModeButtons: true,
        EnableFormFieldsEdition: true,
        EnableTwainAcquisitionButton: true,
    });

    const targetRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        docuViewareConfig.SessionId = sessionStorage.getItem("SessionId")!;
        const response = agent.GDPicture.viewDocument(docuViewareConfig);
        response.then(res => {
            debugger;
            insertMarkup(res, "dvContainer");
        })
    }, []);

    const insertMarkup = (res: any, containerId: string) => {
        debugger;
        if (targetRef.current) {
            targetRef.current.innerHTML! = '';

            // Create a range and fragment
            const range = document.createRange();
            const fragment = range.createContextualFragment(res.htmlContent);

            targetRef.current.appendChild(fragment);
        }
    }


    return <div id="dvContainer" ref={targetRef} style={{ height: "100vh", width: "100%" }}></div>
}