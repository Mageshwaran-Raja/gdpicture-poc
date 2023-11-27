import { createBrowserRouter } from "react-router-dom";
import App from "../layout/App";
import ViewDocument from "../../features/ViewDocument/ViewDocument";
import PDFConverter from "../../features/DocumentToPDF/PDFConverter";

export const Router = createBrowserRouter([
    {
        path: '/',
        element: <App/>,
        children: [
            {path: 'docuVieware', element: <ViewDocument />},
            {path: 'documenttopdf', element: <PDFConverter/>}
        ]
    }
])