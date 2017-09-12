# Information Collection Disclosure

As of version v1.16.0, we are checking your installed version of Archetype against the current version. In this process we collect the current version of Archetype along with the current version of Umbraco that you are running. We use these values to help determine a response message to you. You may opt-out of this process by 'un-checking' the option in any of the Archetypes you have configured in the `Developer->Data Types` section of Umbraco. 

![opt out](assets/optout3.png)

This is a global setting that will set an `AppSetting` and update a file located at `~/config/archetype.config.js`. If you manually change the setting it will require an app reload to refresh to runtime values. If somehow this file becomes corrupt, a new file will be generated and you will again have to opt out.

We identify your install only by a randomly generated GUID. We WILL NOT store your IP address or hostname during the update check. 

Part of the reason we collect this information is to also ascertain how many active installs exist. If this number is sufficiently high, we can plan accordingly and attempt to keep this project going.

We would ask that you reach out to the Archetype team via creating an issue on this site if you have any questions or concerns. We truly want Archetype to be a win-win for all involved. 
