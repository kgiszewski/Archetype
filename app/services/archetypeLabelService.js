angular.module('umbraco.services').factory('archetypeLabelService', function (archetypeCacheService) {
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

    function getNativeLabel(datatype, value, scope) {

    	switch (datatype.selectedEditor) {
    		case "Imulus.UrlPicker":
    			return imulusUrlPicker(value, scope, {});
    		case "Umbraco.TinyMCEv3":
    			return coreTinyMce(value, scope, {});
            case "Umbraco.MultiNodeTreePicker":
                return coreMntp(value, scope, datatype);
            case "Umbraco.MediaPicker":
                return coreMediaPicker(value, scope, datatype);
            case "Umbraco.DropDown":
                return coreDropdown(value, scope, datatype);
    		default:
    			return "";
    	}
    }

    function coreDropdown(value, scope, args) {

        if(!value)
            return "";

        var prevalue = args.preValues[0].value[value];

        if(prevalue) {
            return prevalue.value;
        }

        return "";
    }

    function coreMntp(value, scope, args) {
        var ids = value.split(',');
        var type = "Document";

        switch(args.preValues[0].value.type) {
            case 'content':
                type = 'Document';
                break;
            case 'media':
                type = 'media';
                break;
            case 'member':
                type = 'member';
                break;

            default:
                break;
        }

        var entityArray = [];

        _.each(ids, function(id){
            if(id) {

                var entity = archetypeCacheService.getEntityById(scope, id, type);
                
                if(entity) {
                    entityArray.push(entity.name);
                }
            }
        });

        return entityArray.join(', ');
    }

    function coreMediaPicker(value, scope, args) {
        if(value) {
             var entity = archetypeCacheService.getEntityById(scope, value, "media");     
             
            if(entity) {
                return entity.name; 
            }
        }

        return "";
    }

    function imulusUrlPicker(value, scope, args) {

        if(!args.propertyName) {
            args = {propertyName: "name"}
        }

        var entity;

        switch (value.type) {
            case "content":
                if(value.typeData.contentId) {
                    entity = archetypeCacheService.getEntityById(scope, value.typeData.contentId, "Document");
                }
                break;

            case "media":
                if(value.typeData.mediaId) {
                    entity = archetypeCacheService.getEntityById(scope, value.typeData.mediaId, "Media");
                }
                break;

            case "url":
                return value.typeData.url;
                
            default:
                break;

        }

        if(entity) {
            return entity[args.propertyName];
        }

        return "";
    }

    function coreTinyMce(value, scope, args) {

        if(!args.contentLength) {
            args = {contentLength: 50}
        }

        var suffix = "";
        var strippedText = $(value).text();

        if(strippedText.length > args.contentLength) {
        	suffix = "…";
        }

        return strippedText.substring(0, args.contentLength) + suffix;
    }

	return {
		getFieldsetTitle: function(scope, fieldsetConfigModel, fieldsetIndex) {

            //console.log(scope.model.config);

            if(!fieldsetConfigModel)
                return "";

            var fieldset = scope.model.value.fieldsets[fieldsetIndex];
            var fieldsetConfig = scope.getConfigFieldsetByAlias(fieldset.alias);
            var template = fieldsetConfigModel.labelTemplate;

            if (template.length < 1)
                return fieldsetConfig.label;

            var rgx = /({{(.*?)}})*/g;
            var results;
            var parsedTemplate = template;

            var rawMatches = template.match(rgx);
            
            var matches = [];

            _.each(rawMatches, function(match){
                if(match) {
                    matches.push(match);
                }
            });

            _.each(matches, function (match) {

                // split the template in case it consists of multiple property aliases and/or functions
                var templates = match.replace("{{", '').replace("}}", '').split("|");
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

                    //if passed a function
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

                        templateLabelValue = executeFunctionByName(functionName, window, scope.getPropertyValueByAlias(fieldset, propertyAlias), scope, args);
                    }
                    //normal {{foo}} syntax
                    else {
                        propertyAlias = template;
                        var rawValue = scope.getPropertyValueByAlias(fieldset, propertyAlias);

                        templateLabelValue = rawValue;

                        //determine the type of editor
                        var propertyConfig = _.find(fieldsetConfigModel.properties, function(property){
                            return property.alias == propertyAlias;
                        });

                        if(propertyConfig) {
                        	var datatype = archetypeCacheService.getDatatypeByGuid(propertyConfig.dataTypeGuid);

                        	if(datatype) {

                            	//try to get built-in label
                            	var label = getNativeLabel(datatype, templateLabelValue, scope);

                            	if(label) {
                        			templateLabelValue = label;
                        		}
                        		else {
                        			templateLabelValue = templateLabelValue;
                        		}
                        	}
                        }
                        else {
                        	return templateLabelValue;
                        }

                    }                
                }

                if(!templateLabelValue) {
                    templateLabelValue = "";
                }
                
                parsedTemplate = parsedTemplate.replace(match, templateLabelValue);
            });

            return parsedTemplate;
        }
	}
});