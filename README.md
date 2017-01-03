# Windows-IoT-Core-SonarScope

Original Project Inspiration [Windows 10 IoT Core: UltraSonic Distance Mapper](https://microsoft.hackster.io/en-US/AnuragVasanwala/windows-10-iot-core-ultrasonic-distance-mapper-d94d63)

This project has had a lot of love, optimisation and polish espically around XAML with special fade effects and object clean up.


[![Youtube](https://raw.githubusercontent.com/gloveboxes/Windows-IoT-Core-SonarScope/master/Resources/YouTube.JPG)](https://youtu.be/GKRDX3nHlks)

<iframe width="420" height="315" src="https://www.youtube.com/embed/GKRDX3nHlks" frameborder="0" allowfullscreen></iframe>

![Sonar Scope](https://raw.githubusercontent.com/gloveboxes/Windows-IoT-Core-SonarScope/master/Resources/SonarScopeMiniTrinket3.3v.jpg)

![Sonar Scope](https://raw.githubusercontent.com/gloveboxes/Windows-IoT-Core-SonarScope/master/Resources/SonarScopeMiniTrinket3.3vAbove.jpg)

This project drives the servo over I2C bus from Windows IoT Core to Trinket Mini 3.3V Arduino board.

[Trinket Mini 3.3V](https://www.adafruit.com/products/1500) as it operates at the same voltage as the Raspberry Pi no logic level shifter required.




## Parts List

1. Raspberry Pi 2 or 3
2. [Adafruit Trinket - Mini Microcontroller - 3.3V Logic - MicroUSB](https://www.adafruit.com/products/1500)
3. [Zenor Diode 3.3v 1N4728A](http://au.rs-online.com/web/p/zener-diodes/8050034/)
4. 1K ohm resistor
5. 1 x Servo. I used a [Tower Pro SG-5010](https://www.adafruit.com/product/155)
6. HC-SR04 ultrasonic sensor. Widely avaiable sensor
7. [Servo Power Connector](https://www.jaycar.com.au/2-1mm-dc-socket-with-screw-terminals/p/PA3713)


## Powering the Servo

Ideally you should power the servo from its own separate power supply. However, given the servo is under very light load and if you are careful not to stall the servo you will likely get away with powering the servo from the 5v pin on the Raspberry Pi.

### Notes

1. A lightly loaded servo will draw around 200 milliamps
2. [Max current that can be supplied by 5V pin](http://raspberrypi.stackexchange.com/questions/42630/is-there-a-max-current-that-can-be-supplied-when-powering-the-pi-from-the-pins)


##Wiring

![layout](https://raw.githubusercontent.com/gloveboxes/Windows-IoT-Core-SonarScope/master/Resources/SonarScope_bb.jpg)