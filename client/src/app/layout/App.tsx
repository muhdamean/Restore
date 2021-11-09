// import React from 'react';
// import logo from './logo.svg';

import { ThemeProvider } from "@emotion/react";
import { Container, createTheme, CssBaseline } from "@mui/material";
import { useState } from "react";

import Catalog from "../features/catalog/Catalog";

import Header from "./Header";

// import './App.css';


function App() {
  const [darkMode, setDarkMode]=useState(false);
  const paletteType=darkMode ? 'dark':'light';

  const theme=createTheme({
    palette:{
      mode: paletteType,
      background:{
        default: paletteType==='light' ? '#eaeaea' : '#121212'
      }
    }
  })

  function handleThmeChange(){
    setDarkMode(!darkMode)
  }

  return (
    <ThemeProvider theme={theme} >
      <CssBaseline></CssBaseline>
      <Header darkMode={darkMode} handleThemeChange={handleThmeChange} />
      <Container>
         <Catalog  />
      </Container>
     
    </ThemeProvider>
  );
}

export default App;
