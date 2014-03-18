Archetype
=========
![alt tag](http://imulus.github.io/Archetype/images/logo.png)

## Installation
Install the selected <a href='https://github.com/imulus/Archetype/releases'>release</a> through the Umbraco package installer or via <a href='http://www.nuget.org/packages/Archetype/'>NuGet</a>.


## Official Docs ##
http://imulus.github.io/Archetype

## Contribute ##

Want to contribute to Archetype?  You'll want to use Grunt (our task runner) to help you integrate with a local copy of Umbraco.

### Install Dependencies ###
*Requires Node.js to be installed and in your system path*

    npm install -g grunt-cli && npm install -g grunt
    npm install

### Build ###
    grunt # Compiles to `/dist/`
    grunt --target="D:\path\to\umbraco\site"

    grunt watch
    grunt watch --target="D:\path\to\umbraco\site"

Add `--touch` to either command to automatically touch the web.config on a deploy