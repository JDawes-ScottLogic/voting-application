﻿function VoteViewModel() {
    var self = this;

    self.options = ko.observableArray();
    self.sessionName = ko.observable();
    self.resultsUrl = ko.observable();

    self.chatMessages = ko.observableArray();
    self.socketClient = null;

    // Submit a vote
    self.doVote = function (data, event) {
        if (self.userId && self.sessionId) {
            $.ajax({
                type: 'PUT',
                url: '/api/user/' + self.userId + '/vote',
                contentType: 'application/json',
                data: JSON.stringify({
                    OptionId: data.Id,
                    SessionId: self.sessionId
                }),

                success: function (returnData) {
                    var currentRow = event.currentTarget.parentElement.parentElement;
                    $(currentRow).siblings().removeClass("success");
                    $(currentRow).addClass("success");

                    setTimeout(function () { window.location = "/Result?session=" + sessionId; }, 500);
                }
            });
        }
    }

    self.keyIsEnter = function (key, callback) {
        if (key && key.keyCode == 13) {
            callback();
        }
    }

    self.highlightPreviousVote = function () {
        $.ajax({
            type: 'GET',
            url: '/api/user/' + userId + '/session/' + sessionId + '/vote',
            contentType: 'application/json',

            success: function (data) {
                for (var index = 0; index < data.length; index++) {
                    var previousOptionId = data[index].OptionId;
                    var previousOption = self.options().filter(function (d) { return d.Id == previousOptionId }).pop();
                    var previousOptionRowIndex = self.options().indexOf(previousOption);
                    var matchingRow = $("#inner-vote-table > tbody > tr").eq(previousOptionRowIndex);
                    matchingRow.addClass("success");
                }
            }
        });
    }

    self.allOptions = function () {
        // Get all options
        $.ajax({
            type: 'GET',
            url: "/api/option",

            success: function (data) {
                self.options(data);
            }
        });
    }

    self.addOption = function () {
        //Don't submit without an entry in the name field
        if ($("#newName").val() == "") {
            return;
        }

        var newName = $("#newName").val();
        var newDescription = $("#newDescription").val()
        var newInfo = $("#newInfo").val()

        //Reset before posting, to prevent double posts.
        $("#newName").val("")
        $("#newDescription").val("")
        $("#newInfo").val("")

        $.ajax({
            type: 'POST',
            url: '/api/option',
            contentType: 'application/json',

            data: JSON.stringify({
                Name: newName,
                Description: newDescription,
                Info: newInfo
            }),

            success: function () {
                self.allOptions();
            }
        })
    }

    self.getSession = function (sessionId) {
        if (sessionId) {
            $.ajax({
                type: 'GET',
                url: "/api/session/" + sessionId,

                success: function (data) {
                    self.sessionName(data.Name);
                    $("#vote-table").show();
                }
            });
        }
    }

    self.sendChatMessage = function () {
        if (self.socketClient.sendMessage) {
            self.socketClient.sendMessage($('#chatTextInput').val());
        }
        $('#chatTextInput').val('');
    }

    $(document).ready(function () {
        self.sessionId = getSessionId();
        self.userId = localStorage["userId"];

        self.resultsUrl("/Result?session=" + self.sessionId);

        self.getSession(self.sessionId);
        self.allOptions();

        //Add option on pressing return key
        $("#newOptionRow").keypress(function (event) { self.keyIsEnter(event, self.addOption); });

        var onOpen = function () {
            self.chatMessages.unshift("Connected to chat");
        }

        var onMessage = function (message) {
            self.chatMessages.unshift(message);
        }

        var onClose = function () {
            self.chatMessages.unshift("Disconnected");
        }

        self.socketClient = new WebSocketClient(onOpen, onMessage, onClose);
        self.socketClient.connect();
    });
}

ko.applyBindings(new VoteViewModel());