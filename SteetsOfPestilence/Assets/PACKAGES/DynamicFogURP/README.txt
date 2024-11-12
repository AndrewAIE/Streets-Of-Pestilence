**************************************
*       VOLUMETRIC FOG & MIST 2      *
*         Created by Kronnect        *   
*            README FILE             *
**************************************

Requirements
------------
Dynamic Fog & Mist 2 currently works only with Universal Rendering Pipeline (v7.1.8 or later)
Make sure you have Universal RP package imported in the project before using Dynamic Fog & Mist 2.


Demo Scenes
-----------
There's a demo scene which lets you quickly check if Dynamic Fog & Mist 2 is working correctly in your project.
Note: make sure you have Universal RP 7.2.0 or later installed from Package Manager and also a URP pipeline asset assigned to Graphics Settings.


Documentation/API reference
---------------------------
The PDF is located in the Documentation folder. It contains instructions on how to use this asset as well as a useful Frequent Asked Question section.


Support
-------
Please read the documentation PDF and browse/play with the demo scenes and sample source code included before contacting us for support :-)

* Support-Web: https://kronnect.com/support
* Support-Discord: https://discord.gg/EH2GMaM
* Email: contact@kronnect.com
* Twitter: @Kronnect


Future updates
--------------

All our assets follow an incremental development process by which a few beta releases are published on our support forum (kronnect.com).
We encourage you to signup and engage our forum. The forum is the primary support and feature discussions medium.

Of course, all updates of Dynamic Fog & Mist will be eventually available on the Asset Store.


Version history
---------------

Current version
- Added "DynamicFog.requireSubvolumeUpdate" to force subvolume refresh (useful if you move subvolumes at runtime)
- [Fix] Fixes for orthographic projection

Version 5.0 10-AGO-2022
- Minimum Unity version required is now 2020.3.16
- Depth gradient color alpha now affects final fog transparency

Version 4.5 18-FEB-2022
- Added "Allow Rotation"

Version 4.4 12-NOV-2021
- Added "Use XYZ Distance" option. By disabling it, you can make the fog more consistent regardless of camera height.
- Added "Use Gradient" option. You can disable it to keep the fog color uniform.
- Added "Use Sun Color" option. When enabled, the fog color will be tinted with the color of the directional light.

Version 4.3.2 14-OCT-2021
- Changed fog algorithm to ignore camera far clipping plane distance when rendering over skyfog

v4.3.1 30-MAY-2021
- [Fix] Fixed banding artifacts related to the gradient texture
- [Fix] Changes to profiles used in subvolumes didn't affect the fog instance at runtime

v4.3 11-MAY-2021
- Added "Depth Clip" option
- Material setters optimizations

v4.2.3 14-APR-2021
- [Fix] Fixed conflict with Volumetric Fog 2 menu integration

v4.2.2 3-MAR-2021
- Diffusion light can now be disabled completely by setting its intensity to 0

v4.2.1 22-JAN-2021
- [Fix] Fixed inverted depth issue in URP 8.0 or later

v4.2 16-DEC-2020
- SubVolumes is now a separated option from previous "Fade" section.
- Added "Noise Distance Attenuation" option which reduces noise influence with distance.
- Fixes and improvements

v4.1 4-DEC-2020
- Added "Subvolumes" list option. Allows to filter which sub-volumes can affect this fog volume.

v4.0 27-NOV-2020
- Added "Fade" option. Allows smooth transitioning when moving from outside into the fog volume.
- Added "Show Boundary" option. Shows an overlay on the fog volume in Game View.
- Added "Box Projection" option. Traits depth as distance to camera near plane instead of camera position.
- Added "Max Height" and "Max Distance" options
- Added "Sub-Volume" support. Allows customizing fog properties in different areas within the same volume.
- Regrouped some options in inspector 

v3.1.1 15-OCT-2020
- [Fix] Fixes and diffusion effect improvements

v3.1 1-OCT-2020
- Added orthographic camera support

v3.0.1 18-SEP-2020
- [Fix] Fixed "Flip Depth Texture" option issue which prevented it from being applied

v3.0 15-SEP-2020
- Added Detail Noise option with custom strength and scale
- Added Boundary section (new boundary type: sphere)
- Added Vertical Offset option

v2.2.1 29-APR-2020
- [Fix] Fixed fog not rendering at distance due to camera far clip issue

v2.2 April / 2020
- [Fix] Fixed VR issues

v2.1 April / 2020
- Shader optimizations
- [Fix] Workaround for shadows issue on WebGL 2.0

v1.0 Febrary / 2020
First release