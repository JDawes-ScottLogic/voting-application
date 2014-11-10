﻿function WebSocketClient(onOpen, onMessage, onClose) {

    var self = this;
    self.sendMessage = null;

    // Connect to the web socket
    this.connect = function () {
        var webSocket = new WebSocket("ws://voting-app-dev.azurewebsites.net:80");

        webSocket.onopen = function () {
            // Send a message to the server
            self.sendMessage = function (message) {
                webSocket.send(message);
            };

            if (onOpen) {
                onOpen();
            }
        };

        // Handle incoming messages
        webSocket.onmessage = function (message) {
            if (onMessage) {
                onMessage(message.data);
            }
        };

        // Handle disconnects
        webSocket.onclose = function () {
            if (onClose) {
                onClose();
            }
        };
    };
}