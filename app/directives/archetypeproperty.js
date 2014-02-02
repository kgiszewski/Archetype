angular.module("umbraco").directive('archetypeProperty', function ($compile, $http, propertyEditorResource, umbPropEditorHelper) {
    
    function getFieldsetByAlias(fieldsets, alias)
    {
        return _.find(fieldsets, function(fieldset){
            return fieldset.alias == alias;
        });
    }

    function getPropertyIndexByAlias(properties, alias)
    {
        for (var i in properties)
        {
            if (properties[i].alias == alias) {
                return i;
            }
        }
    }

    //helper that returns a JS ojbect from 'value' string or the original string
    function jsonOrString(value, developerMode, debugLabel){
        if(value && typeof value == 'string'){
            try{
                if(developerMode == '1'){
                    console.log("Trying to parse " + debugLabel + ": " + value); 
                }
                value = JSON.parse(value);
            }
            catch(exception)
            {
                if(developerMode == '1'){
                    console.log("Failed to parse " + debugLabel + "."); 
                }
            }
        }

        if(value && developerMode == '1'){
            console.log(debugLabel + " post-parsing: ");
            console.log(value); 
        }

        return value;
    }

    var linker = function (scope, element, attrs) {
        var configFieldsetModel = getFieldsetByAlias(scope.archetypeConfig.fieldsets, scope.fieldset.alias);
        var view = "";
        var label = configFieldsetModel.properties[scope.propertyConfigIndex].label;
        var dataTypeId = configFieldsetModel.properties[scope.propertyConfigIndex].dataTypeId;
        var config = null;
        var alias = configFieldsetModel.properties[scope.propertyConfigIndex].alias;
        var defaultValue = configFieldsetModel.properties[scope.propertyConfigIndex].value;

        //try to convert the defaultValue to a JS object
        defaultValue = jsonOrString(defaultValue, scope.archetypeConfig.developerMode, "defaultValue");

        //grab info for the selected datatype, prepare for view
        propertyEditorResource.getDataType(dataTypeId).then(function (data) {
            //transform preValues array into object expected by propertyeditor views
            var configObj = {};
            _.each(data.preValues, function(p) {
                configObj[p.key] = p.value;
            });
            config = configObj;

            //determine the view to use [...] and load it
            propertyEditorResource.getPropertyEditorMapping(data.selectedEditor).then(function(propertyEditor) {
                var pathToView = umbPropEditorHelper.getViewPath(propertyEditor.view);

                //load in the DefaultPreValues for the PropertyEditor, if any
                var defaultConfigObj =  {};
                if (propertyEditor.hasOwnProperty('defaultPreValues')) {
                    _.each(propertyEditor.defaultPreValues, function(p) {
                        _.extend(defaultConfigObj, p)
                    });
                }
                var mergedConfig = _.extend(defaultConfigObj, config);

                loadView(pathToView, mergedConfig, defaultValue, alias, scope, element);
            });
        });

    }

    function loadView(view, config, defaultValue, alias, scope, element) {
        if (view)
        {
            $http.get(view).success(function (data) {
                if (data) {
                    if (scope.archetypeConfig.developerMode == '1')
                    {
                        console.log(scope);
                    }

                    //define the initial model and config
                    scope.model = {};
                    scope.model.config = {};

                    //ini the property value after test to make sure a prop exists in the renderModel
                    var renderModelPropertyIndex = getPropertyIndexByAlias(scope.archetypeRenderModel.fieldsets[scope.fieldsetIndex].properties, alias);

                    if (!renderModelPropertyIndex)
                    {
                        scope.archetypeRenderModel.fieldsets[scope.fieldsetIndex].properties.push(JSON.parse('{"alias": "' + alias + '", "value": "' + defaultValue + '"}'));
                        renderModelPropertyIndex = getPropertyIndexByAlias(scope.archetypeRenderModel.fieldsets[scope.fieldsetIndex].properties, alias);
                    }
                    scope.model.value = scope.archetypeRenderModel.fieldsets[scope.fieldsetIndex].properties[renderModelPropertyIndex].value;

                    //set the config from the prevalues
                    scope.model.config = config;

                    //some items need an alias
                    scope.model.alias = "archetype-property-" + scope.fieldsetIndex + "-" + scope.propertyConfigIndex;

                    //watch for changes since there is no two-way binding with the local model.value
                    scope.$watch('model.value', function (newValue, oldValue) {
                        scope.archetypeRenderModel.fieldsets[scope.fieldsetIndex].properties[renderModelPropertyIndex].value = newValue;
                    });

                    element.html(data).show();
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
            property: '=',
            propertyConfigIndex: '=',
            archetypeConfig: '=',
            fieldset: '=',
            fieldsetIndex: '=',
            archetypeRenderModel: '='
        }
    }
});