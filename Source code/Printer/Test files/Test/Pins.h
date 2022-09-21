#ifndef PINS_H
#define PINS_H

#include "Pins.h"
#include <Servo.h>

class BraillePin
{
public:
    BraillePin(uint8_t pin, uint8_t idle, uint8_t pressed);
    void initialize();
    void pinUp();
    void pinDown();

private:
    uint8_t pin = 0;
    uint8_t idle = 0;
    uint8_t pressed = 0;
    Servo servo;
};

#endif