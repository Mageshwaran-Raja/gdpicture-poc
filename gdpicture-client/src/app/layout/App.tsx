import { Outlet } from 'react-router-dom';
import '../../App.css';
import { useEffect } from 'react';
import agent from '../api/agent';

function App() {

  useEffect(() => {
    const response = agent.Session.getSession();
    response.then(res => {
      const sessionId = sessionStorage.getItem("SessionId");
      if (sessionId == null || sessionId == undefined) sessionStorage.setItem("SessionId", res);
    })
  }, []);

  return (
    <Outlet />
  );
}

export default App;
