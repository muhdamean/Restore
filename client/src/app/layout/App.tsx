// import React from 'react';
// import logo from './logo.svg';

import { ThemeProvider } from "@emotion/react";
import { Container, createTheme, CssBaseline } from "@mui/material";
import { useCallback, useEffect, useState } from "react";
import { Route } from "react-router";
import { ToastContainer } from "react-toastify";
import AboutPage from "../../features/about/AboutPage";

import Catalog from "../../features/catalog/Catalog";
import ProductDetails from "../../features/catalog/ProductDetails";
import ContactPage from "../../features/contact/ContactPage";
import HomePage from "../../features/home/Homepage";

import Header from "./Header";
import 'react-toastify/dist/ReactToastify.css';
import ServerError from "../errors/ServerError";
import NotFound from "../errors/NotFound";
import { Switch } from "react-router-dom";
import BasketPage from "../../features/basket/BasketPage";
import LoadingComponent from "./LoadingComponent";
import CheckoutPage from "../../features/checkout/CheckoutPage";
import { useAppDispatch } from "../store/configureStore";
import { fetchBasketAsync, setBasket } from "../../features/basket/basketSlice";
import Login from "../../features/account/Login";
import Register from "../../features/account/Register";
import { fetchCurrentUser } from "../../features/account/accountSlice";
import agent from "../api/agent";
import { getCookie } from "../util/util";

// import './App.css';


function App() {
  const dispatch=useAppDispatch();
  //const {setBasket}=useStoreContext();
  const [loading, setLoading]=useState(true);

const initApp=useCallback(async()=>{
  try{
      await dispatch(fetchCurrentUser());
      await dispatch(fetchBasketAsync());
  }catch(error){
    console.log(error);
  }
},[dispatch])

  useEffect(()=>{
    //initApp().then(()=>setLoading(false));
    // const buyerId=getCookie('buyerId');
    // dispatch(fetchCurrentUser());
    // if(buyerId){
    //   agent.Basket.get()
    //       .then(basket=>dispatch(setBasket(basket)))
    //       .catch(error=>console.log(error))
    //       .finally(()=>setLoading(false));
    // }else{
    //   setLoading(false)
    // }
  }, [initApp])

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

  if(loading) return <LoadingComponent message='initialising app...' />
  return (
    <ThemeProvider theme={theme} >
      <ToastContainer position='bottom-right' hideProgressBar />
      <CssBaseline></CssBaseline>
      <Header darkMode={darkMode} handleThemeChange={handleThmeChange} />
      <Container>
          <Switch>
            <Route exact path='/' component={HomePage}/>
            <Route exact path='/catalog' component={Catalog}/>
            <Route  path='/catalog/:id' component={ProductDetails}/>
            <Route  path='/about' component={AboutPage}/>
            <Route  path='/contact' component={ContactPage}/>
            <Route  path='/server-error' component={ServerError}/>
            <Route  path='/basket' component={BasketPage}/>
            <Route  path='/checkout' component={CheckoutPage}/>
            <Route  path='/login' component={Login}/>
            <Route  path='/register' component={Register}/>
            <Route  component={NotFound}/>
          </Switch>
      </Container>
     
    </ThemeProvider>
  );
}

export default App;
