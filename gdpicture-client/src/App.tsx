import { useState } from 'react';
import './App.css';
import agent from './app/api/agent';

function App() {

  const [fileSelected, setFileSelected] = useState();

  function convertToPDFHandler (event: any) {
    setFileSelected(event.target.files[0]);
  } 

  function onClickHandler() {
    const data = new FormData();
    data.append("file", fileSelected!);

    agent.GDPicture.convertToPDF(data);

    console.log(fileSelected);
  }

  return (
    <div className="App">
      <input type='file' onChange={convertToPDFHandler} />
      <button onClick={onClickHandler}>Convert</button>
    </div>
  );
}

export default App;
