angular.module("umbraco.directives").directive('archetypeSubmitWatcher', function ($rootScope) {
    var linker = function (scope, element, attrs, ngModelCtrl) {
        // call the load callback on scope to obtain the ID of this submit watcher
        var id = scope.loadCallback ? scope.loadCallback() : 0;

        scope.$on("formSubmitting", function (ev, args) {
            // on the "formSubmitting" event, call the submit callback on scope to notify the Archetype controller to do it's magic
            if (id == scope.activeSubmitWatcher) {
                scope.submitCallback(args);
            }
        });
    }

    return {
        restrict: "E",
        replace: true,
        link: linker,
        template: "",
        scope: {
            loadCallback: '=',
            submitCallback: '=',
            activeSubmitWatcher: '='
        }
    }
});