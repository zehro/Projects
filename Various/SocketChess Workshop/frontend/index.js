var ChessBoard = require('chessboardjs')
var Chess = require('chess.js').Chess
var socket = require('socket.io-client')()

var board, game, status

var initGame = function() {
    var config = {
        draggable: true,
        position: 'start',
        onDrop: handleMove
    }

    board = ChessBoard('board', config)
    game = Chess()
}

var handleMove = function(source, target, piece) {
    if (piece[0] != color) {
        return 'snapback'
    }

    var move = game.move({from:source, to:target})
    if (move) {
        socket.emit('move', move)
    } else {
        console.log('illegal')
        return 'snapback'
    }
    socket.emit('message', source + '-->' + target)
}

socket.on('move', function(msg) {
    game.move(msg)
    board.position(game.fen(), false)
})

socket.on('gameStarted', function(msg) {
    color = msg
    initGame()
    status = document.getElementById('status')
    status.innerHTML = 'Color: ' + color
})

socket.on('disconnect', function() {
    board = ChessBoard('board', false)
    status.innerHTML = "The other player has disconnected. Please refresh to play again."
})
