#include <Wire.h>
#include <Adafruit_PWMServoDriver.h>

Adafruit_PWMServoDriver servos = Adafruit_PWMServoDriver();
uint16_t servosIdle[6] = {1550, 1500, 1530, 1530, 1460, 1480};
uint16_t servosPressed[6] = {1450, 1400, 1430, 1630, 1560, 1580}; // lower value, higher pin

void setup(){
    Serial.begin(9600);
    servos.begin();
    servos.setOscillatorFrequency(27000000);
    servos.setPWMFreq(50);
    delay(200);
    servosDown();
    delay(200);
}

void loop(){
    servos.wakeup();
    servosUp();
    delay(10);
    servosUp();
    delay(10);
    servosUp();
    delay(10);
    servosUp();
    delay(500);
    servosDown();
    delay(10);
    servosDown();
    delay(10);
    servosDown();
    delay(10);
    servosDown();
    delay(50);
    servos.sleep();
    delay(10000);
}

void servosDown(){
    // Release servos:
    for (int i = 0; i < 6; i++){
        servos.writeMicroseconds(i, servosIdle[i]);
        delay(1);
    }
}

void servosUp(){
    // Release servos:
    for (int i = 0; i < 6; i++){
        servos.writeMicroseconds(i, servosPressed[i]);
        delay(1);
    }
}
