I spent the past week building this to allow the ChatMix dial for the SteelSeries Arctis Nova 7 to work without Sonar so I could use Wave Link Virtual Inputs with it, because I was constantly having issues with WaveLink and Sonar fighting each other. How did I do this? I analyzed the traffic using USBPCap to see what data was spit out when I used the ChatMix dial. This allows the default driver to control everything else saving me the time of learning how to built a custom driver. I'm a PHP programmer with some experience in Python, and Zero in C#, so I'm a bit outside my comfort zone, but I wanted a nice interface as my buddy was also frustrated with the software and wanted to use it. I built the backend in Python and the frontend in C#. I compiled the Python code using PyInstaller so that you don't need to setup a Python environment and so it just launches with the app. You'll need DotNet6 if you don't already have it. I'm including the source code as well so that maybe the community can add support for other operating systems and possibly headsets. I feel most of the heavy lifting is done, so it hopefully shouldn't be too bad. I don't have a Mac to test it, but I'm pretty sure it only works on Windows currently. Do note, the code base is a hot mess and I'm sorry, but I spent more time on this than I originally wanted to and it's not broke, so I'm not fixing it. Someone more experienced in C# feel free to submit a PR if you want to clean up the mess.

I'm also including the source code for using the existing SteelSeries Sonar - Gaming and SteelSeries Sonar - Chat that feeds them into the Headset. It works, but there is latency, so that's why it's not included in the compiled release. It's probably because the library records it instead of directly passing it, but I couldn't find another library that would do what I want and I don't know enough to write my own. I'm hoping someone else can take a look and figure out the latency issues, because I've been looking at all this for far too long. Maybe I'll come back to it.

Setup
Drag the exe into your startup folder so it runs on startup:
C:\Users\YOURUSERNAME\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup

Launch Get Good

Set your Game Input to Wave Link System and your Chat Input to Wave Link Voice Chat.

Set your Discord output to Wave Link Voice Chat

Click the audio icon in the system tray and change the input to Wave Link System

In Wave Link set your Monitor Mix to Headphones (Arctis Nova 7)

If you don't already have it, add a new input in Wavelink for System.

Now you won't have to route all your game audio in game to a Game Virtual Audio Input.

The app uses nicenames, so if you right click your speaker icon in your system tray, click sounds, you'll see the names of the devices, it's the first name for the device.

Hopefully this helps someone out there. If others use it awesome, if not, I needed it anyway.


Python Libraries
https://github.com/pyusb/pyusb/blob/master/LICENSE
https://github.com/pyinstaller/pyinstaller?tab=License-1-ov-file#readme


C# Libraries
Microsoft.NET.ILLink.Analyzers
Microsoft.NET.ILLink.Tasks
Microsoft.Win32.Registry
System.Security.AccessControl
System.Security.Principal.Windows
NAudio
NAudio.Asio
NAudio.Core
NAudio.Midi
NAudio.WinForms
NAudio.WinMM


Icon
https://publicdomainvectors.org/en/free-clipart/Headphones-pictogram-vector-graphics/23136.html