import axios, { AxiosError, AxiosRequestConfig, AxiosResponse } from "axios";
import saveAs from "file-saver";


axios.defaults.baseURL = "https://localhost:7049/api/";

const responseBody = (response: AxiosResponse) => response.data;

axios.interceptors.response.use(response => {
    
    let contentType = response.headers["content-type"];
    if (contentType) {
        let blob = new File([response.data], "output.pdf", {
            type: "application/pdf"
        })
        saveAs(blob, "out.pdf")
    }

    return response;
}, (error: AxiosError) => {

    console.log(error);
    Promise.reject();

});

const request = {
    post: (url: string, body: any, responseType: AxiosRequestConfig = { responseType: 'arraybuffer' }) => 
            axios.post(url, body, responseType).then(responseBody)
}

const GDPicture = {
    convertToPDF: (file: any) => request.post("GDPicture/ConvertDocumentToPDF", file)
}

const agent = {
    GDPicture
}

export default agent;