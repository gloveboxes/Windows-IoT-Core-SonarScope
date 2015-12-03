#include <Wire.h>
#include <Adafruit_NeoPixel.h>
#include <Servo.h> 

#define MyAddress 0x40

Adafruit_NeoPixel pixels;
unsigned short numPixels = 0;  // How many NeoPixels are attached to the Arduino?


/* Create object for servo motor */
Servo MyServo;
bool ServoInitialised = false;
//byte oldPosition = 0;
//int newPosition = 70;

enum ArduinoCmd : byte
{
    InitialiseNeoPixel = (byte)'I',     //Initialize NeoPixel
    SetNeoPixel = (byte)'P',               //set a NeoPixel pixel value
    DisplayNeoPixel = (byte)'D',         //render NeoPixel pixels to the display
    ServoMovePosition = (byte)'M'           //set Servo position
};

ArduinoCmd cmd;

void setup()
{

//  Serial.begin(115200);
//  while (!Serial) {
//    ; // wait for serial port to connect. Needed for Leonardo only
//  }
  

  pinMode(13, OUTPUT);

  Wire.begin(MyAddress);    /* Initialize I2C Slave & assign call-back function 'onReceive' */
  Wire.onReceive(I2CReceived);
  Wire.onRequest(requestEvent); //Zgłaszane przy żądaniu odczytu (nadawca: Read)
}


void loop() {
// if (ServoInitialised && newPosition != oldPosition) {
//    digitalWrite(13, HIGH);   // turn the LED on (HIGH is the voltage level) 
//
//    oldPosition = newPosition;
//    MyServo.write(oldPosition);  // Move servo to specified the position //
//
//    digitalWrite(13, LOW);
//  }
}

void requestEvent() {
//    Serial.println("RESPONSE");
//    Wire.write(arr,2);
}

/* This function will automatically be called when RPi2 sends data to this I2C slave */
void I2CReceived(int NumberOfBytes)
{
  int temp;

//data format
//byte 0 = Command
//byte 1-2 = index
//byte 3-5 = arbitory could be red, green, blue for a neopixel or set neopixel string size etc or pin numnders for neopixel or servo
  
  byte data[6];
  unsigned short index = 0;
  
  if (NumberOfBytes != 6) {
    for (int b = 0; b < NumberOfBytes; b++){
      Wire.read();
    }
    return;
  }
  
  for (int b = 0; b < 6; b++){
    data[b] = Wire.read();
  }


  cmd = (ArduinoCmd)data[0];  // S = Set pixel, // R = Render Frame
  index = (data[1] << 8) + data[2];  // build unsigned short 16 bit index

  switch(cmd){
    case InitialiseNeoPixel:  // Initialise
      if (numPixels > 0 || index > 512) { return; }
      numPixels = index;
      pixels = Adafruit_NeoPixel(numPixels, data[3], NEO_GRB + NEO_KHZ800);
      pixels.begin(); // This initializes the NeoPixel library.
      break;
      
    case DisplayNeoPixel:  // display neopixel
      if (numPixels == 0) {
        if (index > 512) { return; }
        numPixels = index;
        pixels = Adafruit_NeoPixel(numPixels, data[3], NEO_GRB + NEO_KHZ800);
        pixels.begin(); // This initializes the NeoPixel library.
      }
      else {
        digitalWrite(13, HIGH);
        pixels.show(); // This sends the updated pixel color to the hardware.
        digitalWrite(13, LOW);
      }
      break;
      
    case SetNeoPixel:  // Set Pixel
      if (numPixels == 0 || index > numPixels) { return; }
      pixels.setPixelColor(index, pixels.Color(data[3],data[4],data[5]));
      break;
    
    case ServoMovePosition:
      if (!ServoInitialised) {
          MyServo.attach(data[3]); 
          ServoInitialised = true;
      }
      MyServo.write(index);
      break;
  }
}
