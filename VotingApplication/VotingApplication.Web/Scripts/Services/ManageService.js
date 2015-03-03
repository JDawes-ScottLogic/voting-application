﻿(function () {
    angular.module('GVA.Creation').factory('ManageService', ['$location', '$http', '$routeParams', function ($location, $http, $routeParams) {
        var self = this;

        var observerCallbacks = [];

        var notifyObservers = function () {
            angular.forEach(observerCallbacks, function (callback) {
                callback();
            });
        };

        self.poll = null;

        self.registerPollObserver = function (callback) {

            if (self.poll == null) {
                self.getPoll($routeParams.manageId);
            }
            
            observerCallbacks.push(callback);
        }

        self.poll = null;

        self.getPoll = function (manageId, callback) {

            if (!manageId) {
                return null;
            }

            $http({
                method: 'GET',
                url: '/api/manage/' + manageId
            })
            .success(function (data) { self.poll = data; notifyObservers(); if (callback) { callback(data) } })
            .error(function (data, status) { if (failureCallback) { failureCallback(data, status) } });

        }
        
        self.updatePoll = function (callback, failureCallback) {
            $http({
                method: 'PUT',
                url: '/api/manage/' + $routeParams.manageId,
                data: self.poll
            })
            .success(function (data) { self.poll = data; notifyObservers(); if (callback) { callback(data) } })
            .error(function (data, status) { if (failureCallback) { failureCallback(data, status) } });
        }

        return self;
    }]);
})();
