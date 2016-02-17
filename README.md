# Windows-IoT-Core-SonarScope

Original Project Inspiration [Windows 10 IoT Core: UltraSonic Distance Mapper](https://microsoft.hackster.io/en-US/AnuragVasanwala/windows-10-iot-core-ultrasonic-distance-mapper-d94d63)

This project has had a lot of love, optimisation and polish espically around XAML with special fade effects and object clean up.


[![Youtube](https://raw.githubusercontent.com/gloveboxes/Windows-IoT-Core-SonarScope/master/Resources/YouTube.JPG)](https://youtu.be/GKRDX3nHlks)

<iframe width="420" height="315" src="https://www.youtube.com/embed/GKRDX3nHlks" frameborder="0" allowfullscreen></iframe>

![Sonar Scope](https://raw.githubusercontent.com/gloveboxes/Windows-IoT-Core-SonarScope/master/Resources/SonarScope.jpg)

This project drives the servo over I2C bus from Windows IoT Core to Trinket Pro 3.3V Arduino board.

[Trinket Pro 3V](https://www.adafruit.com/products/2010) as it operates at the same voltage as the Raspberry Pi no logic level shifter required.


Note the [Bridge Driver](https://github.com/gloveboxes/Windows-IoT-Core-SonarScope/tree/master/ArduinoCode/ServoDriver/BridgeDriver) has support for 
Servos and NeoPixels.

![Trinket Pro 3.3V](https://raw.githubusercontent.com/gloveboxes/Windows-IoT-Core-SonarScope/master/Resources/SonarScopeArduino.jpg)

##Wiring

![layout](https://raw.githubusercontent.com/gloveboxes/Windows-IoT-Core-SonarScope/master/Resources/SonarScope_bb.jpg)