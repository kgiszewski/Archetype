angular.module("umbraco.directives").directive('archetypeCustomView', function ($compile, $http) {
    var linker = function (scope, element, attrs) {

        var config = scope.archetype.config.archetypeConfig;

        var view = "/App_plugins/Archetype/views/archetype.default.html";
        if(config.customViewPath) {
            view = config.customViewPath;
        }

        $http.get(view).then(function(data) {

            scope.model = {};
            scope.model.value = scope.archetype.value;
            scope.model.config = scope.archetype.config;

            element.html(data.data).show();

            $compile(element.contents())(scope);
        });
    }

    return {
        restrict: "A",
        replace: true,
        link: linker,
        scope: {
            archetype: "="
        }
    }
});