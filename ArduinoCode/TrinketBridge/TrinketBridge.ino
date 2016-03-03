/*******************************************************************

  PROJECT OWNER: Dave Glover | dglover@microsoft.com | @dglover

  The MIT License (MIT)
  
  Copyright (c) 2015 Microsoft Corporation
  
  Permission is hereby granted, free of charge, to any person obtaining a copy
  of this software and associated documentation files (the "Software"), to deal
  in the Software without restriction, including without limitation the rights
  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
  copies of the Software, and to permit persons to whom the Software is
  furnished to do so, subject to the following conditions:
  
  The above copyright notice and this permission notice shall be included in all
  copies or substantial portions of the Software.
  
  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
  SOFTWARE.


  CREATED: Mar 2016

  SUMMARY: Display a sweeping radar image of objects detected by a servo mounted HC-SR04 Ultrasonic Range senor.

  ACKNOWLEDGMENTS: Project inspired by Windows 10 IoT Core: UltraSonic Distance Mapper
  https://microsoft.hackster.io/en-US/AnuragVasanwala/windows-10-iot-core-ultrasonic-distance-mapper-d94d63?ref=platform&ref_id=4087_trending___&offset=15

  PLATFORM: Raspberry Pi 2 running Windows 10 IoT Core

  1 x Raspberry Pi 2 running WIndows 10 IoT Core
  1 x Trinket 3.3V https://www.adafruit.com/products/1500
  1 x Servo, in this example using a Tower Pro SG-5010 with 140 degree sweep
  1 x 1k resistor
  1 x 3.3 Zener Diode (see level shifter strategies - http://jamesreubenknowles.com/level-shifting-stragety-experments-1741)
  1 x mini breadboard
  1 x some jumper wires

  LIBRARIES: Though probably not best practice, all libraries are included in the project for longevity, versioning and simplicity reasons. See Required Libraries.
  
  REQUIRED LIBRARIES: Install Arduino Libraries from Sketch -> Include Library -> Manage Libraries
  - Adafruit SoftServo

  Download and install :-
  
  - TinyWireS (https://github.com/rambo/TinyWire)
  - TinyNewPing (https://github.com/Tony607/TinyNewPing)

*********************************************************************/

#include "Adafruit_SoftServo.h"  // SoftwareServo (works on non PWM pins)
#include "TinyWireS.h"           // wrapper class for I2C slave routines
#include "TinyNewPing.h"         // NewPing library modified for ATtiny


#define I2C_ADDRESS 0x40          //I2C Slave Address

#define SERVOPIN 4                // Servo control line (orange) on Trinket Pin
#define SERVO_MAX_DEGREES 140     // This sample uses Tower Pro SG-5010 with 140 degree sweep

#define RANGE_SENSOR_PIN  3       // Sensor 1 is connected to PB3
#define MAX_DISTANCE 250 // Maximum distance we want to ping for (in centimeters). Maximum sensor distance is rated at 400-500cm.

#define LEDPIN 1

byte i2cDataBuffer[6]; // i2c recieve data buffer

int lastServoPosition = 1000; // some random large number than the servo max degrees to trigger initial sensor measurement

bool ServoInitialised = false;
bool deferServoRefresh = false;  // minimise conflicts with event based servo refresh

byte Distance; // result from ultrasonic range sensor - limited to 8 bits 255 centimeters

Adafruit_SoftServo myServo;
NewPing SensorOne (RANGE_SENSOR_PIN, RANGE_SENSOR_PIN, MAX_DISTANCE);       // Define the Sensor - yes, using same pin for trigger and echo


enum ArduinoCmd : byte
{
    ServoMovePosition = (byte)'M'           //set Servo position
};

ArduinoCmd cmd;
unsigned short cmdIndex = 0; // worker variable

   
void setup() {
  // Set up the interrupt that will refresh the servo for us automagically
  OCR0A = 0xAF;            // any number is OK
  TIMSK |= _BV(OCIE0A);    // Turn on the compare interrupt (below!)

  pinMode(LEDPIN, OUTPUT);

  TinyWireS.begin(I2C_ADDRESS);      // init I2C Slave mode
  TinyWireS.onReceive(i2CReceived);   // register the onReceive() callback function
  TinyWireS.onRequest(requestEvent);         // When requested, call function transmit()

}

void loop()  {
  if (lastServoPosition != cmdIndex) {
      deferServoRefresh = true;      
      digitalWrite(LEDPIN, HIGH);   // turn the LED on (HIGH is the voltage level)
      
      Distance = SensorOne.ping_cm();        // Get distance in cm. Could be changed to 

      lastServoPosition = cmdIndex;
      digitalWrite(LEDPIN, LOW);   // turn the LED on (HIGH is the voltage level)      
      deferServoRefresh = false;
  }
  
  TinyWireS_stop_check(); 
}


/* This function will automatically be called when RPi2 sends data to this I2C slave */
void i2CReceived(uint8_t  NumberOfBytes)
{
  /***************
  I2C data packet format
  byte 0 = Command
  byte 1-2 = Command index
  byte 3-5 = arbitory could be red, green, blue for a neopixel or set neopixel string size etc or pin numnders for neopixel or servo
  ***************/

  deferServoRefresh = true;  
  
  if (NumberOfBytes != 6) {
    for (int b = 0; b < NumberOfBytes; b++){
      TinyWireS.receive();
    }
    return;
  }
  
  for (int b = 0; b < 6; b++){
    i2cDataBuffer[b] = TinyWireS.receive();
  }

  cmd = (ArduinoCmd)i2cDataBuffer[0];  
  cmdIndex = (i2cDataBuffer[1] << 8) + i2cDataBuffer[2];  // build unsigned short 16 bit index.

  switch(cmd){    
    case ServoMovePosition:
      if (!ServoInitialised) {
//          myServo1.attach(i2cDataBuffer[3]); 
          myServo.attach(SERVOPIN); 
          ServoInitialised = true;
      }
      setServoPosition(cmdIndex);
      break;
  }

  deferServoRefresh = false;
}

void setServoPosition(int degrees){
  if (degrees < 0) { degrees = 0; }
  if (degrees > SERVO_MAX_DEGREES) {degrees = SERVO_MAX_DEGREES; }
  int pos = map(degrees, 0, SERVO_MAX_DEGREES, 0, 255);
  myServo.write(pos);      
}

void requestEvent()
{
  deferServoRefresh = true;
  TinyWireS.send(Distance);                 // Send last recorded distance for current sensor
  deferServoRefresh = false;
}


// We'll take advantage of the built in millis() timer that goes off
// to keep track of time, and refresh the servo every 20 milliseconds
volatile uint8_t counter = 0;
SIGNAL(TIMER0_COMPA_vect) {
  counter += 2;           // this gets called every 2 milliseconds
  if (!deferServoRefresh && counter >= 40) {    // every 40 milliseconds, refresh the servos!
    counter = 0;
    myServo.refresh();
  }
}
