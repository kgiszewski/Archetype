Archetype
=========

## Install Dependencies ##
*Requires Node.js to be installed and in your system path*

    npm install -g grunt-cli && npm install -g grunt
    npm install

## Build ##
    grunt


## Deploy ##
    grunt deploy --target=C:\\path\\to\\umbraco\\site
    grunt watch:dev --target=C:\\path\to\\umbraco\\site

Add `--touch` to either command to automatically touch the web.config on a deploy

## Installation
1) Install the <a href='http://bit.ly/1gUYKW8'>package</a> through Umbraco.

OR

2) Deploy with Grunt (see above).  Recommended if you wish to extend this repo.

OR

3) Not recommended but a down and dirty way is to manually download and copy the files into the appropriate spot:

Your /App_Plugins (and /bin) should contain this structure:

    /App_Plugins
    - package.manifest
    - /css
    -- archetype.css
    - /js
    -- archetype.js (our Grunt script concats the controller.js/config.controller.js, services and all directives into one file, you will have to do so manually)
    -- config.views.js
    - /views
    -- archetype.html
    -- archetype.config.html
    /bin
    -- archectype.dll (you'll have to build it)

## Prevalue Configs ##

Most of the fields are self evident, the following fields will be discussed further:
    
###Developer Mode###

By turning this on, you will get a textarea visible with the data model (not prevalue model).  You can change the contents of the textarea for real-time editing.  Additionally when in developer mode, the console will reflect the scope object of each property.

##View Compatibility##
The basis of compatibility is based on the following:

- The model of the view must use `$scope.model.value` and the config must use `$scope.model.config`.

All core properties have not been tested yet :)

##Developer Notes##
If you decide to extend this project locally, you'll want to set the `<compilation>` `debug` property in your `web.config` to `true`:

    <compilation defaultLanguage="c#" debug="true" batch="false" targetFramework="4.5">

This is in order to circumvent the minification and caching of the JavaScript files.
