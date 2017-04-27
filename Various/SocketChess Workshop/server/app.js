var express = require('express')
var app = express()
app.use(express.static('public'))
var http = require('http').Server(app)
var port = 3000

http.listen(port, function() {
    console.log("Listening on port: ", port)
})

var io = require('socket.io')(http)

games = {}
queue = []
id = 0
gameID = 0


io.on('connection', function(socket) {
    console.log('New Connection')

    assignId(socket)

    if (queue.length > 0) {
        startGame(socket, queue.pop())
        socket.emit('gameStarted', 'w')
        games[socket.gid].black.emit('gameStarted', 'b')
    } else {
        queue.push(socket)
    }

    socket.on('message', function(msg) {
        console.log('Got message from client: ' + msg)
    })

    socket.on('move', function(msg) {
        getOpponent(socket).emit('move', msg)
    })

    socket.on('disconnect', function() {
        opp = getOpponent(socket)
        opp.disconnect()
        games[socket.gid] = undefined
    })
})

function assignId(socket) {
    socket.userId = id
    id++
}

function startGame (s1, s2) {
    s1.gid = gameID
    s2.gid = gameID

    games[gameID] = {
        white: s1,
        black: s2
    }
    gameID++
}

function getOpponent(socket) {
    game = games[socket.gid]
    if (socket.userId === game.white.userId) {
        return game.black
    } else {
        return game.white
    }
}
