/*
  Gateway sketch for UltraSonic Distance Mapper (RPi2 + WinIoT)
  https://www.hackster.io/AnuragVasanwala/windows-10-iot-core-ultrasonic-distance-mapper-d94d63

  Objectives:
	+ Initialize Servo
	+ Initialize I2C Slave in address 0x40 (64 DEC)
	+ Move servo to specified position when master (WinIoT) sends position byte
*/
#include <Wire.h>
#include <Servo.h> 

#define MyAddress 0x40
#define Pin_Servo 3

/* Create object for servo motor */
Servo MyServo;

/* Initial servo position */
byte oldPosition = 0;
int newPosition = 70;

void setup()
{
	pinMode(13, OUTPUT);
	/* Initialize servo and move to initial position */
	MyServo.attach(Pin_Servo);

	/* Initialize I2C Slave & assign call-back function 'onReceive' */
	Wire.begin(MyAddress);
	Wire.onReceive(I2CReceived);
}


void loop() {
  if (newPosition != oldPosition) {
    digitalWrite(13, HIGH);   // turn the LED on (HIGH is the voltage level) 

    oldPosition = newPosition;
    MyServo.write(oldPosition);  // Move servo to specified the position //
    
    digitalWrite(13, LOW);
  }
}


/* This function will automatically be called when RPi2 sends data to this I2C slave */
void I2CReceived(int NumberOfBytes)
{
  newPosition = Wire.read();
}
