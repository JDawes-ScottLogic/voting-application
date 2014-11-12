﻿require(['jquery', 'knockout', 'Common'], function ($, ko, Common) {
    function HomeViewModel() {
        var self = this;

        self.sessions = ko.observableArray();
        self.optionSets = ko.observableArray();

        // Do login
        self.submitLogin = function () {
            $.ajax({
                type: 'PUT',
                url: '/api/user',
                contentType: 'application/json',
                data: JSON.stringify({
                    Name: $("#Name").val()
                }),

                success: function (data) {
                    //Expire in 6 hours
                    var expiryTime = Date.now() + (6 * 60 * 60 * 1000)
                    userId = data;
                    
                    localStorage["userId"] = JSON.stringify({ id: userId, expires: expiryTime });
                    $('#loginForm').addClass("has-success");
                    window.location.replace("vote?session=" + self.sessionId);
                },

                error: function (jqXHR, textStatus, errorThrown) {
                    if (jqXHR.status == 400) {
                        $('#loginForm').addClass("has-error");
                        $('#usernameWarnMessage').show();
                    }
                }
            });
        }

        self.submitSession = function () {
            self.sessionId = $("#session-select").val();
            window.location = "?session=" + self.sessionId;
        }

        self.createSession = function () {
            $.ajax({
                type: 'POST',
                url: '/api/session',
                contentType: 'application/json',

                data: JSON.stringify({
                    Name: $("#session-create").val(),
                    OptionSetId: $("#optionset-select").val()
                }),

                success: function (data) {
                    self.sessionId = data;
                    window.location = "?session=" + self.sessionId;
                }
            })
        }

        self.allSessions = function () {
            $.ajax({
                type: 'GET',
                url: '/api/session',

                success: function (data) {
                    self.sessions(data);
                }
            })
        }

        self.allOptionSets = function () {
            $.ajax({
                type: 'GET',
                url: '/api/optionset',

                success: function (data) {
                    self.optionSets(data);
                }
            })
        }

        $(document).ready(function () {
            self.sessionId = Common.getSessionId();

            self.userId = Common.currentUserId();

            if (!self.sessionId) {
                self.allSessions();
                self.allOptionSets();
                $("#login-box").hide();
                $("#sessions").show();
            }
            else if (!self.userId) {
                $("#sessions").hide();
                $("#login-box").show();
            } else {
                window.location.replace("vote?session=" + self.sessionId);
            }
        });
    }

    ko.applyBindings(new HomeViewModel());
});