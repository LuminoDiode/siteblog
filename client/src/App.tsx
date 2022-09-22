import React from 'react';
import Footer from './UI/parts/footer/Footer';
import Header from './UI/parts/header/Header';
import Main from './UI/parts/main/Main';
import Modal from 'react-modal';
import "./App.css"


function App() {
  Modal.setAppElement('#root');

  return (
    <span className="App">
      <Header/>
      <Main/>
      <Footer/>
    </span>
  );
}

export default App;
