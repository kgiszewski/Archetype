angular.module('umbraco.services').factory('archetypeGlobalConfigService', function ($q, $http, umbRequestHelper) {
    return {
        globalSettings: function () {
            return umbRequestHelper.resourcePromise(
                $http.get("backoffice/ArchetypeApi/ArchetypeDataType/globalSettings"), 'Failed to get whether or not we are checking for updates!'
            );
        },
        setCheckForUpdates: function (isChecking) {
            return umbRequestHelper.resourcePromise(
                $http.post("backoffice/ArchetypeApi/ArchetypeDataType/SetCheckForUpdates", isChecking), 'Failed to update check status!'
            );
        },
        checkForUpdates: function () {
            return umbRequestHelper.resourcePromise(
                $http.post("backoffice/ArchetypeApi/ArchetypeDataType/checkForUpdates", { }), 'Failed to check for updates!'
            );
        }
    }
});