(function(angular) {
    'use strict';

    angular.module('app.routes', ['ngRoute'])
        .config([
            '$routeProvider', '$locationProvider', function($routeProvider, $locationProvider) {
                $routeProvider.
                    when('/', {
                        templateUrl: '/ui/app/common/view.html',
                        title: 'A Sample Data Grid!'
                    }).when('/form/:param', {
                        templateUrl: '/ui/app/common/form.html',
                        title: 'This is a form!'
                    }).when('/login', {
                        templateUrl: '/ui/app/common/login.html',
                        title: 'Login'
                    }).when('/new/', {
                        templateUrl: '/ui/app/common/formnew.html',
                        title: 'Add a new ORDER NOW!'
                    }).otherwise({
                        redirectTo: '/'
                    });

                $locationProvider.html5Mode(true);
            }
        ]);

    angular.module('app.controllers', ['tubular.services'])
        .controller('titleController', [
            '$scope', '$route', function ($scope, $route) {
                var me = this;
                me.content = 'Home';

                $scope.$on('$routeChangeSuccess', function () {
                    me.content = $route.current.title;
                });
            }
        ]).controller('i18nCtrl', [
            '$scope', 'tubularTranslate', 'toastr', function ($scope, tubularTranslate, toastr) {
                $scope.toggle = function () {
                    tubularTranslate.setLanguage(tubularTranslate.currentLanguage === 'en' ? 'es' : 'en');
                    toastr.info('New language: ' + tubularTranslate.currentLanguage);
                };
            }
        ])
        .controller('tubularSampleCtrl', [
            '$scope', '$location', 'toastr', '$http', 'tubularConfig',
            function ($scope, $location, toastr, $http, tubularConfig) {
                var me = this;

                tubularConfig.webApi.requireAuthentication(false);
                $http.get('api/orders/cities').then(function (response) {
                    $scope.cities = [];
                    angular.forEach(response.data, function (value) {
                        $scope.cities.push(value.Key);
                    });
                });

                me.onTableController = function () {
                    console.log('On Before Get Data Event: fired.');
                };

                me.defaultDate = new Date();

                me.ColumnName = 'Date';
                me.Filter = 'Oxxo';

                // Grid Events
                $scope.$on('tbGrid_OnBeforeRequest', function (event, eventData) {
                    console.log(eventData);
                });

                $scope.$on('tbGrid_OnRemove', function () {
                    toastr.success('Record removed');
                });

                $scope.$on('tbGrid_OnConnectionError', function (error) {
                    toastr.error(error.statusText || 'Connection error');
                });

                $scope.$on('tbGrid_OnSuccessfulSave', function () {
                    toastr.success('Record updated');
                });

                // Form Events
                $scope.$on('tbForm_OnConnectionError', function (error) { toastr.error(error.statusText || 'Connection error'); });

                $scope.$on('tbForm_OnSuccessfulSave', function (event, data, formScope) {
                    toastr.success('Record updated');
                    if (formScope) formScope.clear();
                });

                $scope.$on('tbForm_OnSavingNoChanges', function () {
                    toastr.warning('Nothing to save');
                    $location.path('/');
                });

                $scope.$on('tbForm_OnCancel', function () {
                    $location.path('/');
                });
            }
        ]).controller('loginCtrl',
            function ($scope, $location, tubularHttp, $uibModal, $routeParams, toastr) {
                $scope.loading = false;
                $scope.tokenReset = $routeParams.token;

                $scope.submitForm = function () {
                    if (!$scope.username ||
                        !$scope.password ||
                        $scope.username.trim() === '' ||
                        $scope.password.trim() === '') {
                        toastr.error('', 'You need to fill in a username and password');
                        return;
                    }

                    $scope.loading = true;

                    tubularHttp.authenticate($scope.username, $scope.password).then(
                        function (data) {
                            $location.path('/');
                        },
                        function (error) {
                            $scope.loading = false;
                            toastr.error(error.error_description);
                        });
                };
            });

    angular.module('app', [
        'ngAnimate',
        'tubular',
        'toastr',
        'app.routes',
        'app.controllers'
    ]);
})(angular);