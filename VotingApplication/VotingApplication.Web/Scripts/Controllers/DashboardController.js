﻿/// <reference path="../Services/AccountService.js" />
/// <reference path="../Services/PollService.js" />
(function () {
    angular
        .module('GVA.Creation')
        .controller('DashboardController', DashboardController);

    DashboardController.$inject = ['$scope', 'AccountService', 'PollService', 'RoutingService'];

    function DashboardController($scope, AccountService, PollService, RoutingService) {

        $scope.account = AccountService.account;
        $scope.createPoll = createNewPoll;
        $scope.getUserPolls = getUserPolls;
        $scope.navigateToManagePage = navigateToManagePage;
        $scope.copyPoll = copyPoll;

        $scope.userPolls = {};

        activate();


        function activate() {
            AccountService.registerAccountObserver(function () {
                $scope.account = AccountService.account;
            });

            getUserPolls();
        }


        function createNewPoll(question) {
            PollService.createPoll(question, createPollSuccessCallback);
        }

        function createPollSuccessCallback(data) {
            navigateToManagePage(data.ManageId);
        }

        function getUserPolls() {
            PollService.getUserPolls()
                .success(function (data) {
                    $scope.userPolls = data;
                });
        }

        function navigateToManagePage(manageId) {
            RoutingService.navigateToManagePage(manageId);
        }

        function copyPoll(pollId) {
            PollService.copyPoll(pollId)
                .success(function (data) {
                    navigateToManagePage(data.newManageId);
                });
        }
    }

})();
