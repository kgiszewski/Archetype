angular.module('umbraco.filters').filter("archetypeLocale", function(){
	return function(key, locales){

		if(!locales)
			return key;

		if(!locales.defaultLocale)
			return key;

		if(!locales.locale)
			return key;

		return locales.locale[key] || locales.defaultLocale[key] || key;
	}
})