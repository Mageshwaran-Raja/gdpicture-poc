import axios, { AxiosError, AxiosRequestConfig, AxiosResponse } from "axios";
import saveAs from "file-saver";
import { DocuViewareConfig } from "../models/DocuViewareConfig";


axios.defaults.baseURL = "https://localhost:7049/api/";

const responseBody = (response: AxiosResponse) => response.data;

axios.interceptors.response.use(response => {
    debugger;
    let contentType = response.headers["content-type"];
    if (contentType == 'application/pdf') {
        let blob = new File([response.data], "output.pdf", {
            type: "application/pdf"
        })
        saveAs(blob, "output.pdf")
    }

    return response;
}, (error: AxiosError) => {

    console.log(error);
    Promise.reject();

});

const request = {
    get: (url: string) => axios.get(url).then(responseBody),
    post: (url: string, body: {}) => axios.post(url, body).then(responseBody),
    postFile: (url: string, body: any, responseType: AxiosRequestConfig = { responseType: 'arraybuffer' }) => 
            axios.post(url, body, responseType).then(responseBody)
}

const GDPicture = {
    convertToPDF: (file: any) => request.postFile("GDPicture/ConvertDocumentToPDF", file),
    viewDocument: (config: DocuViewareConfig) => request.post("DocumentViewer/GetDocuViewareControl", config)
}

const Session = {
    getSession: () => request.get("Session/create")
}

const agent = {
    GDPicture,
    Session
}

export default agent;