import React, { Component } from 'react';

import { Routes, Route } from 'react-router-dom';

import Box from '@mui/material/Box';

import { createTheme, ThemeProvider } from '@mui/material/styles';

import Lobby from './components/LobbyComponent/Lobby';
import Home from './components/Home';
import Container from '@mui/material/Container';
export default class App extends Component {
  static displayName = App.name;

  render () {
    return (
   
        <Box sx={{ minHeight: '100vh', overflow: 'auto' }} >





            <Container>
                <Routes>
                    <Route path="/" element={<Home />} />
                    <Route path="/lobby">
                        <Route path=":lobbyUrl" element={<Lobby />} />
                        <Route path="*" element={<Home />} />
                    </Route>
                 

                </Routes>

            </Container>
        </Box>
  
     
    );
  }
}
