import { Outlet } from 'react-router-dom';
import '../../App.css';
import { useEffect } from 'react';
import agent from '../api/agent';
import Notification from '../../components/Notification';

function App() {

  useEffect(() => {
    const sessionId = sessionStorage.getItem("SessionId");
    if (sessionId == null || sessionId == undefined) {
      const response = agent.Session.getSession();
      response.then(res => {
        const sessionId = sessionStorage.getItem("SessionId");
        if (sessionId == null || sessionId == undefined) sessionStorage.setItem("SessionId", res);
      })
    }
  }, []);

  return (
    <>
      <Notification />
      <Outlet />
    </>
  );
}

export default App;
