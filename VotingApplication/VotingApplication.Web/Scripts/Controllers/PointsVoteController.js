﻿(function () {
    var VotingApp = angular.module('VotingApp');

    VotingApp.controller('PointsVoteController', function ($scope, PollAction) {

        PollAction.getPoll(PollAction.currentPollId(), function (data) {
            $scope.options = data.Options;
            $scope.totalPointsAvailable = data.MaxPoints;
            $scope.maxPointsPerOption = data.MaxPerVote;
        });

        $scope.unallocatedPoints = function () {
            var unallocatedPoints = $scope.totalPointsAvailable;

            for (var i = 0; i < $scope.options.length; i++) {
                unallocatedPoints -= $scope.options[i].pollValue || 0;
            }

            return unallocatedPoints;
        }

        $scope.disabledAddPoints = function(pointValue) {
            return pointValue >= $scope.maxPointsPerOption || $scope.unallocatedPoints() === 0;
        }
    });
})();