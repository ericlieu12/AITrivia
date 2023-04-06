import React, { useState, useEffect } from 'react';
import { HubConnectionBuilder } from '@microsoft/signalr';
import { useParams } from "react-router-dom";

import Typography from '@mui/material/Typography';
import Backdrop from '@mui/material/Backdrop';
import Grid from '@mui/material/Grid';
import Stack from '@mui/material/Stack';
import { useNavigate } from "react-router-dom";
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemText from '@mui/material/ListItemText';
import ListItemAvatar from '@mui/material/ListItemAvatar';
import Avatar from '@mui/material/Avatar';
import ImageIcon from '@mui/icons-material/Image';
import CircularProgress from '@mui/material/CircularProgress';
import Button from '@mui/material/Button';
import Divider from '@mui/material/Divider';
import Box from '@mui/material/Box';
const Lobby = (props) => {
    const [connection, setConnection] = useState(null);
    const [users, setUsers] = useState([]);
    const [user, setUser] = useState('');

    const [lobbyState, setLobbyState] = useState(0);
    const navigate = useNavigate();
    let params = useParams();
    
    const [chosen, setChosen] = useState('');
    const [question, setQuestion] = useState({});
    const [correct, setCorrect] = useState(true);
    const [timerUp, setTimerUp] = useState(0);
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
        console.log("HITBEFORE")
        if (timerUp == 5) {
            
             sendAnswerResponseToLobby("AA");
            
        }
        
    }, [timerUp])

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

                    connection.on('CorrectOrIncorrect', message => {

                        setLobbyState(3);
                        setCorrect(message);
                    });
                   
                    connection.on('UserCreated', message => {
                        //this message gets triggered when the temp user is created in a lobby
                        //this updates "YOU"
                        console.log(message)
                        setUser(message)

                    });

                    connection.on('UpdateScores', message => {

                        setTimerUp(0)
                        setUsers(message)
                        setLobbyState(4);

                    });

                    connection.on('ErrorConnection', message => {
                        errorReroute()


                    });
                    connection.on('ReadyQuestion', message => {
                        setQuestion(message)
                        setLobbyState(2);


                    });
                    connection.on('DoneQuestion', message => {
                       
                        setTimerUp(5)
                        


                    });
                    connection.on('LobbyDone', message => {
                        
                        console.log("Done")


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
        setTimerUp(0);
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

    const sendAnswerResponseToLobby = async (answer) => {

        var answerLobbyMessage = {
            UrlString: params.lobbyUrl,

        };
        console.log(user);
        console.log(answer)
        answerLobbyMessage.UserId = user.id;
        answerLobbyMessage.answerString = answer;


        if (connection._connectionStarted) {
            try {
                console.log("if statement is true")
                await connection.send('recieveAnswer', answerLobbyMessage);
            }
            catch (e) {
                console.log(e);
            }
        }
        else {
            console.log("if statement is false")
            alert('No connection to server yet.');
        }
    }
    if (lobbyState == 1) {
        return (<div> STARTED </div>)
    }

    if (lobbyState == 2) {
      
        //QUESTIONS
        return (
            <Stack direction="column"
                justifyContent="space-between"
                alignItems="center"
                sx={{ height: '100vh', overflow: "hidden" }}>
                <Stack direction="column"
                    
                    alignItems="center"
                    sx={{ height: '50vh', width: '100%' }}>

                    <Typography mt={3} gutterBottom align="center" variant="h3" >
                        GPT Trivia
                    </Typography>
                    <Typography mt={3} align="center" variant="h3" >
                        {question.question}
                    </Typography>
                   
                </Stack>

                <Stack alignItems="center" sx={{ height: '50vh', width: '100%' }}>
                  
                        <Divider sx={{ width: '90%', bgcolor: "black" }} />
                    <Grid pt={2} sx={{ width: '90%', height: '100%' }} container spacing={2}>


                        {question.answers.map((answer) => (
                            <Grid item xs={6}>
                                <Button sx={{ width: '100%', height: '100%' }} style={{ fontSize: 18, backgroundColor: '#232528', borderRadius: 15 }} variant="contained" onClick={() => { sendAnswerResponseToLobby(answer) }} >
                                {answer}

                            </Button>
                            </Grid>

                        ))}
                          

                              
                          


                   


                    </Grid>
           
                </Stack>
                </Stack>
            )
    }
    if (lobbyState == 3) {
        if (user.isLeader) {
            return (<Stack direction="column"
                justifyContent="space-between"
                alignItems="center"
                sx={{ height: '100vh', overflow: "hidden" }}>
                <Stack direction="column"

                    alignItems="center"
                    sx={{ height: '50vh', width: '100%' }}>

                    <Typography mt={3} gutterBottom align="center" variant="h3" >
                        GPT Trivia
                    </Typography>
                    <Typography mt={3} align="center" variant="h3" >
                        {question.question}
                    </Typography>

                </Stack>

                <Stack alignItems="center" sx={{ height: '50vh', width: '100%' }}>

                    <Divider sx={{ width: '90%', bgcolor: "black" }} />
                    <Grid pt={2} sx={{ width: '90%', height: '100%' }} container spacing={2}>


                        {question.answers.map((answer) => {
                            if (question.correctAnswer == answer) {
                                return (<Grid item xs={6}>
                                    <Button sx={{ width: '100%', height: '100%' }} style={{ fontSize: 18, backgroundColor: '#009FFD', borderRadius: 15 }} variant="contained" onClick={() => { sendAnswerResponseToLobby(answer) }} >
                                        {answer}

                                    </Button>
                                </Grid>)
                            }
                            return (
                                <Grid item xs={6}>
                                    <Button sx={{ width: '100%', height: '100%' }} style={{ fontSize: 18, backgroundColor: '#232528', borderRadius: 15 }} variant="contained" onClick={() => { sendAnswerResponseToLobby(answer) }} >
                                        {answer}

                                    </Button>
                                </Grid>
                                )
                        }
                            

                        )}









                    </Grid>

                </Stack>
            </Stack>)
        }
        return (
            <Stack direction="column">
                <div> YOU GOT IT {correct.toString()} </div>

            </Stack>
        )
       
    }
    if (lobbyState == 4) {
        return (
           
            <Stack direction="column"
                justifyContent="center"
                alignItems="center"
                sx={{ height: '100vh', width: '100%'  }}>
                <Grid sx={{ width: '90%', height: '90%', bgcolor: '#232528', borderRadius: 10  }} container spacing={2}>


                    {users.map((user) => (
                        <Grid item xs={12}>
                            <Stack direction = "column" alignItems = "center">
                                <Stack direction="row"
                                    alignItems="center"
                                    justifyContent="space-between"
                                    sx={{width: '90%', height: '100%'}}
                            >
                                    <Stack pb={2} direction="row" alignItems="center">
                                <Avatar
                                    sx={{
                                        width: '50px',
                                        height: '50px',
                                                bgcolor: '#D295BF'
                                    }}
                                    alt="Remy Sharp"
                                    src="/broken-image.jpg"
                                >
                                    B
                                </Avatar>
                                        <Typography pl={1} sx={{ height: '100%', color: "#EFF2EF", fontSize: 15 }}>
                                    {user.name}
                                    </Typography>
                                </Stack>

                                    <Typography sx={{ height: '100%', color: "#EFF2EF", fontSize: 15 }}>
                                    {user.score}
                                </Typography>
                                </Stack>
                                <Divider sx={{ width: '90%', height: '2px', bgcolor: "#EFF2EF", borderRadius: 15 }} />
                                </Stack>
                        </Grid>
                       

                    ))}
                </Grid>
                </Stack>
                      

              
        )
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
        return (

            <Stack direction="column"
            justifyContent="space-between"
            alignItems="center"
            sx={{ height: '100vh', overflow: "hidden" }}>
            <Stack direction="column"
                justifyContent="space-between"
                alignItems="center" 
                    sx={{ height: '50vh', width: '100%' }}>
                  
                    <Typography mt={ 3} gutterBottom align="center" variant="h3" >
                GPT Trivia
                        </Typography>
                     
                <Stack direction="row"
                    justifyContent="space-around"
                    alignItems="center"
                    sx={{ width: '100vw' }}>
                    <Divider sx={{ width: '45%', bgcolor: "black" }} />
                        <Button sx={{ width: '100px', borderRadius: 28, height: '100px' }} style={{ fontSize: 18, backgroundColor: '#009FFD' }} variant="contained" onClick={() => startLobby()}>START</Button>
                    <Divider sx={{ width: '45%', bgcolor: "black" }} />
                </Stack>
            </Stack>
            <Stack alignItems="center" sx={{ height: '50vh', width: '100%' }}>
                
                    <Grid sx={{ width: '90%', height: '100%' }} container spacing={2}>
               

                    {users.map((user) => (
                        <Grid item xs={2}>

                            <Stack alignItems="center">
                            
                                <Avatar
                                    sx={{
                                        width: '100px',
                                        height: '100px',
                                        bgcolor: '#D295BF'
                                    }}
                                    alt="Remy Sharp"
                                    src="/broken-image.jpg"
                                >
                                    B
                                    </Avatar>
                                <Typography sx={{ fontSize: '25px' }} >
                                        {user.name}
                                        </Typography>
                                    </Stack>
                          
                           
                        </Grid>


                    ))}


                </Grid>
            </Stack>
        </Stack>)



  

    return (
        <div> </div>
    )
};

export default Lobby;