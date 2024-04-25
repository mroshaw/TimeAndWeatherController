# Time and Weather Controller

## By Daft Apple Games

# What is it

This is a Time of Day and Weather Controller for Unity. It sits on top of other assets, called "Providers", that actually provide Time of Day and Dynamic Sky functionality. The controller aims to combine the functions of a Time Provider and a Weather Provider, and add some unique touches of it's own, to give developers a single, unified place to control time and weather in their game.

The implementation currently comes with components to support the "Expanse" HDRP asset, and has been developed and tested with Expanse 1.7.3.

# Getting started

The repository has everything you need to get started. Just put the code into your Assets folder, check the pre-requisites below, and open the demo scene.

## Pre-requisites

To get the demo scenes to work, you'll need to add:

### Third party assets

- Expanse (obviously)

### Unity packages

- Text Mesh Pro
- High Definition RP > Samples > Particle System Shader Samples

The rain and thunder audio is included under the "Attribution-NonCommercial 4.0 International (CC BY-NC 4.0)" license, and is attributed to Alexander. You can find more of his work here: https://orangefreesounds.com/author/alexander/.

# How it works

From a technical perspective, the system works off of a small number of core classes:

- **TimeAndWeatherManager:MonoBehaviour** - this is the main manager class, implemented as a Singleton (whether you like it or not!), providing the main functionality of the controller. That is, public methods to control time and start new weather.
- **WeatherProviderBase:MonoBehaviour** - this is a MonoBehavior abstract class that provides common weather methods, as well as setting the abstract method patterns for specific providers.
- **TimeProviderBase:MonoBehaviour** - same as above, but for time methods.
- **WeatherPresetSettingsBase:ScriptableObject** - this is a ScriptableObject class that provides a common data structure and methods for storing "weather presets". These are the core inputs into the WeatherProvider, and contain definitions of a weather behaviour.
- **TimePresetSettingsBase:ScriptableObject** - same as above, but for time methods.

We then have the Expanse implementation classes:

- **ExpanseWeatherProvider:WeatherProviderBase** - this class provides abstract method definitions, and any custom methods specific to Expanse, to implement and apply weather profiles in Expanse.
- **ExpanseTimeProvider:TimeProviderBase** - same as above, but for time methods.
- **ExpanseWeatherPresetSettings:WeatherPresetSettingsBase** - this ScriptableObject class extends the base class to provide Expanse specific settings, required to define a specific weather preset.
- **ExpanseTimePresetSettings:TimePresetSettingsBase** - you guessed it, same as above but for time.

The idea here is that you could create a new set of "Provider" classes, implementing different Time of Day and Sky / Weather providers, and it would all "just work" with the TimeAndWeatherManager.

# Setting up presets

Presets are just instances of the provider Scriptable Objects, and can be created via the Assets > Create > Daft Apple Games > Weather menu item. If you look in Assets\DaftAppleGames\TimeAndWeatherController\Presets, you'll find a number of these already created. The most interesting one is likely "HeavyRainPreset", which shows the extent of what's configurable for a weather preset. The "MidnightPreset" time preset shows how you can link time and weather, effectively enforcing a clear night sky as the time transitions to midnight.

Most presets come with a "Transition Duration", which is a value in seconds used to Lerp from current settings to new preset settings. These can be adjusted individually, to allow you to get the effect you want.

Note that the "Audio Effect" and "Particle Effect" prefabs are all instantiated at Start(), and placed as children of the main camera. The particle effects are in world space, and so follow the camera around. The code comes with an example AudioMixer, with a separate AudioGroup for the ambient sounds associated to the weather sound effects.

# Other components

There are a number of components that provide debugging functionality, and also demonstrate how the Time and Weather Manager can be used. 

Some notable components include:

- **TimeAndWeatherEvents** - this component that can be used across multiple scenes, and that provides some useful Unity Events to hook into other parts of your scene. For example, these can be used to send NPCs to bed as night approaches, or for farm animals to come to life as dawn breaks.
- **WeatherSyncManager** - this component can be used to synchronise aspects of your weather and time with systems such as The Vegetation Engine and Microsplat. For example, to cause wetness to build in rain, or snow to accumulate during a heavy snow storm. This component is still in development.

# Coming up

These features are currently in the pipeline:

- Automatic time of day progression.
- Random weather transitions.
- Weather volumes.
- Indoor weather volumes.

# Some caveats

I am a hobby developer, and have only just started to scratch the surface of Unity and coding. As such, the code presented here may not be the cleanest or the most practical, and I suspect that in many ways I have overcomplicated things! Feedback, and of course PRs, are very welcome!

