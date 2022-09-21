#include "Pins.h"

BraillePin::BraillePin(uint8_t pin, uint8_t idle, uint8_t pressed)
{
    this->pin = pin;
    this->idle = idle;
    this->pressed = pressed;
    Servo servo;
}

void BraillePin::initialize()
{
    servo.attach(this->pin);
    this->pinDown();
}

void BraillePin::pinUp()
{
    servo.write(this->idle);
}

void BraillePin::pinDown()
{
    servo.write(this->pressed);
}