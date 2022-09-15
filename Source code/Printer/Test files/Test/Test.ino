#include <Wire.h>
#include <Adafruit_PWMServoDriver.h>
Adafruit_PWMServoDriver servos = Adafruit_PWMServoDriver();
uint16_t servosIdle[6] = { 2000, 2000, 1500, 1000, 1000, 1000 };
uint16_t servosPressed[6] = { 1240, 1410, 750, 1910, 1640, 1630 };

void setup(){
  servos.begin();
    servos.setOscillatorFrequency(27000000);
    servos.setPWMFreq(50);
    for(int i = 0; i < 6; i++){
        servos.writeMicroseconds(i, servosIdle[i]);
    }
}

void loop(){
}
