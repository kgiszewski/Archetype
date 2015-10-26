angular.module("umbraco.directives")
    .directive('archetypeClickOutside', function ($timeout, $parse) {
        return {
            restrict: 'A',
            link: function (scope, element, attrs, ctrl) {
                var fn = $parse(attrs.archetypeClickOutside);

                // add click event handler (delayed so we don't trigger the callback immediately if this directive itself was triggered by a mouse click)
                $timeout(function () {
                  $(document).on("click", mouseClick);
                }, 500);

                function mouseClick(event) {  
                    if($(event.target).closest(element).length > 0) {
                        return;
                    }
                    var callback = function () {
                        fn(scope, { $event: event });
                    };
                    scope.$apply(callback);
                }

                // unbind event
                scope.$on('$destroy', function () {
                    $(document).off("click", mouseClick);
                });
            }
        };
    });