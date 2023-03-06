import React, { useState, useEffect } from 'react';
import { HubConnectionBuilder } from '@microsoft/signalr';
import { useParams } from "react-router-dom";


import Backdrop from '@mui/material/Backdrop';
import Grid from '@mui/material/Grid';
import { useNavigate } from "react-router-dom";

import CircularProgress from '@mui/material/CircularProgress';
import Button from '@mui/material/Button';

const Lobby = (props) => {
    const [connection, setConnection] = useState(null);
    const [users, setUsers] = useState([]);
    const [user, setUser] = useState('');

    const [lobbyState, setLobbyState] = useState(0);
    const navigate = useNavigate();
    let params = useParams();
    
    const [chosen, setChosen] = useState('');


    const checkLobbyExists = async () => {


        const response = await fetch("https://localhost:7178/api/lobbies/" + params.lobbyUrl, {
            method: 'GET',
            headers: new Headers({
                'Content-Type': 'application/json',
                'Accept': 'application/json'

            }),

        });
        const lobbyExistsResponse = await response;
        if (lobbyExistsResponse.ok == true) {
            const newConnection = new HubConnectionBuilder()
                .withUrl('https://localhost:7178/hubs/lobby/')
                .withAutomaticReconnect()
                .build();
            newConnection.serverTimeoutInMilliseconds = 120000;
            newConnection.keepAliveIntervalInMilliseconds = 60000;
            setConnection(newConnection);
        }
        else {
            errorReroute()
        }


    }

    useEffect(() => {
        checkLobbyExists()
    }, [])


    useEffect(() => {

        if (connection) {
            connection.start()
                .then(result => {

                    snedMessageToJoinLobby();
                    console.log('Connected!');

                    connection.on('JoinedLobby', message => {
                        //this message gets triggered from hub whenever someone else "joins" lobby
                        //this updates "THEM"
                        setUsers(message)
                        /* console.log(message)*/
                    });

                    connection.on('ReadyLobby', message => {

                        
                        setLobbyState(1);

                    });

                    connection.on('UserCreated', message => {
                        //this message gets triggered when the temp user is created in a lobby
                        //this updates "YOU"
                        setUser(message)

                    });
                    connection.on('ErrorConnection', message => {
                        errorReroute()


                    });
                    connection.on('ErrorAPI', message => {
                        alert("Error")


                    });
                })
                .catch(e => console.log('Connection failed: ', e));
        }

    }, [connection]);

    const startLobby = async () => {

        if (connection._connectionStarted) {
            const startMessage = {
                UrlString: params.lobbyUrl,
                UserId: user.id,
            };
            try {
                await connection.send('StartLobby', startMessage);
            }
            catch (e) {
                console.log(e);
            }
        }
        else {
            alert('No connection to server yet.');
        }
    }

    const errorReroute = () => {

        alert("Error, lobby may not exist or have started. Click ok to reroute back to main page.")
        navigate("/")
    }
    const snedMessageToJoinLobby = async () => {

        var joinLobbyMessage = {
            UrlString: params.lobbyUrl,

        };

        joinLobbyMessage.UserName = prompt("Please enter your name", "Harry");



        if (connection._connectionStarted) {
            try {
                await connection.send('JoinLobby', joinLobbyMessage);
            }
            catch (e) {
                console.log(e);
            }
        }
        else {
            alert('No connection to server yet.');
        }
    }

   
    if (lobbyState == 1) {
        return (<div> STARTED </div>)
    }

    if (users.length == 0)
        return (
            <Backdrop
                sx={{ color: '#fff', zIndex: (theme) => theme.zIndex.drawer + 1 }}
                open={true}

            >
                <CircularProgress color="inherit" />
            </Backdrop>)



    if (users.length != 0)
        return (<Grid container spacing={2}>
            {users.map((user) => (
                <Grid item xs={3}>
                    {user.name}

                </Grid>
                 

            ))}
            <Button sx={{ width: '400px' }} style={{ fontSize: 18 }} variant="contained" onClick={() => startLobby()}>Start Lobby</Button>

        </Grid>)



  

    return (
        <div> </div>
    )
};

export default Lobby;