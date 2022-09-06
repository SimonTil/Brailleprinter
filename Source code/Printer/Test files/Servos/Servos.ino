#include <Wire.h>
#include <Adafruit_PWMServoDriver.h>

Adafruit_PWMServoDriver servos = Adafruit_PWMServoDriver();
uint16_t servosIdle[6] = {1560, 1510, 1530, 1500, 1480, 1490};
uint16_t servosPressed[6] = {1490, 1400, 1430, 1610, 1590, 1600};

void setup(){
    Serial.begin(9600);
    servos.begin();
    servos.setOscillatorFrequency(27000000);
    servos.setPWMFreq(50);
    delay(100);
    servosDown();
    delay(1000);
}

void loop(){
    servos.wakeup();
    servosUp();
    delay(2000);
    servosDown();
    delay(50);
    servos.sleep();
    delay(2000);
}

void servosDown(){
    // Release servos:
    for (int i = 0; i < 6; i++){
        servos.writeMicroseconds(i, servosIdle[i]);
        delay(1);
    }
    delay(400);
    }

void servosUp(){
    // Release servos:
    for (int i = 0; i < 6; i++){
        servos.writeMicroseconds(i, servosPressed[i]);
        delay(1);
    }
    delay(400);
}
