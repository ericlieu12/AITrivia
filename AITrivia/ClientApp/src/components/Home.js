import React, { useState } from 'react';

import { Link } from "react-router-dom";


import Backdrop from '@mui/material/Backdrop';

import Button from '@mui/material/Button';
import Stack from '@mui/material/Stack';
import { useNavigate } from "react-router-dom";


import Typography from '@mui/material/Typography';

import CircularProgress from '@mui/material/CircularProgress';

const Home = (props) => {
    const [loading, setLoading] = useState(false)
    const navigate = useNavigate();
    async function createLobby() {
        setLoading(true)
        try {

            const response = await fetch("https://localhost:7178/api/lobbies/createlobby", {
                method: 'POST',
                headers: new Headers({
                    'Content-Type': 'application/json',
                    'Accept': 'application/json'

                }),

            });
            const lobby = await response.json();


            navigate("/lobby/" + lobby.urlString);
        }
        catch (ex) {
            alert("Failed to create lobbby. Wait a few seconds and try again or refresh the page.")
        }

    }
    if (loading) {
        return (
            <Backdrop
                sx={{ color: '#fff', zIndex: (theme) => theme.zIndex.drawer + 1 }}
                open={true}

            >
                <CircularProgress color="inherit" />
            </Backdrop>)
    }
    return (
        <Stack direction="column" spacing={4} justifyContent="center"
            alignItems="center">
            <Stack>
                <Typography gutterBottom align="center" variant="h3" >
                    GPT Trivia
                </Typography>
                <Typography gutterBottom align="center" variant="h4" >
                    AI Generated trivia questions
                </Typography>
            </Stack>
            <Button sx={{ width: '400px' }} style={{ fontSize: 18 }} variant="contained" onClick={() => createLobby()}>Start Lobby</Button>
            
            <Stack>
                <Typography align="center" variant="body1" >
                    Made by <Link to="https://www.linkedin.com/in/ericlieu12"> Eric Lieu </Link>
                </Typography>
               
            </Stack>
            <Stack>
                <Typography align="center" variant="body2" >
                    Any suggestions or bugs, email me at ericlieu118@gmail.com. Version 1.0, created with React, .NET Core, and SignalR.
                </Typography>
                
            </Stack>

        </Stack>
    )
};

export default Home;