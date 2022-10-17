import React from 'react';
import Footer from './UI/parts/footer/Footer';
import Header from './UI/parts/header/Header';
import Main from './UI/parts/main/Main';
import Modal from 'react-modal';
import "./App.css"
import { BrowserRouter } from 'react-router-dom';


function App() {
  Modal.setAppElement('#root');

  return (
    <span className="App">
      <BrowserRouter>
        <Header />
        <Main />
        <Footer />
      </BrowserRouter>
    </span>
  );
}

export default App;
