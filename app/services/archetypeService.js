angular.module('umbraco.services').factory('archetypeService', function () {
    //private
    function executeFunctionByName(functionName, context) {
        var args = Array.prototype.slice.call(arguments).splice(2);

        var namespaces = functionName.split(".");
        var func = namespaces.pop();

        for(var i = 0; i < namespaces.length; i++) {
            context = context[namespaces[i]];
        }

        if(context && context[func]) {
            return context[func].apply(this, args);
        }

        return "";
    }

    //public
    return {
        getFieldsetTitle: function($scope, fieldsetConfigModel, fieldsetIndex) {
            if(!fieldsetConfigModel)
                return "";
            var fieldset = $scope.model.value.fieldsets[fieldsetIndex];
            var fieldsetConfig = $scope.getConfigFieldsetByAlias(fieldset.alias);
            var template = fieldsetConfigModel.labelTemplate;

            if (template.length < 1)
                return fieldsetConfig.label;

            var rgx = /{{([^)].*)}}/g;
            var results;
            var parsedTemplate = template;

            while ((results = rgx.exec(template)) !== null) {

                // split the template in case it consists of multiple property aliases and/or functions
                var templates = results[0].replace("{{", '').replace("}}", '').split("|");
                var templateLabelValue = "";

                for(var i = 0; i < templates.length; i++) {
                    // stop looking for a template label value if a previous template part already yielded a value
                    if(templateLabelValue != "") {
                        break;
                    }
                    
                    var template = templates[i];
                    
                    //test for function
                    var beginParamsIndexOf = template.indexOf("(");
                    var endParamsIndexOf = template.indexOf(")");

                    if(beginParamsIndexOf != -1 && endParamsIndexOf != -1)
                    {
                        var functionName = template.substring(0, beginParamsIndexOf);
                        var propertyAlias = template.substring(beginParamsIndexOf + 1, endParamsIndexOf).split(',')[0];

                        var args = {};

                        var beginArgsIndexOf = template.indexOf(',');

                        if(beginArgsIndexOf != -1) {

                            var argsString = template.substring(beginArgsIndexOf + 1, endParamsIndexOf).trim();

                            var normalizedJsonString = argsString.replace(/(\w+)\s*:/g, '"$1":');

                            args = JSON.parse(normalizedJsonString);
                        }

                        templateLabelValue = executeFunctionByName(functionName, window, $scope.getPropertyValueByAlias(fieldset, propertyAlias), $scope, args);
                    }
                    else {
                        propertyAlias = template;
                        templateLabelValue = $scope.getPropertyValueByAlias(fieldset, propertyAlias);
                    }                
                }
                parsedTemplate = parsedTemplate.replace(results[0], templateLabelValue);
            }

            return parsedTemplate;
        }
    }
});