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
byte Position = 70;
bool processing = false;
int newPosition = 0;

void setup()
{
	pinMode(13, OUTPUT);
	/* Initialize servo and move to initial position */
	MyServo.attach(Pin_Servo);
	MyServo.write(Position);

	/* Initialize I2C Slave & assign call-back function 'onReceive' */
	Wire.begin(MyAddress);
	Wire.onReceive(I2CReceived);
}

void loop() { /* Do nothing. Just wait for call-back 'onReceive' */ }

/* This function will automatically be called when RPi2 sends data to this I2C slave */
void I2CReceived(int NumberOfBytes)
{
	Position = Wire.read();

	if (processing) { return; }
	processing = true;

	if (Position < 0 || Position > 140) { return; }

	
	newPosition = Position;		/* WinIoT have sent position byte; read it */
	digitalWrite(13, HIGH);   // turn the LED on (HIGH is the voltage level)

 
	MyServo.write(newPosition);  // Move servo to specified the position //

	digitalWrite(13, LOW);

	processing = false;
}


