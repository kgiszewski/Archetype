angular.module("umbraco").directive('contentItem', function ($compile, $http) {
    
    var linker = function (scope, element, attrs) {

        if (scope.content.view)
        {
            $http.get(scope.content.view).success(function (data) {
                if (data) {
                    var rawTemplate = data;

                    //define the initial model and config
                    scope.model = {};
                    scope.model.config = {};

                    //pull these from the content
                    scope.model.value = scope.content.value;
                    scope.model.config = scope.content.config;

                    //some items need an alias
                    scope.model.alias = "scope-" + scope.$id;

                    //watch for changes since there is no two-way binding with the child values
                    scope.$watch('model.value', function (newValue, oldValue) {
                        scope.content.value = newValue;
                    });

                    //add label
                    if (true) {
                        rawTemplate = '<label>{{content.options.label}}</label>' + rawTemplate;
                    }

                    element.html(rawTemplate).show();
                    $compile(element.contents())(scope);
                }
            });
        }
    }

    return {
        restrict: "E",
        rep1ace: true,
        link: linker,
        scope: {
            content: '='
        }
    }
});