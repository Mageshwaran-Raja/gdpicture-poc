import { createBrowserRouter } from "react-router-dom";
import App from "../layout/App";
import ViewDocument from "../../features/ViewDocument/ViewDocument";
import PDFConverter from "../../features/DocumentToPDF/PDFConverter";
import MergePDF from "../../features/MergePDF/MergePDF";

export const Router = createBrowserRouter([
    {
        path: '/',
        element: <App/>,
        children: [
            {path: 'docuVieware', element: <ViewDocument />},
            {path: 'documenttopdf', element: <PDFConverter/>},
            {path: 'mergepdf', element: <MergePDF/>},
        ]
    }
])