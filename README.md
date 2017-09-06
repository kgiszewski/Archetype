Archetype
=========
![alt tag](http://imulus.github.io/Archetype/images/logo.png)

![alt tag](http://imulus.github.io/Archetype/images/example1.png)

## Installation
Install the selected <a href='https://github.com/imulus/Archetype/releases'>release</a> through the Umbraco package installer or via <a href='http://www.nuget.org/packages/Archetype/'>NuGet</a>.

## Official Docs ##
https://github.com/kgiszewski/ArchetypeManual

Get up and running in 15 minutes! https://www.youtube.com/watch?v=79LksNwGjLk

## News and Updates ##
Follow us on Twitter https://twitter.com/ArchetypeKit

## Core Team ##
* Kevin Giszewski (founder/project lead) - University of Notre Dame - https://kevin.giszewski.com/
* Tom Fulton (founder) - Tonic - http://hellotonic.com/
* Lee Kelleher - Umbrella - http://www.umbrellainc.co.uk/
* Matt Brailsford - The Outfield - http://www.theoutfield.co.uk/
* Kenn Jacobsen - Vertica - http://kennjacobsen.dk/

## Donate!
[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=KBKWLURGLGU6L)

This project is funded by the core team members' time only. We don't charge for Archetype but it does take quite a bit of effort to keep it up-to-date with Umbraco core changes. If you're making a few bucks off of Archetype, we wouldn't refuse a donation :)

## Contribute ##

Want to contribute to Archetype?  You'll want to use Grunt (our task runner) to help you integrate with a local copy of Umbraco.

### Install Dependencies ###
*Requires Node.js to be installed and in your system path*

    npm install -g grunt-cli && npm install -g grunt
    npm install

### Build ###
    grunt

   Builds the project to `/dist/`.  These files can be dropped into an Umbraco 7 site, or you can build directly to a site using:

    grunt --target="D:\inetpub\mysite"

You can also watch for changes using:

    grunt watch
    grunt watch --target="D:\inetpub\mysite"


Add `--touch` to either command to automatically touch the web.config on a deploy
