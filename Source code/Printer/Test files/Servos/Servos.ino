#include <Wire.h>
#include <Adafruit_PWMServoDriver.h>

Adafruit_PWMServoDriver servos = Adafruit_PWMServoDriver();
uint16_t servosIdle[6] = {150, 150, 150, 180, 180, 180};
uint16_t servosPressed[6] = {180, 180, 180, 150, 150, 150};

void setup(){
    Serial.begin(9600);
    servos.begin();
    servos.setOscillatorFrequency(27000000);
    servos.setPWMFreq(50);
    delay(100);
}

void loop(){
    Serial.println("Servos idle");
    servosUp();
    delay(1000);
    Serial.println("Servos pressed");
    servosDown();
    delay(1000);
}

void servosUp(){
    // Release servos:
    for (int i = 0; i < 6; i++){
        servos.setPWM(i, 0, servosIdle[i]);
    }
}

void servosDown(){
    // Release servos:
    for (int i = 0; i < 6; i++){
        servos.setPWM(i, 0, servosPressed[i]);
    }
}