angular.module("umbraco.directives").directive('archetypeCustomView', function ($compile, $http) {
    var linker = function (scope, element, attrs) {

        var view = "/App_plugins/Archetype/views/archetype.default.html";
        if(scope.model.config.customViewPath) {
            view = config.customViewPath;
        }

        $http.get(view, { cache: true }).then(function(data) {

            element.html(data.data).show();

            $compile(element.contents())(scope);
        });
    }

    return {
        restrict: "A",
        replace: true,
        link: linker
    }
});