# Windows-IoT-Core-SonarScope

Original Project Inspiration [Windows 10 IoT Core: UltraSonic Distance Mapper](https://microsoft.hackster.io/en-US/AnuragVasanwala/windows-10-iot-core-ultrasonic-distance-mapper-d94d63)


![Sonar Scope](https://raw.githubusercontent.com/gloveboxes/Windows-IoT-Core-SonarScope/master/Resources/SonarScope.jpg)

This project drives the servo over I2C bus from Windows IoT Core to Trinket Pro 3.3V Arduino board.

[Trinket Pro 3V](https://www.adafruit.com/products/2010) as it operates at the same voltage as the Raspberry Pi no logic level shifter required.


Note the [Bridge Driver](https://github.com/gloveboxes/Windows-IoT-Core-SonarScope/tree/master/ArduinoCode/ServoDriver/BridgeDriver) has support for 
Servos and NeoPixels.

![Trinket Pro 3.3V](https://raw.githubusercontent.com/gloveboxes/Windows-IoT-Core-SonarScope/master/Resources/SonarScopeArduino.jpg)